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
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。

        private static readonly HttpClient _client = new();
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

        public BeatSaberScrappedData.MapInfo GetMapInfo(string hash)
        {
            if (_mapDic.TryGetValue(hash, out var map))
            {
                return map;
            }
            return new BeatSaberScrappedData.MapInfo();
        }

        public string GetAlleadyKey(string hash)
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

            try
            {
                HttpResponseMessage res = await _client.GetAsync(url);
                using var fileStream = File.Create(_localPath);
                using var httpStream = await res.Content.ReadAsStreamAsync();
                httpStream.CopyTo(fileStream);
                fileStream.Flush();
                return _localPath;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
                return System.IO.Path.Combine(_coverDir, "_404.png");
            }
        }
    }
}
