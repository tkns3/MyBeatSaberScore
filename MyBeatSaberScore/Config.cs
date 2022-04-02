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
        private static readonly string _dataDir = Path.Combine("data");
        private static readonly string _configPath = Path.Combine(_dataDir, "config.json");

        private static ConfigData _data = new();

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
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
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
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
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

            public ConfigData()
            {
                scoreSaberProfileId = "";
                failures = new();
                favoUsers = new();
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
    }
}
