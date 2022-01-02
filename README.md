なんかBeat Saberのスコアを表示するやつ。

https://github.com/tkns3/MyBeatSaberScore/releases/download/v0.0.1/MyBeatSaberScore-v0.0.1.zip

ダウンロードしたzipファイルを展開して`MyBeatSaberScore.exe`を実行してください。

exe実行時に.NET Runtimeが足りないと言われたら説明に従いdesktop appsのRuntimeをインストールしてください。

起動したら左上のテキストボックスにスコアセイバーのプロファイルIDを入力して最新データを取得をクリックします。スコアセイバーの個人ページURLが`https://scoresaber.com/u/76561198003035723`であれば`76561198003035723`の部分がプロファイルIDです。

初回は全プレイ履歴を取得するためダウンロードに時間がかかるので気長に待ちます。

次回以降は差分データのみを取得するため比較的短い時間で終わります。

取得したデータは`MyBeatSaberScore.exe`と同じ階層に`data`フォルダに保存されます。

アンインストールはzipを展開したフォルダを丸ごと削除してください。
