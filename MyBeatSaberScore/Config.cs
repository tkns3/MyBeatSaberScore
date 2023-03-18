using MyBeatSaberScore.APIs;
using MyBeatSaberScore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

namespace MyBeatSaberScore
{
    internal static class Config
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly string _dataDir = Path.Combine("data");
        private static readonly string _configPath = Path.Combine(_dataDir, "config.json");

        private static ConfigData _data = new();

        #region 列のタグ名
        // マップ情報
        public const string ColumnTagMapBsr = "Map.Bsr";
        public const string ColumnTagMapCover = "Map.Cover";
        public const string ColumnTagMapSongName = "Map.SongName";
        public const string ColumnTagMapMode = "Map.Mode";
        public const string ColumnTagMapDifficulty = "Map.Difficulty";
        public const string ColumnTagMapDuration = "Map.Duration";
        public const string ColumnTagMapBpm = "Map.Bpm";
        public const string ColumnTagMapNotes = "Map.Notes";
        public const string ColumnTagMapNps = "Map.Nps";
        public const string ColumnTagMapNjs = "Map.Njs";
        public const string ColumnTagMapBombs = "Map.Bombs";
        public const string ColumnTagMapWalls = "Map.Walls";
        public const string ColumnTagMapHash = "Map.Hash";
        public const string ColumnTagMapScoreSaberRankedDate = "Map.ScoreSaber.RankedDate";
        public const string ColumnTagMapScoreSaberStar = "Map.ScoreSaber.Star";
        public const string ColumnTagMapBeatLeaderRankedDate = "Map.BeatLeader.RankedDate";
        public const string ColumnTagMapBeatLeaderStar = "Map.BeatLeader.Star";
        // ScoreSaber
        public const string ColumnTagScoreSaberDate = "ScoreSaber.Date";
        public const string ColumnTagScoreSaberScore = "ScoreSaber.Score";
        public const string ColumnTagScoreSaberAcc = "ScoreSaber.Acc";
        public const string ColumnTagScoreSaberAccDiff = "ScoreSaber.AccDiff";
        public const string ColumnTagScoreSaberMissPlusBad = "ScoreSaber.MissPlusBad";
        public const string ColumnTagScoreSaberFullCombo = "ScoreSaber.FullCombo";
        public const string ColumnTagScoreSaberPp = "ScoreSaber.Pp";
        public const string ColumnTagScoreSaberModifiers = "ScoreSaber.Modifiers";
        public const string ColumnTagScoreSaberScoreCount = "ScoreSaber.ScoreCount";
        public const string ColumnTagScoreSaberMiss = "ScoreSaber.Miss";
        public const string ColumnTagScoreSaberBad = "ScoreSaber.Bad";
        public const string ColumnTagScoreSaberWorldRank = "ScoreSaber.WorldRank";
        // BeatLeader
        public const string ColumnTagBeatLeaderDate = "BeatLeader.Date";
        public const string ColumnTagBeatLeaderScore = "BeatLeader.Score";
        public const string ColumnTagBeatLeaderAcc = "BeatLeader.Acc";
        public const string ColumnTagBeatLeaderAccDiff = "BeatLeader.AccDiff";
        public const string ColumnTagBeatLeaderMissPlusBad = "BeatLeader.MissPlusBad";
        public const string ColumnTagBeatLeaderFullCombo = "BeatLeader.FullCombo";
        public const string ColumnTagBeatLeaderPp = "BeatLeader.Pp";
        public const string ColumnTagBeatLeaderModifiers = "BeatLeader.Modifiers";
        public const string ColumnTagBeatLeaderScoreCount = "BeatLeader.ScoreCount";
        public const string ColumnTagBeatLeaderMiss = "BeatLeader.Miss";
        public const string ColumnTagBeatLeaderBad = "BeatLeader.Bad";
        public const string ColumnTagBeatLeaderWorldRank = "BeatLeader.WorldRank";
        // その他
        public const string ColumnTagCheckBox = "CheckBox";
        public const string ColumnTagCopyBsr = "CopyBsr";
        public const string ColumnTagJumpBeatSaver = "JumpBeatSaver";
        public const string ColumnTagJumpScoreSaber = "JumpScoreSaber";
        public const string ColumnTagJumpBeatLeader = "JumpBeatLeader";
        #endregion

