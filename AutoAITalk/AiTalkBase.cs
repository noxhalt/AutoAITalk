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
        private readonly string _name;
        private readonly string _title;
        protected readonly CUIAutomation Automation = new();
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

        public bool Attach() => Attach(GetProcess());

        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        private const TreeScope FindScope = TreeScope.TreeScope_Descendants | TreeScope.TreeScope_Element;

        private IUIAutomationElement? GetElement(
            IUIAutomationCondition condition,
            Predicate<IUIAutomationElement> predicate) =>
            TryFunc(
                array => array == null || array.Length <= 0
                    ? null
                    : Enumerable.Range(0, array.Length)
                        .Select(array.GetElement)
                        .FirstOrDefault(result => predicate(result)),
                MainWindow?.FindAll(FindScope, condition), null);

        private static IUIAutomationLegacyIAccessiblePattern? GetLegacyPattern(IUIAutomationElement? element) =>
            TryFunc(source => (IUIAutomationLegacyIAccessiblePattern)
                source.GetCurrentPattern(UIA_PatternIds.UIA_LegacyIAccessiblePatternId), element, null);

        private static string? GetLegacyPatternValue(IUIAutomationElement? element) =>
            TryFunc(pattern => pattern?.CurrentValue, GetLegacyPattern(element), null);

        private static bool SetLegacyPatternValue(IUIAutomationElement? element, string value) =>
            TryAction(pattern => pattern.SetValue(value), GetLegacyPattern(element));

        private static string? GetLegacyPatternName(IUIAutomationElement? element) =>
            TryFunc(pattern => pattern.CurrentName, GetLegacyPattern(element), null);

        private static bool SilentClickButton(IUIAutomationElement? element) =>
            TryAction(pattern => pattern.DoDefaultAction(), GetLegacyPattern(element));

        // TextEditor Element
        protected virtual IUIAutomationElement? GetEditorElement() => GetElement(
            Automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, "TextBox"),
            element => element.CurrentLocalizedControlType == "編集");

        // Button Element
        protected virtual IUIAutomationElement? GetButtonElement(string name) =>
            TryFunc(
                element => Automation.ControlViewWalker.GetParentElement(element),
                GetElement(
                    Automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, "TextBlock"),
                    element => element.CurrentName == name), null);

        // StatusBar Element
        protected virtual IUIAutomationElement? GetStatusElement() =>
            TryFunc(
                element => Automation.ControlViewWalker.GetFirstChildElement(element),
                GetElement(
                    Automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, "StatusBarItem"),
                    element => element.CurrentLocalizedControlType == "テキスト"), null);

        // Text Editor
        public virtual string? GetEditorText() => GetLegacyPatternValue(GetEditorElement());
        public virtual bool SetEditorText(string message) => SetLegacyPatternValue(GetEditorElement(), message);

        // Buttons
        public virtual bool ClickStart() => SilentClickButton(GetButtonElement("再生"));
        public virtual bool ClickStop() => SilentClickButton(GetButtonElement("停止"));
        public virtual bool ClickHome() => SilentClickButton(GetButtonElement("先頭"));
        public virtual bool ClickEnd() => SilentClickButton(GetButtonElement("末尾"));
        public virtual bool ClickSave() => SilentClickButton(GetButtonElement("音声保存"));
        public virtual bool ClickDuration() => SilentClickButton(GetButtonElement("再生時間"));

        // Status bar
        public virtual string? GetStatusText() => GetLegacyPatternName(GetStatusElement());

        // Statuses
        public bool IsAvailable() => TryAction(element => element.GetRuntimeId(), MainWindow);
        public virtual bool IsPending() => GetStatusText() == "テキストの読み上げの準備をしています。";
        public virtual bool IsTalking() => GetStatusText() == "テキストを読み上げています。";
        public virtual bool IsSuspended() => GetStatusText() == "テキストの読み上げは一時停止中です。";
        public virtual bool IsCompleted() => GetStatusText() == "テキストの読み上げは完了しました。";
        public virtual bool IsStopped() => GetStatusText() == "テキストの読み上げは停止されました。";

        // Talk sugar method
        public bool Talk(string message) =>
            !(IsPending() || IsTalking()) &&
            !(IsSuspended() && !ClickStop()) &&
            SetEditorText(message) && ClickStart();

        public bool TalkWait(string message)
        {
            var status = Talk(message);
            while (IsPending() || IsTalking()) Thread.Sleep(100);
            return status;
        }

        public void Testing()
        {
            if (ClickSave())
            {
                
            }
        }
    }
}