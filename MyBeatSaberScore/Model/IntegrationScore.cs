using MyBeatSaberScore.APIs;
using MyBeatSaberScore.BeatMap;
using System;

namespace MyBeatSaberScore.Model
{
    internal class IntegrationScore
    {
        /// <summary>
        /// マップ情報
        /// </summary>
        public BeatMapData Map { get; }

        /// <summary>
        /// BeatLeaderのスコア
        /// </summary>
        public BeatLeaderScore BeatLeader { get; }

        /// <summary>
        /// ScoreSaberのスコア
        /// </summary>
        public ScoreSaberScore ScoreSaber { get; }

        /// <summary>
        /// マップ情報。
        /// </summary>
        public string SongFullName { get; set; }

        /// <summary>
        /// マップ情報。Keyを10進数になおしたやつ。
        /// </summary>
        public NumOfKey NumOfKey { get; }

        /// <summary>
        /// マップ情報。ローカルに保存したカバー画像のパス。
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// マップ情報。カバー画像のURL。
        /// </summary>
        public string CoverUrl { get; }

        /// <summary>
        /// Checked Onlyが有効なときに表示するかどうか
        /// </summary>
        public bool IsShowChekdOnlySelected { get; set; }

        public IntegrationScore(BeatMapData map)
        {
            Map = map;
            ScoreSaber = new();
            BeatLeader = new();
            SongFullName = $"{map.SongName} {map.SongSubName} / {map.SongAuthorName} [ {map.MapperName} ]";
            NumOfKey = new NumOfKey() { Key = (map.Key.Length > 0) ? Convert.ToInt64(map.Key, 16) : 0, IsDeleted = map.Deleted };
            Cover = BeatMapCover.GetCoverLocalPath(map.Hash);
            CoverUrl = $"https://eu.cdn.beatsaver.com/{map.Hash}.jpg";
            IsShowChekdOnlySelected = true;
        }

        public IntegrationScore(ScoreSaber.PlayerScore score, ScoreSaberPlayHistory.SpecificMapPlayHistory results)
        {
            string hash = score.leaderboard.songHash.ToLower();
            var map = BeatMapDic.Get(hash, score.leaderboard.difficulty.mapMode, score.leaderboard.difficulty.mapDifficulty) ?? new()
            {
                Key = "",
                Hash = hash,
                SongName = score.leaderboard.songName,
                SongSubName = score.leaderboard.songSubName,
                SongAuthorName = score.leaderboard.songAuthorName,
                MapperName = score.leaderboard.levelAuthorName,
                UploadedTime = new(),
                Bpm = 0,
                Duration = 0,
                MapDifficulty = score.leaderboard.difficulty.mapDifficulty,
                MapMode = score.leaderboard.difficulty.mapMode,
                Bombs = 0,
                Notes = 0,
                Walls = 0,
                Njs = 0,
                Nps = 0,
                MaxScore = score.leaderboard.maxScore,
                ScoreSaber = new()
                {
                    Ranked = false,
                    RankedTime = null,
                    Star = 0
                },
                BeatLeader = new()
                {
                    Ranked = false,
                    RankedTime = null,
                    Star = 0
                },
                Deleted = true,
            };

            Map = map;
            ScoreSaber = new(map, score, results);
            BeatLeader = new();
            SongFullName = $"{map.SongName} {map.SongSubName} / {map.SongAuthorName} [ {map.MapperName} ]";
            NumOfKey = new NumOfKey() { Key = (map.Key.Length > 0) ? Convert.ToInt64(map.Key, 16) : 0, IsDeleted = map.Deleted };
            Cover = BeatMapCover.GetCoverLocalPath(map.Hash);
            CoverUrl = $"https://eu.cdn.beatsaver.com/{map.Hash}.jpg";
            IsShowChekdOnlySelected = true;
        }

