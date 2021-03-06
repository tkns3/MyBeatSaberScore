# なにこれ？

Beat Saberを遊んでScore Saberに送信したスコアを表示するWindowsアプリです。

柔軟なフィルターとソートによって目的の譜面のスコアを簡単に確認することができます。

未プレイのランク譜面も表示できるのでランク譜面の消化具合を確認することもできます。

<img src="image/window_sample.png" alt="attach:window_sample" title="attach:window_sample">

# インストール

[latest](https://github.com/tkns3/MyBeatSaberScore/releases/latest)から`MyBeatSaberScore-vX.X.X.zip`をダウンロードして任意のフォルダに展開してください。

# アンインストール

インストールしたフォルダを丸ごと削除してください。

# アップデート

[latest](https://github.com/tkns3/MyBeatSaberScore/releases/latest)から`MyBeatSaberScore-vX.X.X.zip`をダウンロードして展開したフォルダに含まれている全ファイルをインストールフォルダに上書きしてください。

# 使い方

## .Netランタイムのインストール（一度だけ）

.NET 6.0のSDKまたはランタイムをインストールしていない場合は[.NET 6.0 ランタイムのダウンロード](https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0/runtime)からお使いのPC環境にあわせたランタイムをダウンロードしてインストールしてください。

例えばお使いのPCがWindows10の64bit版であれば「デスクトップ アプリを実行する」の項にある「X64のダウンロード」を選択します。

間違えてコンソールアプリ用のランタイムをダウンロードしないように気を付けてください。

<img src="image/runtime.png" alt="attach:runtime" title="attach:runtime">

## 起動

`MyBeatSaberScore.exe`を実行してください。

## データ取得

起動したら左上のテキストボックスにスコアセイバーのプロファイルIDを入力し「データ取得」ボタンをクリックします。
スコアセイバーの個人ページURLが`https://scoresaber.com/u/76561198003035723`であれば`76561198003035723`の部分がプロファイルIDです。

初回は全プレイ履歴やカバー画像を取得するためダウンロードに時間がかかります。気長に待ってください。

次回以降は差分データのみを取得するため比較的短い時間で終わります。

取得したデータは`MyBeatSaberScore.exe`と同じ階層の`data`フォルダに保存しています。

## ユーザー切り替え

ユーザータブで「お気に入りユーザーの管理」と「表示ユーザーの切り替え」ができます。

<img src="image/usage_userselect.png" alt="attach:usage_userselect" title="attach:usage_userselect">

## フィルターとソート

メインページの「検索」「譜面の種類」「プレイ結果」で表示対象をフィルターすることができます。

メインページの列ヘッダをクリックするとソート順を変更することができます。

<img src="image/usage_filter.png" alt="attach:usage_filter" title="attach:usage_filter">

フィルタータブでは色々なフィルターを指定することができます。

<img src="image/usage_filter2.png" alt="attach:usage_filter" title="attach:usage_filter">

## Copy BSR

Twitchのアイコンをクリックすると「!bsr key」をクリップボードにコピーします。

## BeatSaver、ScoreSaberをひらく

BeatSaver、ScoreSaberのアイコンをクリックすると譜面のページをブラウザでひらきます。

## プレイリスト作成

「プレイリスト作成」ボタンをクリックすると表示している譜面からプレイリストを作成します。

保存ダイアログが表示されるので任意のフォルダに任意の名前で保存してください。

# Ｑ＆Ａ

## 起動しません

.NET 6.0のSDKまたはランタイムがインストールできていない可能性があります。

「使い方」の「.Netランタイムのインストール（一度だけ）」を参考に.NET 6.0のランタイムをインストールしてください。

## アップデートしたら未プレイのランク譜面が表示されなくなった

「データ取得」を行うと表示されます。

## bsr、精度が表示されない譜面があります

以下のいずれかの条件があてはまる譜面のbsr、精度は表示しません。

- リリースされた直後。
- リパブリッシュされている。
- BeatSaverから削除されている。

リリースされた直後の譜面は数時間たってから「データ取得」を行うと表示されるようになります。
これは「データ取得」で譜面情報を取得しますが取得先のデータがBeatSaverに同期するまで数時間かかります。

リパブリッシュされた譜面のbsr、精度が表示されることはありません。
BeatSaverアイコンをクリックするとブラウザでBeatSaverを開きリパブリッシュ後の譜面情報を確認することができます。

## プレイ結果のFailureの条件は？

Modifiersに「NF」(No Fail)または「SS」(Slow Song)がついている譜面が対象です。

`data/config.json`を直接編集することで条件を変更することが可能です。

例えば「NB」(No Bomb)を条件に追加したい場合は次のように指定します。


    {
      "scoreSaberProfileId": "76561198003035723",
      "failures": [
        "NF",
        "SS",
        "NB"
      ]
    }

## 本ツールとScoreSaberでppの値が違う

`data/users/{プロファイルID}/scores.json`を削除して「データ取得」を行うとScoreSaberと同じppの値が表示されるようになります。

ScoreSaberのpp評価方法は変更されることがあります。

ただし本ツールはその変更を自動的に検知することができないため手動での対応をお願いします。

## 本ツールとScoreSaberで星の値が違う

数時間たってから「データ取得」を行うとScoreSaberと同じ星の値が表示されるようになります。

ScoreSaberの星評価方法は変更されることがあります。

「データ取得」で譜面情報を取得しますが取得先のデータがScoreSaberに同期するまで数時間かかります。

