using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeatSaberScore
{
    internal static class Config
    {
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
        private static readonly string _dataDir = Path.Combine("data");
        private static readonly string _configPath = Path.Combine(_dataDir, "config.json");

        private static ConfigData _data = new();

        #region 列のタグ名
        public const string ColumnTagCheckBox = "CheckBox";
        public const string ColumnTagBsr = "Bsr";
        public const string ColumnTagCover = "Cover";
        public const string ColumnTagSongName = "SongName";
        public const string ColumnTagDate = "Date";
        public const string ColumnTagMode = "Mode";
        public const string ColumnTagDifficulty = "Difficulty";
        public const string ColumnTagStars = "Stars";
        public const string ColumnTagScore = "Score";
        public const string ColumnTagAcc = "Acc";
        public const string ColumnTagMissPlusBad = "MissPlusBad";
        public const string ColumnTagFullCombo = "FullCombo";
        public const string ColumnTagPp = "Pp";
        public const string ColumnTagModifiers = "Modifiers";
        public const string ColumnTagCopyBsr = "CopyBsr";
        public const string ColumnTagJumpBeatSaver = "JumpBeatSaver";
        public const string ColumnTagJumpScoreSaber = "JumpScoreSaber";
        public const string ColumnTagDuration = "Duration";
        public const string ColumnTagBpm = "Bpm";
        public const string ColumnTagNotes = "Notes";
        public const string ColumnTagNps = "Nps";
        public const string ColumnTagNjs = "Njs";
        public const string ColumnTagBombs = "Bombs";
        public const string ColumnTagObstacles = "Obstacles";
        public const string ColumnTagMiss = "Miss";
        public const string ColumnTagBad = "Bad";
        public const string ColumnTagHash = "Hash";
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
                SaveLocalFile();
            }
        }

        public static List<string> Failures => _data.failures;

        public static ObservableCollection<User> FavoriteUsers => _data.favoUsers;

        public static Grid GridSetting => _data.grid;

        public static void LoadLocalFile()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string jsonString = File.ReadAllText(_configPath, Encoding.UTF8);
                    var data = JsonSerializer.Deserialize<ConfigData>(jsonString);
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

        public static void SaveLocalFile()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize<ConfigData>(_data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configPath, jsonString);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.ToString());
            }
        }

        [DataContract]
        public class ConfigData
        {
            [DataMember]
            public string scoreSaberProfileId { get; set; }

            [DataMember]
            public List<string> failures { get; set; }

            [DataMember]
            public ObservableCollection<User> favoUsers { get; set; }

            [DataMember]
            public Grid grid { get; set; }

            public ConfigData()
            {
                scoreSaberProfileId = "";
                failures = new();
                favoUsers = new();
                grid = new Grid();
            }

            public void Normalize()
            {
                if (failures == null)
                {
                    failures = new() { "NF", "SS" };
                }
                if (failures.Count == 0)
                {
                    failures.Add("NF");
                    failures.Add("SS");
                }
            }
        }

        [DataContract]
        public class User
        {
            [DataMember]
            public string id { get; set; }

            [DataMember]
            public string name { get; set; }

            [IgnoreDataMember]
            public string profilePicture { get { return $"https://cdn.scoresaber.com/avatars/{id}.jpg"; } }

            public User()
            {
                id = "";
                name = "";
            }

            public User(string id, string name)
            {
                this.id = id;
                this.name = name;
            }
        }

        [DataContract]
        public class Grid
        {
            [DataMember]
            public int rowHeight { get; set; }

            [DataMember]
            public List<String> notDisplayColumns { get; set; }

            public Grid()
            {
                rowHeight = 45;
                notDisplayColumns = new List<String>();
            }
        }
    }
}