        public IntegrationScore(BeatLeader.ScoreResponseWithMyScore score, BeatLeaderPlayHistory.SpecificMapPlayHistory results)
        {
            string hash = score.leaderboard.song.hash.ToLower();
            var map = BeatMapDic.Get(hash, score.leaderboard.difficulty.mapMode, score.leaderboard.difficulty.mapDifficulty) ?? new()
            {
                Key = score.leaderboardId.Replace("x", "")[0..^2],
                Hash = hash,
                SongName = score.leaderboard.song.name,
                SongSubName = score.leaderboard.song.subName,
                SongAuthorName = score.leaderboard.song.author,
                MapperName = score.leaderboard.song.mapper,
                UploadedTime = new(),
                Bpm = 0,
                Duration = 0,
                MapDifficulty = score.leaderboard.difficulty.mapDifficulty,
                MapMode = score.leaderboard.difficulty.mapMode,
                Bombs = score.leaderboard.difficulty.bombs,
                Notes = score.leaderboard.difficulty.notes,
                Walls = score.leaderboard.difficulty.walls,
                Njs = score.leaderboard.difficulty.njs,
                Nps = score.leaderboard.difficulty.nps,
                MaxScore = score.leaderboard.difficulty.maxScore,
                ScoreSaber = new()
                {
                    Ranked = false,
                    RankedTime = null,
                    Star = 0
                },
                BeatLeader = new()
                {
                    Ranked = false,
                    RankedTime = null,
                    Star = 0
                },
                Deleted = true,
            };

            Map = map;
            ScoreSaber = new();
            BeatLeader = new(map, score, results);
            SongFullName = $"{map.SongName} {map.SongSubName} / {map.SongAuthorName} [ {map.MapperName} ]";
            NumOfKey = new NumOfKey() { Key = (map.Key.Length > 0) ? Convert.ToInt64(map.Key, 16) : 0, IsDeleted = map.Deleted };
            Cover = BeatMapCover.GetCoverLocalPath(map.Hash);
            CoverUrl = $"https://eu.cdn.beatsaver.com/{map.Hash}.jpg";
            IsShowChekdOnlySelected = true;
        }
    }

    internal class ScoreBase
    {
        /// <summary>
        /// スコア更新日
        /// </summary>
        public DateTime? TimeSet { get; protected set; }

        /// <summary>
        /// スコア
        /// </summary>
        public long ModifiedScore { get; protected set; }

        /// <summary>
        /// 精度=100*スコア/最大スコア
        /// </summary>
        public double Acc { get; protected set; }

        /// <summary>
        /// 最新の精度 - 前回の精度
        /// </summary>
        public double AccDifference { get; protected set; }

        /// <summary>
        /// 初スコアかどうか
        /// </summary>
        public bool IsFirstScore { get; protected set; }

        /// <summary>
        /// 初クリアかどうか
        /// </summary>
        public bool IsFirstClear { get; protected set; }

        /// <summary>
        /// 0:スコア回数>1＆初クリア=false
        /// 1:スコア回数=1＆初クリア=false
        /// 2:スコア回数>1＆初クリア=true
        /// 3:スコア回数=1＆初クリア=true
        /// </summary>
        public int ClearStatus { get; protected set; }

        /// <summary>
        /// PP。ランク譜面以外は0。
        /// </summary>
        public double PP { get; protected set; }

        /// <summary>
        /// [[[modifire],[modifirs],...]
        /// 略称を把握済みのmodifireは以下
        /// BE:Battery Energy(4 Lives).
        /// DA:Disappearing Arrows.
        /// FS:Faster Song.
        /// GN:Ghost Notes.
        /// IF:Insta Fail(1 Life).
        /// NA:No Arrows.
        /// NB:No Bombs.
        /// NF:No Fail.
        /// NO:No Obstacles(No Walls).
        /// PM:Pro Mode.
        /// SF:Super Fast Song.
        /// SS:Slower Song.
        /// 略称を未確認のmodifireは以下
        /// Small Notes, Zen Mode
        /// </summary>
        public string Modifiers { get; protected set; } = string.Empty;

        /// <summary>
        /// スコア更新回数
        /// </summary>
        public long ScoreCount { get; protected set; }

        /// <summary>
        /// ミス＋バッドカットの数
        /// </summary>
        public long MissPlusBad { get; protected set; }

        /// <summary>
        /// ミスカットの数
        /// </summary>
        public long Miss { get; protected set; }

        /// <summary>
        /// バッドカットの数
        /// </summary>
        public long Bad { get; protected set; }

        /// <summary>
        /// フルコン
        /// </summary>
        public string FullCombo { get; protected set; } = string.Empty;

        /// <summary>
        /// 順位
        /// </summary>
        public long WorldRank { get; protected set; }
    }

