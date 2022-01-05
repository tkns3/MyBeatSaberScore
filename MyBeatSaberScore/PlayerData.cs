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
using MyBeatSaberScore.APIs;

namespace MyBeatSaberScore
{
    internal class PlayerData
    {
        private string _userDir = "";
        private string _scoresPath = "";

        public string PlayerId = "";
        public HashSet<string> playedRankHash = new(); // プレイ済みマップのHashSet。キーは「hash + difficulty(1～9)」。
        public Dictionary<int, ScoreSaber.PlayerScore> playedMaps = new(); // プレイ済みマップ。キー「PlayerScore.leaderboard.id」

        public PlayerData()
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
                var collection = JsonSerializer.Deserialize<ScoreSaber.PlayerScoreCollection>(jsonString);
                if (collection != null)
                {
                    collection.Normalize();
                    collection.playerScores.ForEach(score =>
                    {
                        playedMaps[score.leaderboard.id] = score;
                    });
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

            var collection = new ScoreSaber.PlayerScoreCollection()
            {
                playerScores = playedMaps.Values.ToList()
            };
            collection.metadata.total = playedMaps.Values.Count;

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
                var collection = await ScoreSaber.GetPlayerScores(playerId, 100, page);

                count++;
                callback((collection.metadata.total + 100) / 100, count);

                foreach (var score in collection.playerScores)
                {
                    // 更新日が同じデータであれば更新済みデータはすべて取得済み
                    if (playedMaps.ContainsKey(score.leaderboard.id))
                    {
                        isAllGet = playedMaps[score.leaderboard.id].score.timeSet == score.score.timeSet;
                    }
                    playedMaps[score.leaderboard.id] = score;
                }
                // 100件以下の場合は全データ取得済み
                if (collection.playerScores.Count < 100)
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
    }
}
