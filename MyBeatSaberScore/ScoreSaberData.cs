using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeatSaberScore
{
    internal class ScoreSaberData
    {
        private static readonly HttpClient _client = new HttpClient();
        private string _userDir = "";
        private string _scoresPath = "";

        public string PlayerId = "";
        public HashSet<string> playedRankHash = new(); // プレイ済みマップのHashSet。キーは「hash + difficulty(1～9)」。
        public Dictionary<int, PlayerScore> playedMaps = new(); // プレイ済みマップ。キー「PlayerScore.leaderboard.id」

        public ScoreSaberData()
        {
        }

        public void LoadLocalFile(string playerId)
        {
            PlayerId = playerId;

            _userDir = Path.Combine("data", "users", $"{playerId}");
            Directory.CreateDirectory(_userDir);

            _scoresPath = Path.Combine(_userDir, "scores.json");

            playedMaps.Clear();

            if (File.Exists(_scoresPath))
            {
                string jsonString = File.ReadAllText(_scoresPath, Encoding.UTF8);
                var collection = JsonSerializer.Deserialize<PlayerScoreCollection>(jsonString);
                if (collection != null)
                {
                    foreach (var score in collection.playerScores)
                    {
                        score.leaderboard.songHash = score.leaderboard.songHash.ToLower();
                        score.leaderboard.difficulty.difficultyRawInt = Difficulty.ToDifficultyRawInt(score.leaderboard.difficulty.gameMode, score.leaderboard.difficulty.difficulty);
                        playedMaps[score.leaderboard.id] = score;
                    }
                }
            }

            foreach (var score in playedMaps.Values)
            {
                if (score.leaderboard.ranked)
                {
                    playedRankHash.Add(score.leaderboard.songHash + score.leaderboard.difficulty.difficulty);
                }
            }
        }

        public void SaveLocalFile()
        {
            if (PlayerId.Length == 0) return;

            var collection = new PlayerScoreCollection();
            collection.playerScores = playedMaps.Values.ToArray();
            collection.metadata.total = collection.playerScores.Length;
            var jsonString = JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_scoresPath, jsonString);
        }

        public async Task DownloadLatestScores(Action<int, int> callback)
        {
            if (PlayerId.Length == 0) return;

            int count = 0;
            var playerId = this.PlayerId;
            var isAllGet = false;
            for (var page = 1; !isAllGet; page++)
            {
                var recent = await GetRecentScores(playerId, 100, page);

                count++;
                callback((recent.metadata.total + 100) / 100, count);

                foreach (var score in recent.playerScores)
                {
                    // 更新日が同じデータであれば更新済みデータはすべて取得済み
                    if (playedMaps.ContainsKey(score.leaderboard.id))
                    {
                        isAllGet = playedMaps[score.leaderboard.id].score.timeSet == score.score.timeSet;
                    }
                    score.leaderboard.songHash = score.leaderboard.songHash.ToLower();
                    score.leaderboard.difficulty.difficultyRawInt = Difficulty.ToDifficultyRawInt(score.leaderboard.difficulty.gameMode, score.leaderboard.difficulty.difficulty);
                    playedMaps[score.leaderboard.id] = score;
                }
                // 100件以下の場合は全データ取得済み
                if (recent.playerScores.Length < 100)
                {
                    isAllGet = true;
                }
            }

            playedRankHash.Clear();
            foreach (var score in playedMaps.Values)
            {
                if (score.leaderboard.ranked)
                {
                    playedRankHash.Add(score.leaderboard.songHash + score.leaderboard.difficulty.difficulty);
                }
            }
        }

        public static async Task<PlayerScoreCollection> GetRecentScores(string playerId, int limit, int page)
        {
            string url = $"https://scoresaber.com/api/player/{playerId}/scores?sort=recent&limit={limit}&page={page}";

            try
            {
                var httpsResponse = await _client.GetAsync(url);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PlayerScoreCollection>(responseContent);

                if (result?.playerScores?.Length > 0)
                {
                    foreach (var score in result.playerScores)
                    {
                        score.leaderboard.songHash = score.leaderboard.songHash.ToLower();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + url);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }

            return new PlayerScoreCollection();
        }
    }

    [DataContract]
    public class PlayerScoreCollection
    {
        [DataMember]
        public PlayerScore[] playerScores { get; set; }

        [DataMember]
        public Metadata metadata { get; set; }

        public PlayerScoreCollection()
        {
            playerScores = new PlayerScore[0];
            metadata = new Metadata();
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
    }

    [DataContract]
    public class Score
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public LeaderboardPlayerInfo leaderboardPlayerInfo { get; set; }

        [DataMember]
        public int rank { get; set; }

        [DataMember]
        public int baseScore { get; set; }

        [DataMember]
        public int modifiedScore { get; set; }

        [DataMember]
        public double pp { get; set; }

        [DataMember]
        public double weight { get; set; }

        [DataMember]
        public string modifiers { get; set; }

        [DataMember]
        public double multiplier { get; set; }

        [DataMember]
        public int badCuts { get; set; }

        [DataMember]
        public int missedNotes { get; set; }

        [DataMember]
        public int maxCombo { get; set; }

        [DataMember]
        public bool fullCombo { get; set; }

        [DataMember]
        public int hmd { get; set; }

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
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string profilePicture { get; set; }

        [DataMember]
        public string country { get; set; }

        [DataMember]
        public int permissions { get; set; }

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
        public int id { get; set; }

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
        public int maxScore { get; set; }

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
        public int maxPP { get; set; }

        [DataMember]
        public double stars { get; set; }

        [DataMember]
        public bool positiveModifiers { get; set; }

        [DataMember]
        public int plays { get; set; }

        [DataMember]
        public int dailyPlays { get; set; }

        [DataMember]
        public string coverImage { get; set; }

        [DataMember]
        public Score? playerScore { get; set; }

        [DataMember]
        public Difficulty[]? difficulties { get; set; }

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
            difficulties = new Difficulty[0];
        }
    }

    public class Difficulty
    {
        [DataMember]
        public int leaderboardId { get; set; }

        [DataMember]
        public int difficulty { get; set; }

        [DataMember]
        public string gameMode { get; set; }

        [DataMember]
        public string difficultyRaw { get; set; }

        [IgnoreDataMember]
        public int difficultyRawInt { get; set; }

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

        static public int ToDifficultyRawInt(string gameMode, int difficulty)
        {
            EGameMode egm = EGameMode._UnKnown;
            _ = Enum.TryParse(gameMode, out egm);

            return (int)egm * 32 + difficulty;
        }
    }

    [DataContract]
    public class Metadata
    {
        [DataMember]
        public int total { get; set; }

        [DataMember]
        public int page { get; set; }

        [DataMember]
        public int itemsPerPage { get; set; }
    }
}