        public static string ScoreSaberProfileId
        {
            get
            {
                return _data.scoreSaberProfileId;
            }
            set
            {
                _data.scoreSaberProfileId = value;
                SaveToLocalFile();
            }
        }

        public static List<string> Failures => _data.failures;

        public static ObservableCollection<User> FavoriteUsers => _data.favoUsers;

        public static ViewTarget ViewTarget { get => _data.viewTarget; set => _data.viewTarget = value; }

        public static GridConfig Grid => _data.grid;

        public static WindowConfig Window => _data.window;

        public static void LoadFromLocalFile()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var data = Utility.Json.DeserializeFromLocalFile<ConfigData>(_configPath);
                    if (data != null)
                    {
                        data.Normalize();
                        _data = data;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.ToString());
            }
        }

        public static void SaveToLocalFile()
        {
            try
            {
                Utility.Json.SerializeToLocalFile(_data, _configPath, Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.ToString());
            }
        }

        public static string GetPairTagName(string tagName)
        {
            return tagName switch
            {
                ColumnTagScoreSaberDate => ColumnTagBeatLeaderDate,
                ColumnTagScoreSaberScore => ColumnTagBeatLeaderScore,
                ColumnTagScoreSaberAcc => ColumnTagBeatLeaderAcc,
                ColumnTagScoreSaberAccDiff => ColumnTagBeatLeaderAccDiff,
                ColumnTagScoreSaberMissPlusBad => ColumnTagBeatLeaderMissPlusBad,
                ColumnTagScoreSaberFullCombo => ColumnTagBeatLeaderFullCombo,
                ColumnTagScoreSaberPp => ColumnTagBeatLeaderPp,
                ColumnTagScoreSaberModifiers => ColumnTagBeatLeaderModifiers,
                ColumnTagScoreSaberScoreCount => ColumnTagBeatLeaderScoreCount,
                ColumnTagScoreSaberMiss => ColumnTagBeatLeaderMiss,
                ColumnTagScoreSaberBad => ColumnTagBeatLeaderBad,
                ColumnTagScoreSaberWorldRank => ColumnTagBeatLeaderWorldRank,
                ColumnTagMapScoreSaberRankedDate => ColumnTagMapBeatLeaderRankedDate,
                ColumnTagMapScoreSaberStar => ColumnTagMapBeatLeaderStar,
                ColumnTagBeatLeaderDate => ColumnTagScoreSaberDate,
                ColumnTagBeatLeaderScore => ColumnTagScoreSaberScore,
                ColumnTagBeatLeaderAcc => ColumnTagScoreSaberAcc,
                ColumnTagBeatLeaderAccDiff => ColumnTagScoreSaberAccDiff,
                ColumnTagBeatLeaderMissPlusBad => ColumnTagScoreSaberMissPlusBad,
                ColumnTagBeatLeaderFullCombo => ColumnTagScoreSaberFullCombo,
                ColumnTagBeatLeaderPp => ColumnTagScoreSaberPp,
                ColumnTagBeatLeaderModifiers => ColumnTagScoreSaberModifiers,
                ColumnTagBeatLeaderScoreCount => ColumnTagScoreSaberScoreCount,
                ColumnTagBeatLeaderMiss => ColumnTagScoreSaberMiss,
                ColumnTagBeatLeaderBad => ColumnTagScoreSaberBad,
                ColumnTagBeatLeaderWorldRank => ColumnTagScoreSaberWorldRank,
                ColumnTagMapBeatLeaderRankedDate => ColumnTagMapScoreSaberRankedDate,
                ColumnTagMapBeatLeaderStar => ColumnTagMapScoreSaberStar,
                _ => "unknown"
            };
        }

