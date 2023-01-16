using MyBeatSaberScore.APIs;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyBeatSaberScore
{
    internal static class MapUtil
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _coverDir = Path.Combine(_mapsDir, "cover");

        public static List<BeatSaberScrappedData.MapInfo> _mapList = new();
        private static readonly Dictionary<string, BeatSaberScrappedData.MapInfo> _mapDic = new(); // KeyはHASH

        public static void Initialize()
        {
            Directory.CreateDirectory(_mapsDir);
            Directory.CreateDirectory(_coverDir);
            UpdateMapListByScrappedData();
        }

        public static void UpdateMapListByScrappedData()
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

        public static BeatSaberScrappedData.MapInfo GetMapInfo(string hash)
        {
            if (_mapDic.TryGetValue(hash, out var map))
            {
                return map;
            }
            return new BeatSaberScrappedData.MapInfo();
        }

        public static string GetAlleadyKey(string hash)
        {
            if (_mapDic.TryGetValue(hash.ToLower(), out var map))
            {
                return map.Key;
            }

            return "";
        }

        public static bool IsExistCoverAtLocal(string hash)
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

            return await DownloadCover(url, _localPath);
        }

        private static async Task<string> DownloadCover(string url, string localPath)
        {
            try
            {
                HttpResponseMessage res = await HttpTool.Client.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                {
                    _logger.Warn($"{url}: StatusCode={res.StatusCode}");
                    return System.IO.Path.Combine(_coverDir, "_404.png");
                }
                using var fileStream = File.Create(localPath);
                using var httpStream = await res.Content.ReadAsStreamAsync();
                httpStream.CopyTo(fileStream);
                fileStream.Flush();
                return localPath;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
                return System.IO.Path.Combine(_coverDir, "_404.png");
            }
        }

        public static long MaxScore(int notes)
        {
            int[] scoreArray = new int[] { 0, 115, 345, 575, 805, 1035, 1495, 1955, 2415, 2875, 3335, 3795, 4255, 4715 };
            if (notes < 14)
            {
                return scoreArray[notes];
            }
            else
            {
                return 4715 + (notes - 13) * 920;
            }
        }
    }
}
