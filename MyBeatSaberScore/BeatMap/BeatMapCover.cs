using MyBeatSaberScore.APIs;
using MyBeatSaberScore.Utility;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MyBeatSaberScore.BeatMap
{
    internal static class BeatMapCover
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _coverDir = Path.Combine(_mapsDir, "cover");

        public static void Initialize()
        {
            Directory.CreateDirectory(_mapsDir);
            Directory.CreateDirectory(_coverDir);
        }

        public static bool IsExistCoverAtLocal(string hash)
        {
            hash = hash.ToLower();
            var _localPath = Path.Combine(Environment.CurrentDirectory, _coverDir, $"{hash}.png");
            return File.Exists(_localPath);
        }

        public static string GetCoverLocalPath(ScoreSaber.PlayerScore score)
        {
            return GetCoverLocalPath(score.leaderboard.songHash);
        }

        public static string GetCoverLocalPath(string hash)
        {
            hash = hash.ToLower();

            var _localPath = Path.Combine(Environment.CurrentDirectory, _coverDir, $"{hash}.png");
            if (File.Exists(_localPath))
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

            var _localPath = Path.Combine(_coverDir, $"{hash}.png");
            if (File.Exists(_localPath))
            {
                return _localPath;
            }

            return await DownloadCover(url, _localPath);
        }

        private static async Task<string> DownloadCover(string url, string localPath)
        {
            try
            {
                await HttpTool.Download(url, localPath);
                return localPath;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
                return Path.Combine(_coverDir, "_404.png");
            }
        }
    }
}