#pragma warning disable IDE1006 // 命名スタイル
        public class ConfigData
        {
            public string scoreSaberProfileId { get; set; } = string.Empty;

            public ViewTarget viewTarget { get; set; }

            public List<string> failures { get; set; } = new();

            public ObservableCollection<User> favoUsers { get; set; } = new();

            public WindowConfig window { get; set; } = new();

            public GridConfig grid { get; set; } = new();

            public void Normalize()
            {
                if (failures.Count == 0)
                {
                    failures.Add("NF");
                    failures.Add("SS");
                }

                if (viewTarget == ViewTarget.None)
                {
                    viewTarget = ViewTarget.ScoreSaber;
                }
            }
        }

        public class User
        {
            public string id { get; set; } = string.Empty;

            public string beatLeaderName { get; set; } = string.Empty;

            public string beatLeaderAvatar { get; set; } = string.Empty;

            public string scoreSaberName { get; set; } = string.Empty;

            public string scoreSaberAvatar { get; set; } = string.Empty;

            public User()
            {
            }

            public User(string id, BeatLeader.PlayerResponseFull beatLeaderProfile, ScoreSaber.PlayerProfile scoreSaberProfile)
            {
                this.id = id;
                beatLeaderName = beatLeaderProfile.name;
                beatLeaderAvatar = beatLeaderProfile.avatar;
                scoreSaberName = scoreSaberProfile.name;
                scoreSaberAvatar = scoreSaberProfile.profilePicture;
            }
        }

        public class GridConfig
        {
            public int rowHeight { get; set; } = 45;

            public List<String> notDisplayColumns { get; set; } = new();

            public GridColumnRestoreParam columnRestore { get; set; } = new();
        }

        public class GridColumnRestoreParam
        {
            public RestoreMode mode { get; set; } = RestoreMode.Last;

            public DateTime? savedDate { get; set; }

            public List<GridColumnParam> lastParams { get; set; } = new();

            public List<GridColumnParam> savedParams { get; set; } = new();
        }

        public class GridColumnParam
        {
            public string name { get; set; } = "";

            /// <summary>
            /// "Auto" または double.Parse() が可能な文字列。
            /// </summary>
            public string width { get; set; } = "Auto";

            public int displayIndex { get; set; } = -1;

            public GridColumnParam()
            {
            }

            public GridColumnParam(DataGridColumn column)
            {
                name = TagBehavior.GetTag(column).ToString() ?? "";
                displayIndex = column.DisplayIndex;
                width = column.Width.ToString();
            }
        }

        public class WindowConfig
        {
            public WindowBoundsRestoreParam boundsRestore { get; set; } = new();
        }

        public class WindowBoundsRestoreParam
        {
            public RestoreMode mode { get; set; } = RestoreMode.Last;

            public DateTime? savedDate { get; set; }

            public WindowBounds last { get; set; } = new();

            public WindowBounds saved { get; set; } = new();
        }

        public class WindowBounds
        {
            public double top { get; set; } = 100;

            public double left { get; set; } = 100;

            public double width { get; set; } = 1200;

            public double height { get; set; } = 800;

            public double vtop { get; set; } = 0;

            public double vleft { get; set; } = 0;

            public double vwidth { get; set; } = 0;

            public double vheight { get; set; } = 0;

            public bool maximized { get; set; } = false;
        }

        /// <summary>
        /// 復元方法。
        /// </summary>
        public enum RestoreMode
        {
            /// <summary>
            /// デフォルトパラメータで福毛
            /// </summary>
            Default,

            /// <summary>
            /// 前回終了時のパラメータで復元
            /// </summary>
            Last,

            /// <summary>
            /// 保存しているパラメータで復元
            /// </summary>
            Saved,
        }
#pragma warning restore IDE1006 // 命名スタイル
    }
}
