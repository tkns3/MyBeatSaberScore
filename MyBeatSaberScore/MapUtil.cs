using MyBeatSaberScore.APIs;
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
    internal class MapUtil
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _mapsPath = Path.Combine(_mapsDir, "maps.json");
        private static readonly string _coverDir = Path.Combine(_mapsDir, "cover");
        private static readonly string _deletedmapsPath = Path.Combine(_mapsDir, "deletedmaps.json");
        private static readonly string _rankedPath = Path.Combine(_mapsDir, "ranked.json");

        public AcquiredMapCollection _mapCollection = new();
        private HashSet<string> _deletedMaps = new();

        public BeatSavior.RankedMapCollection rankedMapCollection = new();
        public HashSet<string> rankedMapHashSet = new(); // ランクマップのHashSet。キーは「hash + difficulty(1～9)」。

        public MapUtil()
        {
            Directory.CreateDirectory(_mapsDir);
            Directory.CreateDirectory(_coverDir);
        }

        public void LoadLocalFile()
        {
            if (File.Exists(_mapsPath))
            {
                string jsonString = File.ReadAllText(_mapsPath, Encoding.UTF8);
                var collection = JsonSerializer.Deserialize<AcquiredMapCollection>(jsonString);
                if (collection != null)
                {
                    collection.Normalize();
                    _mapCollection = collection;
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
                var jsonString = JsonSerializer.Serialize<AcquiredMapCollection>(_mapCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_mapsPath, jsonString);
            }
            {
                var collection = new DeletedMapCollection();
                collection.hashs = _deletedMaps.ToArray();
                var jsonString = JsonSerializer.Serialize<DeletedMapCollection>(collection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_deletedmapsPath, jsonString);
            }
        }

        public void LoadLocalRankedFile()
        {
            try
            {
                if (File.Exists(_rankedPath))
                {
                    string jsonString = File.ReadAllText(_rankedPath, Encoding.UTF8);
                    var collection = JsonSerializer.Deserialize<BeatSavior.RankedMapCollection>(jsonString);
                    if (collection != null)
                    {
                        rankedMapHashSet.Clear();
                        foreach (var map in collection.maps)
                        {
                            map.hash = map.hash.ToLower();
                            map.coverURL = "https://cdn.scoresaber.com/covers/" + map.hash.ToUpper() + ".png";
                            if (map.diffs.easy != null) rankedMapHashSet.Add(map.hash + "1");
                            if (map.diffs.normal != null) rankedMapHashSet.Add(map.hash + "3");
                            if (map.diffs.hard != null) rankedMapHashSet.Add(map.hash + "5");
                            if (map.diffs.expert != null) rankedMapHashSet.Add(map.hash + "7");
                            if (map.diffs.expertplus != null) rankedMapHashSet.Add(map.hash + "9");
                        }
                        rankedMapCollection = collection;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }
        }

        public void SaveLocalRankedFile()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize<BeatSavior.RankedMapCollection>(rankedMapCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_rankedPath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }
        }

        public int GetMaxScore(string hash, int difficultyRawInt)
        {
            if (!_mapCollection.maps.ContainsKey(hash))
            {
                return 0;
            }

            var detail = _mapCollection.maps[hash];
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

            if (_mapCollection.maps.ContainsKey(hash))
            {
                return _mapCollection.maps[hash].id;
            }

            return "";
        }

        public async Task<string> GetKey(string hash)
        {
            hash = hash.ToLower();

            if (_mapCollection.maps.ContainsKey(hash))
            {
                return _mapCollection.maps[hash].id;
            }

            if (_deletedMaps.Contains(hash))
            {
                return "";
            }

            var map = await BeatSaver.GetMapDetailByHash(hash);
            if (map.error.Contains("Not Found") == true)
            {
                _deletedMaps.Add(hash);
            }
            else if (map.versions.Count > 0)
            {
                _mapCollection.maps[hash] = map;
                return map.id;
            }
            return "";
        }

        public static string GetCoverLocalPath(ScoreSaber.PlayerScore score)
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

        public static async Task<string> GetCover(ScoreSaber.PlayerScore score)
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

        public async Task DownloadRankedMaps()
        {
            rankedMapCollection = await BeatSavior.GetRankedMaps();

            rankedMapHashSet.Clear();
            foreach (var map in rankedMapCollection.maps)
            {
                if (map.diffs.easy != null) rankedMapHashSet.Add(map.hash + "1");
                if (map.diffs.normal != null) rankedMapHashSet.Add(map.hash + "3");
                if (map.diffs.hard != null) rankedMapHashSet.Add(map.hash + "5");
                if (map.diffs.expert != null) rankedMapHashSet.Add(map.hash + "7");
                if (map.diffs.expertplus != null) rankedMapHashSet.Add(map.hash + "9");
            }
        }

        [DataContract]
        public class AcquiredMapCollection
        {
            [DataMember]
            public string version { get; set; }

            [DataMember]
            public Dictionary<string, BeatSaver.MapDetail> maps { get; set; }

            public AcquiredMapCollection()
            {
                version = "2";
                maps = new();
            }

            public void Normalize()
            {
                if (version == null)
                {
                    version = "2";
                }
                if (maps == null)
                {
                    maps = new();
                }
                foreach (var map in maps)
                {
                    map.Value.Normalize();
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

    }
}
