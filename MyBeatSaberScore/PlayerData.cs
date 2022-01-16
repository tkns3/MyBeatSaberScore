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
        private string _profilePath = "";

        public string PlayerId = "";
        public HashSet<string> playedRankHash = new(); // プレイ済みマップのHashSet。キーは「hash + difficulty(1～9)」。
        public Dictionary<long, ScoreSaber.PlayerScore> playedMaps = new(); // プレイ済みマップ。キー「PlayerScore.leaderboard.id」
        public ScoreSaber.PlayerProfile profile = new();

        public PlayerData()
        {
        }

        public void LoadLocalFile(string playerId)
        {
            PlayerId = playerId;

            _userDir = Path.Combine("data", "users", $"{playerId}");
            Directory.CreateDirectory(_userDir);

            _scoresPath = Path.Combine(_userDir, "scores.json");
            _profilePath = Path.Combine(_userDir, "profile.json");

            playedMaps.Clear();
            playedRankHash.Clear();

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

            if (File.Exists(_profilePath))
            {
                string jsonString = File.ReadAllText(_profilePath, Encoding.UTF8);
                var tmp = JsonSerializer.Deserialize<ScoreSaber.PlayerProfile>(jsonString);
                if (tmp != null)
                {
                    profile = tmp;
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

        /// <summary>
        /// 最新スコアをダウンロードしてローカルのスコア一覧を更新する
        /// </summary>
        /// <param name="callback">進捗コールバック</param>
        /// <returns>true:取得成功、false:取得失敗、スコア一覧は更新しない</returns>
        public async Task<bool> DownloadLatestScores(Action<int, int> callback)
        {
            if (PlayerId.Length == 0)
            {
                return false;
            }

            callback(100, 1);

            var limit = 100; // １回のAPI呼び出しで取得する件数
            var isAllGet = false; // ローカルに保持していないデータを全て取得できたかどうか
            var isGetSuccess = true; // スコアセイバーからの取得に成功したかどうか
            List<ScoreSaber.PlayerScoreCollection> collections = new();

            for (var page = 1; !isAllGet && isGetSuccess; page++)
            {
                (isGetSuccess, var collection) = await ScoreSaber.GetPlayerScores(PlayerId, limit, page);
                if (isGetSuccess)
                {
                    if (collection.metadata.total > 0)
                    {
                        callback((int)collection.metadata.total, page * limit);
                    }

                    // 後でまとめて処理するために取得したデータを残しておく
                    collections.Add(collection);

                    // ローカルに保持していないデータをすべて取得できたか確認する
                    {
                        foreach (var score in collection.playerScores)
                        {
                            // 更新日が同じデータがローカルにあればすべて取得できた
                            if (playedMaps.TryGetValue(score.leaderboard.id, out var played))
                            {
                                isAllGet = played.score.timeSet == score.score.timeSet;
                            }
                        }
                        // limit件未満の場合はすべて取得できた
                        if (collection.playerScores.Count < limit)
                        {
                            isAllGet = true;
                        }
                    }
                }
            }

            // ローカルに保持していないデータをすべて取得できた場合はローカルデータを更新する
            if (isGetSuccess)
            {
                collections.ForEach(collection =>
                {
                    collection.playerScores.ForEach(score =>
                    {
                        playedMaps[score.leaderboard.id] = score;

                        if (score.leaderboard.ranked)
                        {
                            playedRankHash.Add(score.leaderboard.songHash + score.leaderboard.difficulty.difficulty);
                        }
                    });
                });
            }

            callback(100, 100);

            return isGetSuccess;
        }

        public async Task<ScoreSaber.PlayerProfile> GetPlayerProfile()
        {
            profile = await ScoreSaber.GetPlayerInfo(PlayerId) ?? new ScoreSaber.PlayerProfile();
            var jsonString = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_profilePath, jsonString);
            return profile;
        }

        public ScoreSaber.PlayerProfile GetPlayerProfileFromLocal()
        {
            return profile;
        }
    }
}
