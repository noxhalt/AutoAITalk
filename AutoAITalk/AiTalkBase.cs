#nullable enable
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UIAutomationClient;
using static AutoAITalk.Sugar;

namespace AutoAITalk
{
    public abstract class AiTalkBase
    {
        private const int DefaultTick = 20;
        
        protected static readonly CUIAutomation Automation = new();

        private readonly string _name;
        private readonly string _title;
        protected IUIAutomationElement? MainWindow;

        protected AiTalkBase(string name, string title)
        {
            _name = name;
            _title = title;
            Attach();
        }

        // Process attacher

        private bool Attach(Process? process) =>
            TryAction(proc => MainWindow = Automation.ElementFromHandle(proc.MainWindowHandle), process);

        private Process? GetProcess()
        {
            var result = Process.GetProcesses().ToList()
                .FindAll(process => process.MainWindowHandle != IntPtr.Zero)
                .FindAll(process => _name == process.ProcessName)
                .FindAll(process => process.MainWindowTitle.StartsWith(_title));
            return result.Count == 0 ? null : result[0];
        }

        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        protected const TreeScope FindScope = TreeScope.TreeScope_Descendants | TreeScope.TreeScope_Element;

        protected IUIAutomationElement? GetElement(
            IUIAutomationCondition condition,
            Predicate<IUIAutomationElement> predicate) =>
            TryFunc(
                array => array == null || array.Length <= 0
                    ? null
                    : Enumerable.Range(0, array.Length)
                        .Select(array.GetElement)
                        .FirstOrDefault(result => predicate(result)),
                MainWindow?.FindAll(FindScope, condition), null);

        protected static IUIAutomationLegacyIAccessiblePattern? GetLegacyPattern(IUIAutomationElement? element) =>
            TryFunc(source => (IUIAutomationLegacyIAccessiblePattern)
                source.GetCurrentPattern(UIA_PatternIds.UIA_LegacyIAccessiblePatternId), element, null);

        protected static string? GetLegacyPatternValue(IUIAutomationElement? element) =>
            TryFunc(pattern => pattern?.CurrentValue, GetLegacyPattern(element), null);

        protected static bool SetLegacyPatternValue(IUIAutomationElement? element, string value) =>
            TryAction(pattern => pattern.SetValue(value), GetLegacyPattern(element));

        protected static string? GetLegacyPatternName(IUIAutomationElement? element) =>
            TryFunc(pattern => pattern.CurrentName, GetLegacyPattern(element), null);

        protected static bool SilentClickButton(IUIAutomationElement? element) =>
            TryAction(pattern => pattern.DoDefaultAction(), GetLegacyPattern(element));

        protected static IUIAutomationCondition ClassNameCondition(string name) =>
            Automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, name);

        // TextEditor Element
        protected virtual IUIAutomationElement? GetEditorElement() =>
            GetElement(ClassNameCondition("TextBox"), element => element.CurrentLocalizedControlType == "編集");

        // Button Element
        protected virtual IUIAutomationElement? GetTextButtonElement(string name) =>
            TryFunc(
                element => Automation.ControlViewWalker.GetParentElement(element),
                GetElement(ClassNameCondition("TextBlock"), element => element.CurrentName == name), null);

        // StatusBar Element
        protected virtual IUIAutomationElement? GetStatusElement() =>
            TryFunc(
                element => Automation.ControlViewWalker.GetFirstChildElement(element),
                GetElement(
                    ClassNameCondition("StatusBarItem"),
                    element => element.CurrentLocalizedControlType == "テキスト"), null);

        // Save dialogs
        protected virtual IUIAutomationElement? GetSaveOptionWindow() =>
            GetElement(ClassNameCondition("Window"), element => element.CurrentName == "音声保存");

        protected virtual IUIAutomationElement? GetSaveOptionOkButton() =>
            GetElement(ClassNameCondition("Button"), element => element.CurrentName == "OK");

        protected virtual IUIAutomationElement? GetSaveDialogEdit() =>
            GetElement(ClassNameCondition("Edit"), element => element.CurrentName == "ファイル名:");

        protected virtual IUIAutomationElement? GetSaveDialogSaveButton() =>
            GetElement(ClassNameCondition("Button"), element => element.CurrentName == "保存(S)");

