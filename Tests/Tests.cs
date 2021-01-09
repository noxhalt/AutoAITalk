using System;
using System.Threading;
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
            // これが操作用インスタンス
            var akane = new Voiceroid2();

            // 喋ってもらうプリセット
            const string akaneId = "茜2";

            // 接続が成功したか
            if (!akane.IsAvailable()) return;
            Console.WriteLine("接続に成功");
            
            // TEST
            akane.TalkWait($"{akaneId}＞てすとやでー");
            akane.Testing();
        }
        
        [Test]
        public void Voiceroid2()
        {
            // これが操作用インスタンス
            var akane = new Voiceroid2();

            // 喋ってもらうプリセット
            const string akaneId = "茜2";

            // 接続が成功したか
            if (akane.IsAvailable())
            {
                // 実験用
                Console.WriteLine(akane.GetStatusText());
                // とりあえずなんか喋ってもらう
                Console.WriteLine($"{akaneId}: 準備OKやで！");
                akane.SetEditorText($"{akaneId}＞準備出来たでー！");
                akane.ClickStart();
            }
            else
            {
                // 接続を待機
                Console.WriteLine($"{akaneId}: むにゃむにゃ…");
                while (!akane.Attach()) Thread.Sleep(500);
                Console.WriteLine($"{akaneId}: おはよう！");
            }

            // 話し終わるのを待つ
            while (akane.IsPending() || akane.IsTalking()) Thread.Sleep(500);

            // テキストの設定と取得をテスト
            var message = $"{akaneId}＞テキストの設定と取得のテストやで！";

            // 喋ってもらい、成功かどうかを取得
            if (akane.Talk(message))
            {
                Console.WriteLine(akane.GetEditorText());
                Console.WriteLine($"{akaneId}: 喋れたで！");
                Console.WriteLine(akane.GetStatusText());
            }
            else
            {
                Console.WriteLine($"{akaneId}: 喋れんかったで…");
            }
            
            // 話し終わるのを待つ
            while (akane.IsPending() || akane.IsTalking()) Thread.Sleep(500);
        }
    }
}