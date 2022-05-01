using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class ScoreSaber
    {
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。

        private static readonly HttpClient _client = new();

        public static async Task<(bool, PlayerScoreCollection)> GetPlayerScores(string playerId, int limit, int page)
        {
            string url = $"https://scoresaber.com/api/player/{playerId}/scores?sort=recent&limit={limit}&page={page}";

            try
            {
                var httpsResponse = await _client.GetAsync(url);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var collection = JsonSerializer.Deserialize<PlayerScoreCollection>(responseContent);

                if (collection?.playerScores?.Count > 0)
                {
                    collection.playerScores.ForEach(score => score.Normalize());
                    return (true, collection);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return (false, new PlayerScoreCollection());
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
                var httpsResponse = await _client.GetAsync(url);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var collection = JsonSerializer.Deserialize<LeaderboardInfoCollection>(responseContent);

                if (collection?.leaderboards?.Count > 0)
                {
                    collection.Normalize();
                    return collection;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new LeaderboardInfoCollection();
        }

        public static async Task<PlayerProfile> GetPlayerInfo(string playerId)
        {
            string url = $"https://scoresaber.com/api/player/{playerId}/full";

            try
            {
                var httpsResponse = await _client.GetAsync(url);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var collection = JsonSerializer.Deserialize<PlayerProfile>(responseContent);
                return collection ?? new PlayerProfile();
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new PlayerProfile();
        }

        [DataContract]
        public class LeaderboardInfoCollection
        {
            [DataMember]
            public List<LeaderboardInfo> leaderboards { get; set; }

            [DataMember]
            public Metadata metadata { get; set; }

            public LeaderboardInfoCollection()
            {
                leaderboards = new();
                metadata = new Metadata();
            }

            public void Normalize()
            {
                leaderboards.ForEach(leaderboard => leaderboard.Normalize());
            }
        }

        [DataContract]
        public class PlayerScoreCollection
        {
            [DataMember]
            public List<PlayerScore> playerScores { get; set; }

            [DataMember]
            public Metadata metadata { get; set; }

            public PlayerScoreCollection()
            {
                playerScores = new();
                metadata = new Metadata();
            }

            public void Normalize()
            {
                playerScores.ForEach(playerScores => playerScores.Normalize());
            }
        }

        [DataContract]
        public class PlayerScore
        {
            [DataMember]
            public Score score { get; set; }

            [DataMember]
            public LeaderboardInfo leaderboard { get; set; }

            public PlayerScore()
            {
                score = new Score();
                leaderboard = new LeaderboardInfo();
            }

            public void Normalize()
            {
                leaderboard.Normalize();
            }
        }

        [DataContract]
        public class Score
        {
            [DataMember]
            public long id { get; set; }

            [DataMember]
            public LeaderboardPlayerInfo leaderboardPlayerInfo { get; set; }

            [DataMember]
            public long rank { get; set; }

            [DataMember]
            public long baseScore { get; set; }

            [DataMember]
            public long modifiedScore { get; set; }

            [DataMember]
            public double pp { get; set; }

            [DataMember]
            public double weight { get; set; }

            [DataMember]
            public string modifiers { get; set; }

            [DataMember]
            public double multiplier { get; set; }

            [DataMember]
            public long badCuts { get; set; }

            [DataMember]
            public long missedNotes { get; set; }

            [DataMember]
            public long maxCombo { get; set; }

            [DataMember]
            public bool fullCombo { get; set; }

            [DataMember]
            public long hmd { get; set; }

            [DataMember]
            public bool hasReplay { get; set; }

            [DataMember]
            public string timeSet { get; set; }

            public Score()
            {
                leaderboardPlayerInfo = new LeaderboardPlayerInfo();
                modifiers = "";
                timeSet = "";
            }
        }

        public class LeaderboardPlayerInfo
        {
            [DataMember]
            public long id { get; set; }

            [DataMember]
            public string name { get; set; }

            [DataMember]
            public string profilePicture { get; set; }

            [DataMember]
            public string country { get; set; }

            [DataMember]
            public long permissions { get; set; }

            [DataMember]
            public string role { get; set; }

            public LeaderboardPlayerInfo()
            {
                name = "";
                profilePicture = "";
                country = "";
                role = "";
            }
        }

        [DataContract]
        public class LeaderboardInfo
        {
            [DataMember]
            public long id { get; set; }

            [DataMember]
            public string songHash { get; set; }

            [DataMember]
            public string songName { get; set; }

            [DataMember]
            public string songSubName { get; set; }

            [DataMember]
            public string songAuthorName { get; set; }

            [DataMember]
            public string levelAuthorName { get; set; }

            [DataMember]
            public Difficulty difficulty { get; set; }

            [DataMember]
            public long maxScore { get; set; }

            [DataMember]
            public string createdDate { get; set; }

            [DataMember]
            public string? rankedDate { get; set; }

            [DataMember]
            public string? qualifiedDate { get; set; }

            [DataMember]
            public string? lovedDate { get; set; }

            [DataMember]
            public bool ranked { get; set; }

            [DataMember]
            public bool qualified { get; set; }

            [DataMember]
            public bool loved { get; set; }

            [DataMember]
            public long maxPP { get; set; }

            [DataMember]
            public double stars { get; set; }

            [DataMember]
            public bool positiveModifiers { get; set; }

            [DataMember]
            public long plays { get; set; }

            [DataMember]
            public long dailyPlays { get; set; }

            [DataMember]
            public string coverImage { get; set; }

            [DataMember]
            public Score? playerScore { get; set; }

            [DataMember]
            public List<Difficulty> difficulties { get; set; }

            public LeaderboardInfo()
            {
                songHash = "";
                songName = "";
                songSubName = "";
                songAuthorName = "";
                levelAuthorName = "";
                difficulty = new Difficulty();
                createdDate = "";
                coverImage = "";
                difficulties = new();
            }

            public void Normalize()
            {
                if (difficulties == null)
                {
                    difficulties = new();
                }
                songHash = songHash.ToLower();
                difficulty.Normalize();
                foreach (Difficulty diff in difficulties)
                {
                    diff.Normalize();
                }
            }
        }

        public class Difficulty
        {
            [DataMember]
            public long leaderboardId { get; set; }

            [DataMember]
            public long difficulty { get; set; }

            [DataMember]
            public string gameMode { get; set; }

            [DataMember]
            public string difficultyRaw { get; set; }

            [IgnoreDataMember]
            public long difficultyRawInt { get; set; }

            public Difficulty()
            {
                gameMode = "";
                difficultyRaw = "";
            }

            private enum EGameMode
            {
                _UnKnown = 0,
                SoloStandard = 1,
                SoloOneSaber = 2,
                SoloNoArrows = 3,
                Solo90Degree = 4,
                Solo360Degree = 5,
                SoloLightshow = 6,
                SoloLawless = 7,
            }

            public void Normalize()
            {
                difficultyRawInt = ToDifficultyRawInt(gameMode, difficulty);
            }

            static public long ToDifficultyRawInt(string gameMode, long difficulty)
            {
                EGameMode egm = EGameMode._UnKnown;
                _ = Enum.TryParse(gameMode, out egm);

                return (long)egm * 32 + difficulty;
            }
        }

        [DataContract]
        public class Metadata
        {
            [DataMember]
            public long total { get; set; }

            [DataMember]
            public long page { get; set; }

            [DataMember]
            public long itemsPerPage { get; set; }
        }

        [DataContract]
        public class PlayerProfile
        {
            [DataMember]
            public string id { get; set; }

            [DataMember]
            public string name { get; set; }

            [DataMember]
            public string profilePicture { get; set; }

            [DataMember]
            public string country { get; set; }

            [DataMember]
            public double pp { get; set; }

            [DataMember]
            public long rank { get; set; }

            [DataMember]
            public long countryRank { get; set; }

            [DataMember]
            public string role { get; set; }

            [DataMember]
            public string histories { get; set; }

            [DataMember]
            public double permissions { get; set; }

            [DataMember]
            public bool banned { get; set; }

            [DataMember]
            public bool inactive { get; set; }

            [DataMember]
            public ScoreStats scoreStats { get; set; }

            public PlayerProfile()
            {
                id = "";
                name = "";
                profilePicture = "Resources/_404.png";
                country = "";
                role = "";
                histories = "";
                scoreStats = new();
            }
        }

        [DataContract]
        public class ScoreStats
        {
            [DataMember]
            public long totalScore { get; set; }

            [DataMember]
            public long totalRankedScore { get; set; }

            [DataMember]
            public double averageRankedAccuracy { get; set; }

            [DataMember]
            public long totalPlayCount { get; set; }

            [DataMember]
            public long rankedPlayCount { get; set; }

            [DataMember]
            public long replaysWatched { get; set; }

            public ScoreStats()
            {
                //
            }
        }
    }
}
