﻿using MyBeatSaberScore.APIs;
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

        public Dictionary<string, BeatSaver.MapDetail> _acquiredMaps = new();
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
                var collection = JsonSerializer.Deserialize<BeatSaver.MapCollection>(jsonString);
                if (collection != null)
                {
                    collection.Normalize();
                    collection.mapDetails.ForEach(detail =>
                    {
                        if (detail.versions.Count > 0)
                        {
                            _acquiredMaps[detail.versions[0].hash] = detail;
                        }
                    });
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
                var collection = new BeatSaver.MapCollection();
                collection.mapDetails = _acquiredMaps.Values.ToList();
                var jsonString = JsonSerializer.Serialize<BeatSaver.MapCollection>(collection, new JsonSerializerOptions { WriteIndented = true });
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

            var map = await BeatSaver.GetMapDetailByHash(hash);
            if (map.error.Contains("Not Found") == true)
            {
                _deletedMaps.Add(hash);
            }
            else if (map.versions.Count > 0)
            {
                _acquiredMaps[hash] = map;
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