        protected virtual IUIAutomationElement? GetSaveDialogCancelButton() =>
            GetElement(ClassNameCondition("Button"), element => element.CurrentName == "キャンセル");

        protected bool ClickSaveOptionOkButton() => SilentClickButton(GetSaveOptionOkButton());
        protected bool SetSaveDialogEditText(string path) => SetLegacyPatternValue(GetSaveDialogEdit(), path);
        protected bool ClickSaveDialogSaveButton() => SilentClickButton(GetSaveDialogSaveButton());
        protected bool ClickSaveDialogCancelButton() => SilentClickButton(GetSaveDialogCancelButton());

        // Common dialog
        protected virtual IUIAutomationElement? GetDialog(string name) =>
            GetElement(ClassNameCondition("#32770"), element => element.CurrentName == name);

        protected virtual IUIAutomationElement? GetDialogYesButton() =>
            GetElement(ClassNameCondition("Button"), element => element.CurrentName == "はい(Y)");

        protected virtual IUIAutomationElement? GetDialogNoButton() =>
            GetElement(ClassNameCondition("Button"), element => element.CurrentName == "いいえ(N)");

        protected bool ClickDialogYesButton() => SilentClickButton(GetDialogYesButton());
        protected bool ClickDialogNoButton() => SilentClickButton(GetDialogNoButton());
        
        // Public methods        
        
        // Attach process
        public bool Attach() => Attach(GetProcess());
        public bool AttachWait(int tick = DefaultTick) => WaitFor(() => !Attach(), tick);

        // Editor Text
        public virtual string? GetEditorText() => GetLegacyPatternValue(GetEditorElement());
        public virtual bool SetEditorText(string message) => SetLegacyPatternValue(GetEditorElement(), message);

        // Buttons
        public virtual bool ClickStart() => SilentClickButton(GetTextButtonElement("再生"));
        public virtual bool ClickStop() => SilentClickButton(GetTextButtonElement("停止"));
        public virtual bool ClickHome() => SilentClickButton(GetTextButtonElement("先頭"));
        public virtual bool ClickEnd() => SilentClickButton(GetTextButtonElement("末尾"));
        public virtual bool ClickSave() => SilentClickButton(GetTextButtonElement("音声保存"));
        public virtual bool ClickDuration() => SilentClickButton(GetTextButtonElement("再生時間"));

        // Status Text
        public virtual string? GetStatusText() => GetLegacyPatternName(GetStatusElement());

        // Statuses
        public bool IsAvailable() => TryAction(element => element.GetRuntimeId(), MainWindow);
        public virtual bool IsPending() => GetStatusText() == "テキストの読み上げの準備をしています。";
        public virtual bool IsTalking() => GetStatusText() == "テキストを読み上げています。";
        public virtual bool IsSuspended() => GetStatusText() == "テキストの読み上げは一時停止中です。";
        public virtual bool IsCompleted() => GetStatusText() == "テキストの読み上げは完了しました。";
        public virtual bool IsStopped() => GetStatusText() == "テキストの読み上げは停止されました。";
        
        // Talk
        public bool Talk(string message) =>
            !(IsPending() || IsTalking()) && !(IsSuspended() && !ClickStop()) && SetEditorText(message) && ClickStart();

        public bool TalkWait(string message, int tick = DefaultTick) =>
            Talk(message) && WaitFor(() => IsPending() || IsTalking(), tick);

        // Save
        public virtual bool Save(string path, bool overwrite = false, int tick = DefaultTick)
        {
            if (!ClickSave()) return false;
            if (GetSaveOptionWindow() != null && !ClickSaveOptionOkButton()) return false;
            if (!(SetSaveDialogEditText(path) && ClickSaveDialogSaveButton())) return false;
            if (GetDialog("名前を付けて保存") == null)
                return WaitFor(() => GetSaveOptionWindow() != null, tick);
            if (overwrite)
                return ClickDialogYesButton() && WaitFor(() => GetSaveOptionWindow() != null, tick);

            ClickDialogNoButton();
            ClickSaveDialogCancelButton();
            return false;
        }
        
        public bool Save(string message, string path, bool overwrite = false) =>
            SetEditorText(message) && Save(path, overwrite);
    }
}