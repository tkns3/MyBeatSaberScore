# MyBeatSaberScore

MyBeatSaberScoreはBeat Saberを遊んでScore SaberやBeat Leaderに送信したスコアを表示するWindowsアプリです。

柔軟なフィルターとソートによって目的の譜面のスコアを簡単に確認することができます。

未プレイのランク譜面も表示できるのでランク譜面の消化具合を確認することもできます。

<img src="image/window_sample.png" alt="attach:window_sample" title="attach:window_sample">

# インストール

[latest](https://github.com/tkns3/MyBeatSaberScore/releases/latest)から`MyBeatSaberScore.exe`をダウンロードします。

任意の場所に`MyBeatSaberScore`フォルダを作成してその中にダウンロードした`MyBeatSaberScore.exe`を配置してください。

<img src="image/install.png" alt="attach:install" title="attach:install">

※`MyBeatSaberScore`フォルダのフォルダ名は変更しても問題ありません。

※以降はインストールで作成したフォルダのことをインストールフォルダと記載します。

# アンインストール

インストールフォルダを削除してください。

# アップデート

## v0.6.0以上のバージョンを使用している場合

アプリ内でアップデートボタン（画像内①）をクリックすると最新の実行ファイルをダウウンロードして自動的に再起動します。

<img src="image/usage_update.png" alt="attach:usage_update" title="attach:usage_update">

現在のバージョンが最新の場合はアップデートボタンはクリックできません。

## v0.6.0未満のバージョンを使用している場合

インストールフォルダから`data`フォルダ、`log`フォルダ以外を削除します。

<img src="image/update_from_old.png" alt="attach:update_from_old" title="attach:update_from_old">

[latest](https://github.com/tkns3/MyBeatSaberScore/releases/latest)から`MyBeatSaberScore.exe`をダウンロードしてインストールフォルダに配置します。

# 使い方

## 起動

`MyBeatSaberScore.exe`を実行してください。

「WindowsによってPCが保護されました」と表示された場合、メッセージ内の「詳細情報」をクリックすると現れる「実行」ボタンをクリックしてください。

英語で「.NETが必要」の旨のメッセージが表示された場合、次の「.NETランタイムのインストール」を行ってください。

### .NETランタイムのインストール

.NET 6.0のSDKまたはランタイムをインストールしていない場合は[.NET 6.0 ランタイムのダウンロード](https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0/runtime)からお使いのPC環境にあわせたランタイムをダウンロードしてインストールしてください。

例えばお使いのPCがWindows10の64bit版であれば「デスクトップ アプリを実行する」の項にある「X64のダウンロード」を選択します。

間違えてコンソールアプリ用のランタイムをダウンロードしないように気を付けてください。

<img src="image/runtime.png" alt="attach:runtime" title="attach:runtime">

## データ取得

Score Saberの個人ページURL`https://scoresaber.com/u/76561198003035723`やBeat Leaderの個人ページURL`https://www.beatleader.xyz/u/76561198003035723`の数字部分`76561198003035723`がプロフィールIDです。

左上のテキストボックス（画像内①）にプロフィールIDを入力し「データ取得」（画像内②）ボタンをクリックします。

<img src="image/usage_get.png" alt="attach:usage_get" title="attach:usage_get">

初回は全プレイ履歴とカバー画像を取得するためダウンロードに時間がかかります。気長に待ってください。

取得したデータは`MyBeatSaberScore.exe`と同じ階層の`data`フォルダに保存しています。

「差分」（デフォルト）を指定した場合、取得済みデータより新しいデータのみを取得します。

「全部」を指定した場合、取得済みデータについてもスコアを取得しなおします。

通常は通信量が少ない「差分」がおすすめです。

次のようなケースは「全部」を試してください。

- 譜面ごとの順位の最新情報が欲しい。
- リウェイトや星変化によって譜面のppがScore SaberやBeat Leaderと一致しなくなっている。
- アンランク時代にプレイ済みの譜面がランク化されたが、ランク譜面としてプレイ済みになっていない。

## Score SaberとBeat Leaderの表示切り替え

Score Saberアイコン、Beat Leaderアイコンをクリックすることで表示を切り替えることができます。

<img src="image/usage_select_leaderboard.png" alt="attach:usage_select_leaderboard" title="attach:usage_select_leaderboard">

## ユーザー切り替え

ユーザーページで「お気に入りユーザーの管理」と「表示ユーザーの切り替え」ができます。

<img src="image/usage_userselect.png" alt="attach:usage_userselect" title="attach:usage_userselect">

プロフィールIDを入力（画像内①）してプラスボタン（画像内②）をクリックするとユーザを追加します。

ユーザごとのリロードボタン（画像内③）をクリックするとそのユーザのアイコン、画像を最新に更新します。

ユーザごとのマイナスボタン（画像内④）をクリックするとそのユーザを削除します。

ユーザ（画像内⑤）をクリックするとメインページに遷移しそのユーザのスコアを表示します。

この画面からユーザを削除しても取得済みのスコアは削除されません。

## フィルターとソート

メインページの「検索」「譜面の種類」「プレイ結果」で表示対象をフィルターすることができます。

メインページの列ヘッダをクリックするとソート順を変更することができます。

<img src="image/usage_filter.png" alt="attach:usage_filter" title="attach:usage_filter">

フィルターページでは色々なフィルターを指定することができます。

<img src="image/usage_filter2.png" alt="attach:usage_filter" title="attach:usage_filter">

## Copy BSR

Twitchのアイコンをクリックすると「!bsr key」をクリップボードにコピーします。

<img src="image/usage_CopyBSR.png" alt="attach:usage_CopyBSR" title="attach:usage_CopyBSR">

## Beat Saver、Score Saber、Beat Leaderの譜面ページをブラウザでひらく

Beat Saverアイコン（画像内①）、Score Saberアイコン（画像内②）、Beat Leadrアイコン（画像内③）をクリックすると譜面のページをブラウザでひらきます。

<img src="image/usage_JumpBSSSBL.png" alt="attach:usage_JumpBSSSBL" title="attach:usage_JumpBSSSBL">

## プレイリスト作成

「プレイリスト作成」ボタンをクリックすると表示している譜面からプレイリストを作成します。

保存ダイアログが表示されるので任意のフォルダに任意の名前で保存してください。

## CSV作成

「CSV作成」ボタンをクリックすると表示している譜面データをCSVにエクスポートします。

保存ダイアログが表示されるので任意のフォルダに任意の名前で保存してください。

出力対象選択、ヘッダー項目名変更、値フォーマット変更を行いたい場合は `data/csv_format.json` を作成してください。

`data/csv_format.json` のサンプル。

    [
      {
        "Target": "Map.Bsr",
        "Name": "Map.Bsr",
        "Format": "{0}"
      },
      {
        "Target": "Map.SongName",
        "Name": "Map.SongName",
        "Format": "\"{0}\""
      },
      {
        "Target": "Map.SongSubName",
        "Name": "Map.SongSubName",
        "Format": "\"{0}\""
      },
      {
        "Target": "Map.SongAuthorName",
        "Name": "Map.SongAuthorName",
        "Format": "\"{0}\""
      },
      {
        "Target": "Map.MapperName",
        "Name": "Map.MapperName",
        "Format": "\"{0}\""
      },
      {
        "Target": "Map.Mode",
        "Name": "Map.Mode",
        "Format": "{0}"
      },
      {
        "Target": "Map.Difficulty",
        "Name": "Map.Difficulty",
        "Format": "{0}"
      },
      {
        "Target": "Map.Duration",
        "Name": "Map.Duration",
        "Format": "{0}"
      },
      {
        "Target": "Map.Bpm ",
        "Name": "Map.Bpm ",
        "Format": "{0}"
      },
      {
        "Target": "Map.Notes",
        "Name": "Map.Notes",
        "Format": "{0}"
      },
      {
        "Target": "Map.Nps",
        "Name": "Map.Nps",
        "Format": "{0}"
      },
      {
        "Target": "Map.Njs",
        "Name": "Map.Njs",
        "Format": "{0}"
      },
      {
        "Target": "Map.Bombs ",
        "Name": "Map.Bombs ",
        "Format": "{0}"
      },
      {
        "Target": "Map.Walls",
        "Name": "Map.Walls",
        "Format": "{0}"
      },
      {
        "Target": "Map.Hash",
        "Name": "Map.Hash",
        "Format": "{0}"
      },
      {
        "Target": "Map.ScoreSaber.RankedDate",
        "Name": "Map.ScoreSaber.RankedDate",
        "Format": "{0:yyyy/MM/dd HH:mm:ss (ddd)}"
      },
      {
        "Target": "Map.ScoreSaber.Star",
        "Name": "Map.ScoreSaber.Star",
        "Format": "{0}"
      },
      {
        "Target": "Map.BeatLeader.RankedDate",
        "Name": "Map.BeatLeader.RankedDate",
        "Format": "{0:yyyy/MM/dd HH:mm:ss (ddd)}"
      },
      {
        "Target": "Map.BeatLeader.Star",
        "Name": "Map.BeatLeader.Star",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.Date",
        "Name": "ScoreSaber.Date",
        "Format": "{0:yyyy/MM/dd HH:mm:ss (ddd)}"
      },
      {
        "Target": "ScoreSaber.Score",
        "Name": "ScoreSaber.Score",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.Acc",
        "Name": "ScoreSaber.Acc",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.AccDiff",
        "Name": "ScoreSaber.AccDiff",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.MissPlusBad",
        "Name": "ScoreSaber.MissPlusBad",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.FullCombo",
        "Name": "ScoreSaber.FullCombo",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.Pp",
        "Name": "ScoreSaber.Pp",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.Modifiers",
        "Name": "ScoreSaber.Modifiers",
        "Format": "\"{0}\""
      },
      {
        "Target": "ScoreSaber.ScoreCount",
        "Name": "ScoreSaber.ScoreCount",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.Miss",
        "Name": "ScoreSaber.Miss",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.Bad",
        "Name": "ScoreSaber.Bad",
        "Format": "{0}"
      },
      {
        "Target": "ScoreSaber.WorldRank",
        "Name": "ScoreSaber.WorldRank",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.Date",
        "Name": "BeatLeader.Date",
        "Format": "{0:yyyy/MM/dd HH:mm:ss (ddd)}"
      },
      {
        "Target": "BeatLeader.Score",
        "Name": "BeatLeader.Score",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.Acc",
        "Name": "BeatLeader.Acc",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.AccDiff",
        "Name": "BeatLeader.AccDiff",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.MissPlusBad",
        "Name": "BeatLeader.MissPlusBad",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.FullCombo",
        "Name": "BeatLeader.FullCombo",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.Pp",
        "Name": "BeatLeader.Pp",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.Modifiers",
        "Name": "BeatLeader.Modifiers",
        "Format": "\"{0}\""
      },
      {
        "Target": "BeatLeader.ScoreCount",
        "Name": "BeatLeader.ScoreCount",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.Miss",
        "Name": "BeatLeader.Miss",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.Bad",
        "Name": "BeatLeader.Bad",
        "Format": "{0}"
      },
      {
        "Target": "BeatLeader.WorldRank",
        "Name": "BeatLeader.WorldRank",
        "Format": "{0}"
      }
    ]

表のどの列をエクスポートするかはTargetで指定します。指定していないTargetはCSVにエクスポートしません。

CSVヘッダーの項目名はNameで指定します。任意の文字列を指定してください。

値フォーマットはFormatで指定します。C# string.Formatの書式指定文字列を指定します。

# Ｑ＆Ａ

## 起動しません

.NET 6.0のSDKまたはランタイムがインストールできていない可能性があります。

「使い方」の「.NETランタイムのインストール」を参考に.NET 6.0のランタイムをインストールしてください。

## bsr、精度が表示されない譜面があります

Beat Leaderにスコアが残っていない譜面（Score Saberだけにスコアが残っている譜面）のうち以下のいずれかの条件があてはまる譜面のbsr、精度は表示しません。

- リリースされた直後。
- リパブリッシュされている。
- Beat Saverから削除されている。

リリースされた直後の譜面は数時間たってから「データ取得」を行うと表示されるようになります。
これは「データ取得」で譜面情報を取得しますが取得先のデータがBeatSaverに同期するまで数時間かかります。

リパブリッシュされた譜面のbsr、精度が表示されることはありません。
Beat SaverアイコンをクリックするとブラウザでBeat Saverを開きリパブリッシュ後の譜面情報を確認することができます。

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

## 本ツールとScore Saber/Beat Leaderで譜面の順位の値が違う

「差分/全部」のラジオボックスから「全部」を選択して「データ取得」を行うとScore Saber/Beat Leaderと同じ順位が表示されるようになります。

「差分」の取得は過去に取得した譜面の順位を取得しなおさないのでScore Saber/Beat Leaderとズレが発生します。

## 本ツールとScore Saber/Beat Leaderでppの値が違う

「差分/全部」のラジオボックスから「全部」を選択して「データ取得」を行うとScore Saber/Beat Leaderと同じppの値が表示されるようになります。

「差分」の取得は過去に取得した譜面のppを取得しなおさないのでScore Saber/Beat Leaderとズレが発生します。

## 本ツールとScore Saber/Beat Leaderで星の値が違う

MyBeatSaberScoreが参照している譜面データが更新されてから「データ取得」を行うとScore Saber/Beat Leaderと同じ星の値が表示されるようになります。

Score Saber/Beat Leaderの星の値は変更されることがあります。

ただしMyBeatSaberScoreが参照している譜面データはScore Saber/Beat Leaderでの変化をリアルタイムに反映していません。

Score Saberの星情報を含む譜面データは`https://github.com/andruzzzhka/BeatSaberScrappedData`から取得しています。このデータは数時間ごとに更新されます。

Beat Leaderの星情報を含む譜面データは`https://github.com/tkns3/BeatLeaderRankedData`から取得しています。このデータはJSTの20時10分頃に更新されます。

