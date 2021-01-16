﻿# AutoAITalk

VOICEROID2系をとにかく喋らせるライブラリです。  
内部的にはCOM版のUIAutomationを使ってます。  
運が良ければA.I.VOICEに対応します。(後述)  
VOICEROID+系は持ってないのでなんとも、実況作るの下手なのであまり買ってもね…。

## どういったものか

とにかくバックグラウンドでボイロに喋ったり音声保存してもらえるように作りました。  
作りましたが、それ以外の機能は殆どついてないし、そこまで凝った実装はしてないです。

まあ、この程度の機能でもアイディア次第で既存のソフトと似たようなの作れると思います。  
バックグラウンドで保存もできるので「ファイルぶん投げてIoT機器に喋らせる」とかも出来そうです。  
…えーっと、そのあたりはボイロ側のライセンスに違反しない範囲でお願いします。

基本的に例外は(ちょっとしたことでも吐くので)握りつぶしてありますし、ダメやったらnullとかfalseとかを返します。  
このあたりはもう少し改善の余地があると思う。

あと簡易的に`TalkWait()`だったり便利系のメソッドとか追加してあります、実装が結構雑ですけど。  
(あ、話者切り替えはプロンプト機能使ってください！)

現在とりあえず作った感じなので、一部メソッドは返す値が変わるかもです、そのあたり注意です。  
(例えばBoolだとエラーだったとしても原因分からないので、Enumとかに置き換えるかもしれません)

## あと何か言うこと

C#とかめっちゃ久しぶりに書いたから書き方が下手かもしれない、許して。  
新しい機能とか見たらどうしても使いたくなってしまうし、前衛芸術とか言われそうなコードを書いちゃってます。

ちなみに抽象クラスにほぼ全部実装書いてるのはA.I.VOICEとVoiceroid2のどこを共通化出来るか知らないからです。  
多分あとでリファクタリングします。

前提が合ってたら現状の`AiVoice.cs`でA.I.VOICEまで動いてくれます。  
プロセス名は当てずっぽうなので、マジでこのまま動いたら奇跡ですけどね。

## そういえば

A.I.VOICEで公式がAPI出してくれたらこれ作る意味はあまり無いですね。  
PS. 公式の配信で「後々に公開予定」とのこと

最近Recotte Studioが発売されたのでVOICEROID2のAPIが公開されるならそろそろなんですけどね。  
(このあたり結構関連性があった気がする)

## サンプル

これいる？

```c#

var voiceroid = new Voiceroid2();
if (voiceroid.IsAvailable())
    voiceroid.TalkWait("起動完了です！");

```