    internal class ScoreSaberScore : ScoreBase
    {
        public ScoreSaber.PlayerScore? Reference { get; private set; }

        public ScoreSaberScore()
        {
            Reference = null;
            TimeSet = null;
            ModifiedScore = -1;
            Acc = 0;
            AccDifference = 0;
            IsFirstScore = false;
            IsFirstClear = false;
            ClearStatus = (IsFirstScore ? 1 : 0) + (IsFirstClear ? 2 : 0);
            PP = 0;
            Modifiers = "";
            ScoreCount = 0;
            MissPlusBad = 0;
            Miss = 0;
            Bad = 0;
            FullCombo = "";
            WorldRank = 0;
        }

        public ScoreSaberScore(BeatMapData map, ScoreSaber.PlayerScore score, ScoreSaberPlayHistory.SpecificMapPlayHistory results)
        {
            Set(map, score, results);
        }

        public void Set(BeatMapData map, ScoreSaber.PlayerScore score, ScoreSaberPlayHistory.SpecificMapPlayHistory results)
        {
            Reference = score;
            TimeSet = (score.score.modifiedScore >= 0) ? score.score.timeSet : null;
            ModifiedScore = score.score.modifiedScore;
            Acc = (map.MaxScore > 0 && score.score.modifiedScore > 0) ? (double)score.score.modifiedScore * 100 / map.MaxScore : 0;
            AccDifference = (map.MaxScore > 0 && results.LatestChange() > 0) ? (double)results.LatestChange() * 100 / map.MaxScore : 0;
            IsFirstScore = results.Count == 1;
            IsFirstClear = results.IsFirstClear();
            ClearStatus = (IsFirstScore ? 1 : 0) + (IsFirstClear ? 2 : 0);
            PP = score.leaderboard.ranked ? score.score.pp : 0;
            Modifiers = score.score.modifiers;
            ScoreCount = results.Count;
            MissPlusBad = score.score.badCuts + score.score.missedNotes;
            Miss = score.score.missedNotes;
            Bad = score.score.badCuts;
            FullCombo = (score.score.fullCombo) ? "FC" : "";
            WorldRank = score.score.rank;
        }
    }

    internal class BeatLeaderScore : ScoreBase
    {
        public BeatLeader.ScoreResponseWithMyScore? Reference { get; private set; }

        public BeatLeaderScore()
        {
            Reference = null;
            TimeSet = null;
            ModifiedScore = -1;
            Acc = 0;
            AccDifference = 0;
            IsFirstScore = false;
            IsFirstClear = false;
            ClearStatus = (IsFirstScore ? 1 : 0) + (IsFirstClear ? 2 : 0);
            PP = 0;
            Modifiers = "";
            ScoreCount = 0;
            MissPlusBad = 0;
            Miss = 0;
            Bad = 0;
            FullCombo = "";
            WorldRank = 0;
        }

        public BeatLeaderScore(BeatMapData map, BeatLeader.ScoreResponseWithMyScore score, BeatLeaderPlayHistory.SpecificMapPlayHistory results)
        {
            Set(map, score, results);
        }

        public void Set(BeatMapData map, BeatLeader.ScoreResponseWithMyScore score, BeatLeaderPlayHistory.SpecificMapPlayHistory results)
        {
            Reference = score;
            TimeSet = (score.modifiedScore >= 0) ? new DateTime(1970, 1, 1).AddSeconds(double.Parse(score.timeset)) : null;
            ModifiedScore = score.modifiedScore;
            Acc = (map.MaxScore > 0 && score.modifiedScore > 0) ? (double)score.modifiedScore * 100 / map.MaxScore : 0;
            AccDifference = (map.MaxScore > 0 && results.LatestChange() > 0) ? (double)results.LatestChange() * 100 / map.MaxScore : 0;
            IsFirstScore = results.Count == 1;
            IsFirstClear = results.IsFirstClear();
            ClearStatus = (IsFirstScore ? 1 : 0) + (IsFirstClear ? 2 : 0);
            PP = score.pp;
            Modifiers = score.modifiers;
            ScoreCount = results.Count;
            MissPlusBad = score.badCuts + score.missedNotes;
            Miss = score.missedNotes;
            Bad = score.badCuts;
            FullCombo = (score.fullCombo) ? "FC" : "";
            WorldRank = score.rank;
        }
    }
}
