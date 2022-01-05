# なにこれ？

Score Saberのスコアを表示するWindowsアプリです。

未プレイのランク譜面も表示するのでランク譜面の消化具合を確認するときに便利です。

# インストール

[releases](https://github.com/tkns3/MyBeatSaberScore/releases)から`MyBeatSaberScore-vX.X.X.zip`をダウンロードして任意のフォルダに展開してください。

# アンインストール

インストールしたフォルダを丸ごと削除してください。

# アップデート

[releases](https://github.com/tkns3/MyBeatSaberScore/releases)から最新の`MyBeatSaberScore-vX.X.X.zip`をダウンロードしてzipに含まれている全ファイルをインストールフォルダに上書きしてください。

# 使い方

`MyBeatSaberScore.exe`を実行してください。

`MyBeatSaberScore.exe`を実行した時に.NET Runtimeが足りないと言われた場合は説明に従いdesktop appsのRuntimeをインストールしてください。

起動したら左上のテキストボックスにスコアセイバーのプロファイルIDを入力し「最新データを取得」のボタンをクリックします。スコアセイバーの個人ページURLが`https://scoresaber.com/u/76561198003035723`であれば`76561198003035723`の部分がプロファイルIDです。

初回は全プレイ履歴やカバー画像を取得するためダウンロードに時間がかかります。気長に待ってください。

次回以降は差分データのみを取得するため比較的短い時間で終わります。

取得したデータは`MyBeatSaberScore.exe`と同じ階層の`data`フォルダに保存しています。

# Ｑ＆Ａ

## アップデートしたら未プレイのランク譜面が表示されなくなった

「最新データを取得」を行うと表示されます。

