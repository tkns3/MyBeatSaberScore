using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class ScoreSaber
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public enum GetScoresResult
        {
            FAIL,
            CONTINUE,
            FINISH,
        }

        public static async Task<(GetScoresResult, PlayerScoreCollection)> GetPlayerScores(string playerId, int limit, int page)
        {
            string url = $"https://scoresaber.com/api/player/{playerId}/scores?sort=recent&limit={limit}&page={page}";

            try
            {
                var httpsResponse = await HttpTool.Client.GetAsync(url);

                // metadata.totalと取得できるデータ数が一致しないことがある。
                // そのため全てのデータを取得できたかの判定にmetadata.totalを使えない。
                // データがないページまで到達した(404 NotFoundが返ってくる)場合は成功扱いにする。
                if (httpsResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (GetScoresResult.FINISH, new PlayerScoreCollection());
                }

                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var collection = JsonSerializer.Deserialize<PlayerScoreCollection>(responseContent);

                if (collection?.playerScores?.Count > 0)
                {
                    return (GetScoresResult.CONTINUE, collection);
                }

                // [2022/10/12]
                // APIの仕様が変わったのか最終ページの次を指定すると404 Not Foundではなく200 OKのCount=0が返ってくるようになっている
                // 200 OKのCount=0も終了扱いにする
                if (collection?.playerScores?.Count == 0)
                {
                    return (GetScoresResult.FINISH, new PlayerScoreCollection());
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return (GetScoresResult.FAIL, new PlayerScoreCollection());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minStar"></param>
        /// <param name="maxStar"></param>
        /// <param name="page">1～</param>
        /// <param name="category">0:trending, 1:date ranked, 2:scores set, 3:star difficulty, 4:author (default 1)</param>
        /// <param name="sort">0:descending, 1:ascending (default 0)</param>
        /// <param name="ranked">0:false, 1:true (default 1)</param>
        /// <param name="verified">0:false, 1:true (defult 1)</param>
        /// <returns></returns>
        public static async Task<LeaderboardInfoCollection> GetLeaderboards(int minStar, int maxStar, int page, int category = 1, int sort = 0, int ranked = 1, int verified = 1)
        {
            string url = $"https://scoresaber.com/api/leaderboards?category={category}&maxStar={maxStar}&minStar={minStar}&page={page}&ranked={ranked}&sort={sort}&verified={verified}";

            try
            {
                var result = await HttpTool.GetAndDeserialize<LeaderboardInfoCollection>(url);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new LeaderboardInfoCollection();
        }

        public static async Task<LeaderboardInfo> GetLeaderboard(string hash, BeatMapDifficulty mapDifficulty, BeatMapMode mapMode)
        {
            int difficulty = mapDifficulty switch
            {
                BeatMapDifficulty.Easy => 1,
                BeatMapDifficulty.Normal => 3,
                BeatMapDifficulty.Hard => 5,
                BeatMapDifficulty.Expert => 7,
                BeatMapDifficulty.ExpertPlus => 9,
                _ => 1,
            };
            string mode = mapMode switch
            {
                BeatMapMode.Standard => "SoloStandard",
                BeatMapMode.OneSaber => "SoloOneSaber",
                BeatMapMode.NoArrows => "SoloNoArrows",
                BeatMapMode.Degree90 => "Solo90Degree",
                BeatMapMode.Degree360 => "Solo360Degree",
                BeatMapMode.Lightshow => "SoloLightshow",
                BeatMapMode.Lawless => "SoloLawless",
                _ => "SoloStandard",
            };
            string url = $"https://scoresaber.com/api/leaderboard/by-hash/{hash}/info?difficulty={difficulty}&mode={mode}";

            try
            {
                var result = await HttpTool.GetAndDeserialize<LeaderboardInfo>(url);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new LeaderboardInfo();
        }

        public static async Task<PlayerProfile> GetPlayerInfo(string playerId)
        {
            string url = $"https://scoresaber.com/api/player/{playerId}/full";

            try
            {
                var result = await HttpTool.GetAndDeserialize<PlayerProfile>(url);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new PlayerProfile();
        }

        public class LeaderboardInfoCollection
        {
            public List<LeaderboardInfo> leaderboards { get; set; } = new();
            public Metadata metadata { get; set; } = new();
        }

        public class PlayerScoreCollection
        {
            public List<PlayerScore> playerScores { get; set; } = new();
            public Metadata metadata { get; set; } = new();
        }

        public class PlayerScore
        {
            public Score score { get; set; } = new();
            public LeaderboardInfo leaderboard { get; set; } = new();
        }

        public class Score
        {
            public long id { get; set; }
            public LeaderboardPlayerInfo leaderboardPlayerInfo { get; set; } = new();
            public long rank { get; set; }
            public long baseScore { get; set; }
            public long modifiedScore { get; set; }
            public double pp { get; set; }
            public double weight { get; set; }
            public string modifiers { get; set; } = "";
            public double multiplier { get; set; }
            public long badCuts { get; set; }
            public long missedNotes { get; set; }
            public long maxCombo { get; set; }
            public bool fullCombo { get; set; }
            public long hmd { get; set; }
            public bool hasReplay { get; set; }
            public DateTime timeSet { get; set; } = new();
        }

        public class LeaderboardPlayerInfo
        {
            public long id { get; set; }
            public string name { get; set; } = "";
            public string profilePicture { get; set; } = "";
            public string country { get; set; } = "";
            public long permissions { get; set; }
            public string role { get; set; } = "";
        }

        public class LeaderboardInfo
        {
            public long id { get; set; }
            public string songHash
            {
                get
                {
                    return _songHash;
                }
                set
                {
                    _songHash = value.ToLower();
                }
            }
            public string songName { get; set; } = "";
            public string songSubName { get; set; } = "";
            public string songAuthorName { get; set; } = "";
            public string levelAuthorName { get; set; } = "";
            public Difficulty difficulty { get; set; } = new();
            public long maxScore { get; set; }
            public DateTime createdDate { get; set; }
            public DateTime? rankedDate { get; set; }
            public DateTime? qualifiedDate { get; set; }
            public DateTime? lovedDate { get; set; }
            public bool ranked { get; set; }
            public bool qualified { get; set; }
            public bool loved { get; set; }
            public long maxPP { get; set; }
            public double stars { get; set; }
            public bool positiveModifiers { get; set; }
            public long plays { get; set; }
            public long dailyPlays { get; set; }
            public string coverImage { get; set; } = "";
            public Score? playerScore { get; set; }
            public List<Difficulty> difficulties { get; set; } = new();

            private string _songHash = "";
        }

        public class Difficulty
        {
            public long leaderboardId { get; set; }
            public long difficulty
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
                        1 => BeatMapDifficulty.Easy,
                        3 => BeatMapDifficulty.Normal,
                        5 => BeatMapDifficulty.Hard,
                        7 => BeatMapDifficulty.Expert,
                        9 => BeatMapDifficulty.ExpertPlus,
                        _ => BeatMapDifficulty.Unknown,
                    };
                }
            }
            public string gameMode
            {
                get
                {
                    return _gameMode;
                }
                set
                {
                    _gameMode = value;
                    mapMode = value switch
                    {
                        "SoloStandard" => BeatMapMode.Standard,
                        "SoloOneSaber" => BeatMapMode.OneSaber,
                        "SoloNoArrows" => BeatMapMode.NoArrows,
                        "SoloLightshow" => BeatMapMode.Lightshow,
                        "Solo90Degree" => BeatMapMode.Degree90,
                        "Solo360Degree" => BeatMapMode.Degree360,
                        "SoloLawless" => BeatMapMode.Lawless,
                        _ => BeatMapMode.Unknown,
                    };
                }
            }
            public string difficultyRaw { get; set; } = "";

            private string _gameMode = "";
            private long _difficulty;

            internal BeatMapMode mapMode;
            internal BeatMapDifficulty mapDifficulty;
        }

        public class Metadata
        {
            public long total { get; set; }
            public long page { get; set; }
            public long itemsPerPage { get; set; }
        }

        public class PlayerProfile
        {
            public string id { get; set; } = "";
            public string name { get; set; } = "";
            public string profilePicture { get; set; } = "Resources/_404.png";
            public string country { get; set; } = "";
            public double pp { get; set; }
            public long rank { get; set; }
            public long countryRank { get; set; }
            public string role { get; set; } = "";
            public string histories { get; set; } = "";
            public double permissions { get; set; }
            public bool banned { get; set; }
            public bool inactive { get; set; }
            public ScoreStats scoreStats { get; set; } = new();
        }

        public class ScoreStats
        {
            public long totalScore { get; set; }
            public long totalRankedScore { get; set; }
            public double averageRankedAccuracy { get; set; }
            public long totalPlayCount { get; set; }
            public long rankedPlayCount { get; set; }
            public long replaysWatched { get; set; }
        }
    }
}
