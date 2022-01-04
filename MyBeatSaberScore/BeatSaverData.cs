using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeatSaberScore
{
    internal class BeatSaverData
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _mapsPath = Path.Combine(_mapsDir, "maps.json");
        private static readonly string _coverDir = Path.Combine(_mapsDir, "cover");
        private static readonly string _deletedmapsPath = Path.Combine(_mapsDir, "deletedmaps.json");

        public Dictionary<string, MapDetail> _acquiredMaps = new();
        private HashSet<string> _deletedMaps = new();

        public BeatSaverData()
        {
            Directory.CreateDirectory(_mapsDir);
            Directory.CreateDirectory(_coverDir);
        }

        public void LoadLocalFile()
        {
            _acquiredMaps.Clear();
            if (File.Exists(_mapsPath))
            {
                string jsonString = File.ReadAllText(_mapsPath, Encoding.UTF8);
                var collection = JsonSerializer.Deserialize<MapCollection>(jsonString);
                if (collection != null)
                {
                    foreach (var detail in collection.mapDetails)
                    {
                        if (detail != null && NormalizeMapDetail(detail) == true)
                        {
                            _acquiredMaps[detail.versions[0].hash] = detail;
                        }
                    }
                }
            }

            _deletedMaps.Clear();
            if (File.Exists(_deletedmapsPath))
            {
                string jsonString = File.ReadAllText(_deletedmapsPath, Encoding.UTF8);
                var collection = JsonSerializer.Deserialize<DeletedMapCollection>(jsonString);
                if (collection != null)
                {
                    foreach (var hash in collection.hashs)
                    {
                        _deletedMaps.Add(hash);
                    }
                }
            }
        }

        public void SaveLocalFile()
        {
            {
                var collection = new MapCollection();
                collection.mapDetails = _acquiredMaps.Values.ToArray();
                var jsonString = JsonSerializer.Serialize<MapCollection>(collection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_mapsPath, jsonString);
            }
            {
                var collection = new DeletedMapCollection();
                collection.hashs = _deletedMaps.ToArray();
                var jsonString = JsonSerializer.Serialize<DeletedMapCollection>(collection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_deletedmapsPath, jsonString);
            }
        }

        public int GetMaxScore(string hash, int difficultyRawInt)
        {
            if (!_acquiredMaps.ContainsKey(hash))
            {
                return 0;
            }

            var detail = _acquiredMaps[hash];
            var list = detail.versions[0].diffs.Where(d => d.difficultyRawInt == difficultyRawInt);
            if (list.Any())
            {
                int[] scoreArray = new int[] { 0, 115, 345, 575, 805, 1035, 1495, 1955, 2415, 2875, 3335, 3795, 4255, 4715 };
                if (list.First().notes < 14)
                {
                    return scoreArray[list.First().notes];
                }
                else
                {
                    return 4715 + (list.First().notes - 13) * 920;
                }
            }
            else
            {
                return 0;
            }
        }

        public string GetAlleadyKey(string hash)
        {
            hash = hash.ToLower();

            if (_acquiredMaps.ContainsKey(hash))
            {
                return _acquiredMaps[hash].id;
            }

            return "";
        }

        public async Task<string> GetKey(string hash)
        {
            hash = hash.ToLower();

            if (_acquiredMaps.ContainsKey(hash))
            {
                return _acquiredMaps[hash].id;
            }

            if (_deletedMaps.Contains(hash))
            {
                return "";
            }

            var map = await GetMapDetailByHash(hash);
            if (map?.versions?.Length > 0)
            {
                _acquiredMaps[hash] = map;
                return map.id;
            }
            else
            {
                return "";
            }
        }

        public async Task<MapDetail> GetMapDetailByHash(string hash)
        {
            hash = hash.ToLower();
            string url = $"https://beatsaver.com/api/maps/hash/{hash}";

            try
            {
                var httpsResponse = await _client.GetAsync(url);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var detail = JsonSerializer.Deserialize<MapDetail>(responseContent);

                if (detail?.error?.Contains("Not Found") == true)
                {
                    _deletedMaps.Add(hash);
                }
                else if (detail != null && NormalizeMapDetail(detail) == true)
                {
                    return detail;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + url);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }

            return new MapDetail();
        }

        public static string GetCoverLocalPath(PlayerScore score)
        {
            return GetCoverLocalPath(score.leaderboard.songHash);
        }

        public static string GetCoverLocalPath(string hash)
        {
            hash = hash.ToLower();

            var _localPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, _coverDir, $"{hash}.png");
            if (System.IO.File.Exists(_localPath))
            {
                return _localPath;
            }
            else
            {
                return "Resources/_404.png";
            }
        }

        public static async Task<string> GetCover(PlayerScore score)
        {
            return await GetCover(score.leaderboard.songHash, score.leaderboard.coverImage);
        }

        public static async Task<string> GetCover(string hash, string url)
        {
            hash = hash.ToLower();

            var _localPath = System.IO.Path.Combine(_coverDir, $"{hash}.png");
            if (System.IO.File.Exists(_localPath))
            {
                return _localPath;
            }

            try
            {
                HttpResponseMessage res = await _client.GetAsync(url);
                using (var fileStream = File.Create(_localPath))
                {
                    using (var httpStream = await res.Content.ReadAsStreamAsync())
                    {
                        httpStream.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                }
                return _localPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + url);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
                return System.IO.Path.Combine(_coverDir, "_404.png");
            }
        }

        private static bool NormalizeMapDetail(MapDetail detail)
        {
            if (detail.id != null && detail.versions?.Length > 0)
            {
                foreach (MapVersion v in detail.versions)
                {
                    foreach (MapDifficulty d in v.diffs)
                    {
                        d.difficultyRawInt = MapDifficulty.ToDifficultyRawInt(d.characteristic, d.difficulty);
                    }
                }

                detail.id = detail.id.ToLower();
                detail.versions[0].hash = detail.versions[0].hash.ToLower();

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [DataContract]
    public class DeletedMapCollection
    {
        [DataMember]
        public string[] hashs { get; set; }

        public DeletedMapCollection()
        {
            hashs = Array.Empty<string>();
        }
    }

    [DataContract]
    public class MapCollection
    {
        [DataMember]
        public MapDetail[] mapDetails { get; set; }

        public MapCollection()
        {
            mapDetails = Array.Empty<MapDetail>();
        }
    }

    [DataContract]
    public class MapDetail
    {
        [DataMember]
        public bool automapper { get; set; }

        [DataMember]
        public string createdAt { get; set; }

        [DataMember]
        public string curator { get; set; }

        [DataMember]
        public string deletedAt { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string lastPublishedAt { get; set; }

        [DataMember]
        public MapDetailMetadata metadata { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public bool qualified { get; set; }

        [DataMember]
        public bool ranked { get; set; }

        [DataMember]
        public MapStats stats { get; set; }

        [DataMember]
        public string updatedAt { get; set; }

        [DataMember]
        public string uploaded { get; set; }

        [DataMember]
        public UserDetail uploader { get; set; }

        [DataMember]
        public MapVersion[] versions { get; set; }

        [DataMember]
        public string error { get; set; }

        public MapDetail()
        {
            createdAt = "";
            curator = "";
            deletedAt = "";
            description = "";
            id = "";
            lastPublishedAt = "";
            metadata = new MapDetailMetadata();
            name = "";
            stats = new MapStats();
            updatedAt = "";
            uploaded = "";
            uploader = new UserDetail();
            versions = new MapVersion[0];
            error = "";
        }
    }

    [DataContract]
    public class Instant
    {
        [DataMember]
        public int epochSeconds { get; set; }

        [DataMember]
        public int nanosecondsOfSecond { get; set; }

        [DataMember]
        public string value { get; set; }

        public Instant()
        {
            value = "";
        }
    }

    [DataContract]
    public class MapDetailMetadata
    {
        [DataMember]
        public double bpm { get; set; }

        [DataMember]
        public int duration { get; set; }

        [DataMember]
        public string levelAuthorName { get; set; }

        [DataMember]
        public string songAuthorName { get; set; }

        [DataMember]
        public string songName { get; set; }

        [DataMember]
        public string songSubName { get; set; }

        public MapDetailMetadata()
        {
            levelAuthorName = "";
            songAuthorName = "";
            songName = "";
            songSubName = "";
        }
    }

    [DataContract]
    public class MapStats
    {
        [DataMember]
        public int downloads { get; set; }

        [DataMember]
        public int downvotes { get; set; }

        [DataMember]
        public int plays { get; set; }

        [DataMember]
        public double score { get; set; }

        [DataMember]
        public int upvotes { get; set; }

        public MapStats()
        {
        }
    }

    [DataContract]
    public class UserDetail
    {
        [DataMember]
        public string avatar { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string hash { get; set; }

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public UserStats stats { get; set; }

        [DataMember]
        public bool testplay { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public bool uniqueSet { get; set; }

        [DataMember]
        public int uploadLimit { get; set; }

        public UserDetail()
        {
            avatar = "";
            email = "";
            hash = "";
            name = "";
            stats = new UserStats();
            type = "";
        }
    }

    [DataContract]
    public class UserStats
    {
        [DataMember]
        public double avgBpm { get; set; }

        [DataMember]
        public double avgDuration { get; set; }

        [DataMember]
        public double avgScore { get; set; }

        [DataMember]
        public UserDiffStats diffStats { get; set; }

        [DataMember]
        public string firstUpload { get; set; }

        [DataMember]
        public string lastUpload { get; set; }

        [DataMember]
        public int rankedMaps { get; set; }

        [DataMember]
        public int totalDownvotes { get; set; }

        [DataMember]
        public int totalMaps { get; set; }

        [DataMember]
        public int totalUpvotes { get; set; }

        public UserStats()
        {
            diffStats = new UserDiffStats();
            firstUpload = "";
            lastUpload = "";
        }
    }

    [DataContract]
    public class UserDiffStats
    {
        [DataMember]
        public int easy { get; set; }

        [DataMember]
        public int expert { get; set; }

        [DataMember]
        public int expertPlus { get; set; }

        [DataMember]
        public int hard { get; set; }

        [DataMember]
        public int normal { get; set; }

        [DataMember]
        public int total { get; set; }
    }

    [DataContract]
    public class MapVersion
    {
        [DataMember]
        public string coverURL { get; set; }

        [DataMember]
        public string createdAt { get; set; }

        [DataMember]
        public MapDifficulty[] diffs { get; set; }

        [DataMember]
        public string downloadURL { get; set; }

        [DataMember]
        public string feedback { get; set; }

        [DataMember]
        public string hash { get; set; }

        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string previewURL { get; set; }

        [DataMember]
        public int sageScore { get; set; }

        [DataMember]
        public string scheduledAt { get; set; }

        [DataMember]
        public string state { get; set; }

        [DataMember]
        public string testplayAt { get; set; }

        [DataMember]
        public MapTestplay[] testplays { get; set; }

        public MapVersion()
        {
            coverURL = "";
            createdAt = "";
            diffs = new MapDifficulty[0];
            downloadURL = "";
            feedback = "";
            hash = "";
            key = "";
            previewURL = "";
            scheduledAt = "";
            state = "";
            testplayAt = "";
            testplays = new MapTestplay[0];
        }
    }

    [DataContract]
    public class MapDifficulty
    {
        [DataMember]
        public int bombs { get; set; }

        [DataMember]
        public string characteristic { get; set; }

        [DataMember]
        public bool chroma { get; set; }

        [DataMember]
        public bool cinema { get; set; }

        [DataMember]
        public string difficulty { get; set; }

        [DataMember]
        public int events { get; set; }

        [DataMember]
        public double length { get; set; }

        [DataMember]
        public bool me { get; set; }

        [DataMember]
        public bool ne { get; set; }

        [DataMember]
        public double njs { get; set; }

        [DataMember]
        public int notes { get; set; }

        [DataMember]
        public double nps { get; set; }

        [DataMember]
        public int obstacles { get; set; }

        [DataMember]
        public double offset { get; set; }

        [DataMember]
        public MapParitySummary paritySummary { get; set; }

        [DataMember]
        public double seconds { get; set; }

        [DataMember]
        public double stars { get; set; }

        [IgnoreDataMember]
        public int difficultyRawInt { get; set; }

        public MapDifficulty()
        {
            characteristic = "";
            difficulty = "";
            paritySummary = new MapParitySummary();
        }

        private enum ECharacteristic
        {
            _UnKnown = 0,
            _Standard = 1,
            _OneSaber = 2,
            _NoArrows = 3,
            _90Degree = 4,
            _360Degree = 5,
            _Lightshow = 6,
            _Lawless = 7,
        }

        private enum EDifficulty
        {
            UnKnown = 0,
            Easy = 1,
            Normal = 3,
            Hard = 5,
            Expert = 7,
            ExpertPlus = 9,
        }

        static public int ToDifficultyRawInt(string characteristic, string difficulty)
        {
            ECharacteristic ec = ECharacteristic._UnKnown;
            _ = Enum.TryParse("_" + characteristic, out ec);

            EDifficulty ed = EDifficulty.UnKnown;
            _ = Enum.TryParse(difficulty, out ed);

            return (int)ec * 32 + (int)ed;
        }
    }

    [DataContract]
    public class MapParitySummary
    {
        [DataMember]
        public int errors { get; set; }

        [DataMember]
        public int resets { get; set; }

        [DataMember]
        public int warns { get; set; }
    }

    [DataContract]
    public class MapTestplay
    {
        [DataMember]
        public string createdAt { get; set; }

        [DataMember]
        public string feedback { get; set; }

        [DataMember]
        public string feedbackAt { get; set; }

        [DataMember]
        public UserDetail user { get; set; }

        [DataMember]
        public string video { get; set; }

        public MapTestplay()
        {
            createdAt = "";
            feedback = "";
            feedbackAt = "";
            user = new UserDetail();
            video = "";
        }
    }
}
