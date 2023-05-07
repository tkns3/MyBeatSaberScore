using MyBeatSaberScore.APIs;
using MyBeatSaberScore.BeatMap;
using System;

namespace MyBeatSaberScore.Model
{
    public class IntegrationScore
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
            var map = BeatMapDic.Get(hash, score.leaderboard.difficulty.mapMode, score.leaderboard.difficulty.mapDifficulty) ?? CreateBeatMapData(score);

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
            var map = BeatMapDic.Get(hash, score.leaderboard.difficulty.mapMode, score.leaderboard.difficulty.mapDifficulty) ?? CreateBeatMapData(score);

            Map = map;
            ScoreSaber = new();
            BeatLeader = new(map, score, results);
            SongFullName = $"{map.SongName} {map.SongSubName} / {map.SongAuthorName} [ {map.MapperName} ]";
            NumOfKey = new NumOfKey() { Key = (map.Key.Length > 0) ? Convert.ToInt64(map.Key, 16) : 0, IsDeleted = map.Deleted };
            Cover = BeatMapCover.GetCoverLocalPath(map.Hash);
            CoverUrl = $"https://eu.cdn.beatsaver.com/{map.Hash}.jpg";
            IsShowChekdOnlySelected = true;
        }

        BeatMapData CreateBeatMapData(ScoreSaber.PlayerScore score)
        {
            return new BeatMapData()
            {
                Key = "",
                Hash = score.leaderboard.songHash.ToLower(),
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
        }

        BeatMapData CreateBeatMapData(BeatLeader.ScoreResponseWithMyScore score)
        {
            return new BeatMapData()
            {
                Key = score.leaderboardId.Replace("x", "")[0..^2],
                Hash = score.leaderboard.song.hash.ToLower(),
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
        }

        public static string GetFilterTargetMapName(object item)
        {
            return ((IntegrationScore)item).SongFullName;
        }

        public static string GetFilterTargetMapBsr(object item)
        {
            return ((IntegrationScore)item).Map.Key;
        }

        public static string GetFilterTargetMapHash(object item)
        {
            return ((IntegrationScore)item).Map.Hash;
        }

        public static bool GetFilterTargetMapTypeIsRanked(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).Map.BeatLeader.Ranked : ((IntegrationScore)item).Map.ScoreSaber.Ranked;
        }

        public static BeatMapMode GetFilterTargetMapMode(object item)
        {
            return ((IntegrationScore)item).Map.MapMode;
        }

        public static BeatMapDifficulty GetFilterTargetMapDifficulty(object item)
        {
            return ((IntegrationScore)item).Map.MapDifficulty;
        }

        public static DateTime? GetFilterTargetMapRankedDate(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).Map.BeatLeader.RankedTime : ((IntegrationScore)item).Map.ScoreSaber.RankedTime;
        }

        public static double GetFilterTargetMapStar(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).Map.BeatLeader.Star : ((IntegrationScore)item).Map.ScoreSaber.Star;
        }

        public static double GetFilterTargetMapDuration(object item)
        {
            return ((IntegrationScore)item).Map.Duration;
        }

        public static double GetFilterTargetMapBpm(object item)
        {
            return ((IntegrationScore)item).Map.Bpm;
        }

        public static long GetFilterTargetMapNotes(object item)
        {
            return ((IntegrationScore)item).Map.Notes;
        }

        public static long GetFilterTargetMapBombs(object item)
        {
            return ((IntegrationScore)item).Map.Bombs;
        }

        public static long GetFilterTargetMapWalls(object item)
        {
            return ((IntegrationScore)item).Map.Walls;
        }

        public static double GetFilterTargetMapNps(object item)
        {
            return ((IntegrationScore)item).Map.Nps;
        }

        public static double GetFilterTargetMapNjs(object item)
        {
            return ((IntegrationScore)item).Map.Njs;
        }

        public static DateTime? GetFilterTargetPlayUpdateDate(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.TimeSet : ((IntegrationScore)item).ScoreSaber.TimeSet;
        }

        public static PlayResultType GetFilterTargetPlayResultType(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.ResultType : ((IntegrationScore)item).ScoreSaber.ResultType;
        }

        public static bool GetFilterTargetPlayIsFullCombo(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.FullCombo.Length > 0 : ((IntegrationScore)item).ScoreSaber.FullCombo.Length > 0;
        }

        public static double GetFilterTargetPlayPp(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.PP : ((IntegrationScore)item).ScoreSaber.PP;
        }

        public static double GetFilterTargetPlayAcc(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.Acc : ((IntegrationScore)item).ScoreSaber.Acc;
        }

        public static long GetFilterTargetPlayWorldRank(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.WorldRank : ((IntegrationScore)item).ScoreSaber.WorldRank;
        }

        public static long GetFilterTargetPlayMissPlusBad(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.MissPlusBad : ((IntegrationScore)item).ScoreSaber.MissPlusBad;
        }

        public static long GetFilterTargetPlayMiss(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.Miss : ((IntegrationScore)item).ScoreSaber.Miss;
        }

        public static long GetFilterTargetPlayBad(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.Bad : ((IntegrationScore)item).ScoreSaber.Bad;
        }

        public static ModifiersFlag GetFilterTargetPlayModifiers(object item)
        {
            bool beatLeader = AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            return beatLeader ? ((IntegrationScore)item).BeatLeader.Modifiers2 : ((IntegrationScore)item).ScoreSaber.Modifiers2;
        }

        public static bool GetFilterTargetEtcCheckedOnly(object item)
        {
            return ((IntegrationScore)item).IsShowChekdOnlySelected;
        }
    }

    [Flags]
    public enum ModifiersFlag
    {
        BE = 1 << 0, // 4 Lives (Battery Energy)
        DA = 1 << 1, // Disappearing Arrows
        FS = 1 << 2, // Faster Song
        GN = 1 << 3, // Ghost Notes
        IF = 1 << 4, // 1 Life (Insta Fail)
        NA = 1 << 5, // No Arrows
        NB = 1 << 6, // No Bombs
        NF = 1 << 7, // No Fail
        NO = 1 << 8, // No Walls (No Obstacles)
        OD = 1 << 9, // Old Dots
        OP = 1 << 10, // Out of Platform
        PM = 1 << 11, // Pro Mode
        SC = 1 << 12, // Small Notes
        SF = 1 << 13, // Super Fast Song
        SS = 1 << 14, // Slower Song
    }

    public class ScoreBase
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
        /// OD:Old Dots (ビートセイバーのバージョンが古いとドットノーツのヒットボックスが異なっていることを表す)
        /// OP:Out of Platform (BeatLeaderはプラットフォーム前方(正常な範囲外)でプレイするとNJS Cheesingとしてスコアが下がる)
        /// PM:Pro Mode.
        /// SC:Small Notes,
        /// SF:Super Fast Song.
        /// SS:Slower Song.
        /// </summary>
        public string Modifiers { get; protected set; } = string.Empty;

        public ModifiersFlag Modifiers2 { get; protected set; } = new();

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

        /// <summary>
        /// プレイ結果
        /// </summary>
        public PlayResultType ResultType { get; protected set; }

        public static bool IsFailureByConfig(string modifiers)
        {
            if (modifiers.Length > 0)
            {
                foreach (var f in Config.Failures)
                {
                    if (modifiers.Contains(f))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static PlayResultType GetPlayResultType(long modifiedScore, string modifiers)
        {
            if (modifiedScore < 0) // スコアなし＝プレイしていない
            {
                return PlayResultType.NotPlay;
            }
            else if (IsFailureByConfig(modifiers)) // モディファイに失敗相当の文字列あり＝フェイルしている
            {
                return PlayResultType.Failure;
            }
            else
            {
                return PlayResultType.Clear;
            }
        }

        public static ModifiersFlag ParseModifiers(string modifiers)
        {
            ModifiersFlag mods = 0;
            if (modifiers.Length > 0)
            {
                foreach (var mod in modifiers.Split(','))
                {
                    switch (mod.Trim())
                    {
                        case "BE": mods |= ModifiersFlag.BE; break;
                        case "DA": mods |= ModifiersFlag.DA; break;
                        case "FS": mods |= ModifiersFlag.FS; break;
                        case "GN": mods |= ModifiersFlag.GN; break;
                        case "IF": mods |= ModifiersFlag.IF; break;
                        case "NA": mods |= ModifiersFlag.NA; break;
                        case "NB": mods |= ModifiersFlag.NB; break;
                        case "NF": mods |= ModifiersFlag.NF; break;
                        case "NO": mods |= ModifiersFlag.NO; break;
                        case "OD": mods |= ModifiersFlag.OD; break;
                        case "OP": mods |= ModifiersFlag.OP; break;
                        case "PM": mods |= ModifiersFlag.PM; break;
                        case "SC": mods |= ModifiersFlag.SC; break;
                        case "SF": mods |= ModifiersFlag.SF; break;
                        case "SS": mods |= ModifiersFlag.SS; break;
                    }
                }
            }
            return mods;
        }
    }

    public class ScoreSaberScore : ScoreBase
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
            Modifiers2 = ParseModifiers(Modifiers);
            ScoreCount = 0;
            MissPlusBad = 0;
            Miss = 0;
            Bad = 0;
            FullCombo = "";
            WorldRank = 0;
            ResultType = PlayResultType.NotPlay;
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
            Modifiers2 = ParseModifiers(Modifiers);
            ScoreCount = results.Count;
            MissPlusBad = score.score.badCuts + score.score.missedNotes;
            Miss = score.score.missedNotes;
            Bad = score.score.badCuts;
            FullCombo = (score.score.fullCombo) ? "FC" : "";
            WorldRank = score.score.rank;
            ResultType = GetPlayResultType(ModifiedScore, Modifiers);
        }
    }

    public class BeatLeaderScore : ScoreBase
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
            Modifiers2 = ParseModifiers(Modifiers);
            ScoreCount = 0;
            MissPlusBad = 0;
            Miss = 0;
            Bad = 0;
            FullCombo = "";
            WorldRank = 0;
            ResultType = PlayResultType.NotPlay;
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
            Modifiers2 = ParseModifiers(Modifiers);
            ScoreCount = results.Count;
            MissPlusBad = score.badCuts + score.missedNotes;
            Miss = score.missedNotes;
            Bad = score.badCuts;
            FullCombo = (score.fullCombo) ? "FC" : "";
            WorldRank = score.rank;
            ResultType = GetPlayResultType(ModifiedScore, Modifiers);
        }
    }
}
