using System;
using System.Linq;
using NUnit.Framework;
using AutoAITalk;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Dev()
        {
            // My PlayGround!!
            
            var akane = new Voiceroid2();
            const string akaneId = "茜2";
            if (!akane.IsAvailable()) return;
            
            // とりあえずひつじを100匹数えてもらおう
            Enumerable.Range(1, 100).ToList().ForEach(index => akane.TalkWait($"{akaneId}＞ひつじが{index}匹"));
        }

        [Test]
        public void Voiceroid2()
        {
            // 操作用インスタンス
            var akane = new Voiceroid2();

            // 喋ってもらうプリセット
            const string akaneId = "茜2";

            // 接続が成功したか
            if (akane.IsAvailable() || akane.AttachWait())
            {
                // とりあえずなんか喋ってもらう
                Console.WriteLine($"{akaneId}: 準備OKやで！");
                akane.TalkWait($"{akaneId}＞準備出来たでー！");
            }

            // テキストの設定と取得をテスト
            var message = $"{akaneId}＞テキストの設定と取得のテストやで！";

            // 喋ってもらい、成功かどうかを取得
            if (akane.TalkWait(message))
            {
                Console.WriteLine(akane.GetEditorText());
                Console.WriteLine($"{akaneId}: 喋れたで！");
                Console.WriteLine(akane.GetStatusText());
            }
            else
            {
                Console.WriteLine($"{akaneId}: 喋れんかったで…");
            }
        }

        [Test]
        public void AiVoice()
        {
            // 操作用インスタンス
            var akane = new AiVoice();

            // 喋ってもらうプリセット
            const string akaneId = "琴葉 茜";

            // 接続が成功したか
            if (akane.IsAvailable() || akane.AttachWait())
            {
                // とりあえずなんか喋ってもらう
                Console.WriteLine($"{akaneId}: 準備OKやで！");
                akane.TalkWait($"{akaneId}＞準備出来たでー！");
            }

            // テキストの設定と取得をテスト
            var message = $"{akaneId}＞テキストの設定と取得のテストやで！";

            // 喋ってもらい、成功かどうかを取得
            if (akane.TalkWait(message))
            {
                Console.WriteLine(akane.GetEditorText());
                Console.WriteLine($"{akaneId}: 喋れたで！");
                Console.WriteLine(akane.GetStatusText());
            }
            else
            {
                Console.WriteLine($"{akaneId}: 喋れんかったで…");
            }
        }
    }
}