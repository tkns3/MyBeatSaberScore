using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatSaver
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static async Task<MapDetail> GetMapDetailByHash(string hash)
        {
            string url = $"https://beatsaver.com/api/maps/hash/{hash}";

            try
            {
                _logger.Info(url);
                var result = await HttpTool.GetAndDeserialize<MapDetail>(url);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new MapDetail();
        }

        public class MapCollection
        {
            public List<MapDetail> mapDetails { get; set; } = new();
        }

        public class MapDetail
        {
            public bool automapper { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime? curatedAt { get; set; }
            public UserDetail? curator { get; set; } = new();
            public DateTime? deletedAt { get; set; }
            public string description { get; set; } = "";
            public string id
            {
                get
                {
                    return _id;
                }
                set
                {
                    _id = value.ToLower();
                }
            }
            public DateTime lastPublishedAt { get; set; }
            public MapDetailMetadata metadata { get; set; } = new();
            public string name { get; set; } = "";
            public bool qualified { get; set; }
            public bool ranked { get; set; }
            public MapStats stats { get; set; } = new();
            public List<string> tags { get; set; } = new();
            public DateTime updatedAt { get; set; }
            public DateTime uploaded { get; set; }
            public UserDetail uploader { get; set; } = new();
            public List<MapVersion> versions { get; set; } = new();

            private string _id = "";
        }

        public class MapDetailMetadata
        {
            public double bpm { get; set; }
            public int duration { get; set; }
            public string levelAuthorName { get; set; } = "";
            public string songAuthorName { get; set; } = "";
            public string songName { get; set; } = "";
            public string songSubName { get; set; } = "";
        }

        public class MapStats
        {
            public int downloads { get; set; }
            public int downvotes { get; set; }
            public int plays { get; set; }
            public int reviews { get; set; }
            public double score { get; set; }
            public double scoreOneDP { get; set; }
            public string sentiment { get; set; } = "";
            public int upvotes { get; set; }
        }

        public class UserDetail
        {
            public bool admin { get; set; }
            public string avatar { get; set; } = "";
            public bool curator { get; set; }
            public string description { get; set; } = "";
            public string email { get; set; } = "";
            public string hash { get; set; } = "";
            public int id { get; set; }
            public string name { get; set; } = "";
            public UserStats stats { get; set; } = new();
            public bool testplay { get; set; }
            public string type { get; set; } = "";
            public bool uniqueSet { get; set; }
            public int uploadLimit { get; set; }
            public bool verifiedMapper { get; set; }
        }

        public class UserStats
        {
            public double avgBpm { get; set; }
            public double avgDuration { get; set; }
            public double avgScore { get; set; }
            public UserDiffStats diffStats { get; set; } = new();
            public string firstUpload { get; set; } = "";
            public string lastUpload { get; set; } = "";
            public int rankedMaps { get; set; }
            public int totalDownvotes { get; set; }
            public int totalMaps { get; set; }
            public int totalUpvotes { get; set; }
        }

        public class UserDiffStats
        {
            public int easy { get; set; }
            public int expert { get; set; }
            public int expertPlus { get; set; }
            public int hard { get; set; }
            public int normal { get; set; }
            public int total { get; set; }
        }

        public class MapVersion
        {
            public string coverURL { get; set; } = "";
            public DateTime createdAt { get; set; }
            public List<MapDifficulty> diffs { get; set; } = new();
            public string downloadURL { get; set; } = "";
            public string feedback { get; set; } = "";
            public string hash
            {
                get
                {
                    return _hash;
                }
                set
                {
                    _hash = value.ToLower();
                }
            }
            public string key
            {
                get
                {
                    return _key;
                }
                set
                {
                    _key = value.ToLower();
                }
            }
            public string previewURL { get; set; } = "";
            public int sageScore { get; set; }
            public DateTime scheduledAt { get; set; }
            public string state { get; set; } = "";
            public DateTime testplayAt { get; set; }
            public List<MapTestplay> testplays { get; set; } = new();

            private string _hash = "";
            private string _key = "";
        }

        public class MapDifficulty
        {
            public int bombs { get; set; }
            public string characteristic
            {
                get
                {
                    return _characteristic;
                }
                set
                {
                    _characteristic = value;
                    mapMode = value switch
                    {
                        "Standard" => BeatMapMode.Standard,
                        "OneSaber" => BeatMapMode.OneSaber,
                        "NoArrows" => BeatMapMode.NoArrows,
                        "Lightshow" => BeatMapMode.Lightshow,
                        "90Degree" => BeatMapMode.Degree90,
                        "360Degree" => BeatMapMode.Degree360,
                        "Lawless" => BeatMapMode.Lawless,
                        _ => BeatMapMode.Unknown,
                    };
                }
            }
            public bool chroma { get; set; }
            public bool cinema { get; set; }
            public string difficulty
            {
                get
                {
                    return _difficulty;
                }
                set
                {
                    _difficulty = value;
                    mapDifficulty = value switch
                    {
                        "Easy" => BeatMapDifficulty.Easy,
                        "Normal" => BeatMapDifficulty.Normal,
                        "Hard" => BeatMapDifficulty.Hard,
                        "Expert" => BeatMapDifficulty.Expert,
                        "ExpertPlus" => BeatMapDifficulty.ExpertPlus,
                        _ => BeatMapDifficulty.Unknown,
                    };
                }
            }
            public int events { get; set; }
            public double length { get; set; }
            public bool me { get; set; }
            public bool ne { get; set; }
            public double njs { get; set; }
            public int notes { get; set; }
            public double nps { get; set; }
            public int obstacles { get; set; }
            public double offset { get; set; }
            public MapParitySummary paritySummary { get; set; } = new();
            public double seconds { get; set; }
            public double stars { get; set; }

            private string _characteristic = "";
            private string _difficulty = "";

            internal BeatMapMode mapMode;
            internal BeatMapDifficulty mapDifficulty;

        }

        public class MapParitySummary
        {
            public int errors { get; set; }
            public int resets { get; set; }
            public int warns { get; set; }
        }

        public class MapTestplay
        {
            public DateTime createdAt { get; set; }
            public string feedback { get; set; } = "";
            public string feedbackAt { get; set; } = "";
            public UserDetail user { get; set; } = new();
            public string video { get; set; } = "";
        }
    }
}
