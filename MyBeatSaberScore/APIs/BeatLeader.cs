using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatLeader
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static async Task<LeaderboardsResponse> GetLeaderboardsByHash(string hash)
        {
            string url = $"https://api.beatleader.xyz/leaderboards/hash/{hash}";

            try
            {
                var result = await HttpTool.GetAndDeserialize<LeaderboardsResponse>(url);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new LeaderboardsResponse();
        }

        public static string GetLeaderboardId(LeaderboardsResponse response, BeatMapDifficulty difficulty, BeatMapMode mode)
        {
            string? id = response.leaderboards.Find(l => l.difficulty.mapDifficulty == difficulty && l.difficulty.mapMode == mode)?.id;
            return id ?? "";
        }

        public class LeaderboardsResponse
        {
            public List<LeaderboardsInfoResponse> leaderboards { get; set; } = new();
        }

        public class LeaderboardsInfoResponse
        {
            public string id { get; set; } = "";
            public DifficultyDescription difficulty { get; set; } = new();
        }

        public class DifficultyDescription
        {
            public int id { get; set; }
            public int value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    mapDifficulty = value switch
                    {
                        1 => BeatMapDifficulty.Easy,
                        3 => BeatMapDifficulty.Normal,
                        5 => BeatMapDifficulty.Hard,
                        7 => BeatMapDifficulty.Expert,
                        9 => BeatMapDifficulty.ExpertPlus,
                        _ => BeatMapDifficulty.Unknown,
                    };
                }
            }
            public int mode
            {
                get
                {
                    return _mode;
                }
                set
                {
                    _mode = value;
                    mapMode = value switch
                    {
                        1 => BeatMapMode.Standard,
                        2 => BeatMapMode.OneSaber,
                        3 => BeatMapMode.NoArrows,
                        4 => BeatMapMode.Degree90,
                        5 => BeatMapMode.Degree360,
                        6 => BeatMapMode.Lightshow,
                        7 => BeatMapMode.Lawless,
                        _ => BeatMapMode.Unknown,
                    };
                }
            }

            private int _value;
            private int _mode;

            internal BeatMapDifficulty mapDifficulty;
            internal BeatMapMode mapMode;
        }
    }
}
