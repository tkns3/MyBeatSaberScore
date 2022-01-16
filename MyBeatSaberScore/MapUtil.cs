using MyBeatSaberScore.APIs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        private static readonly string _coverDir = Path.Combine(_mapsDir, "cover");

        public List<BeatSaberScrappedData.MapInfo> _mapList = new();
        private readonly Dictionary<string, BeatSaberScrappedData.MapInfo> _mapDic = new();

        public MapUtil()
        {
            Directory.CreateDirectory(_mapsDir);
            Directory.CreateDirectory(_coverDir);
        }

        public void LoadLocalFile()
        {
            _mapList = BeatSaberScrappedData.DeserializeCombinedScrappedData();
            _mapList.ForEach(map =>
            {
                map.Diffs.ForEach(diff =>
                {
                    _mapDic[map.Hash] = map;
                });
            });
        }

        public int GetMaxScore(string hash, long difficultyRawInt)
        {
            if (!_mapDic.ContainsKey(hash))
            {
                return 0;
            }

            if (_mapDic.TryGetValue(hash, out var map))
            {
                var diff = map.Diffs.Find(d => d.difficultyRawInt == difficultyRawInt);
                if (diff != null)
                {
                    int[] scoreArray = new int[] { 0, 115, 345, 575, 805, 1035, 1495, 1955, 2415, 2875, 3335, 3795, 4255, 4715 };
                    if (diff.Notes < 14)
                    {
                        return scoreArray[diff.Notes];
                    }
                    else
                    {
                        return 4715 + (diff.Notes - 13) * 920;
                    }
                }
            }
            return 0;
        }

        public string GetAlleadyKey(string hash)
        {
            if (_mapDic.TryGetValue(hash.ToLower(), out var map))
            {
                return map.Key;
            }

            return "";
        }

        public static bool isExistCoverAtLocal(string hash)
        {
            hash = hash.ToLower();
            var _localPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, _coverDir, $"{hash}.png");
            return System.IO.File.Exists(_localPath);
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
    }
}
