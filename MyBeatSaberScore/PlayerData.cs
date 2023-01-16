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
        private string _historyPath = "";
        private string _profilePath = "";

        public string PlayerId = "";
        public HashSet<string> playedRankHash = new(); // プレイ済みマップのHashSet。キーは「hash + difficulty(1～9)」。
        public Dictionary<long, ScoreSaber.PlayerScore> playedMaps = new(); // プレイ済みマップ。キー「PlayerScore.leaderboard.id」
        public ScoreSaber.PlayerProfile profile = new();
        public bool IsExistProfile = false;
        public AllHistory History = new();

        public PlayerData()
        {
        }

        public void LoadLocalFile(string playerId)
        {
            PlayerId = playerId;

            _userDir = Path.Combine("data", "users", $"{playerId}");
            Directory.CreateDirectory(_userDir);

            _scoresPath = Path.Combine(_userDir, "scores.json");
            _historyPath = Path.Combine(_userDir, "history.json");
            _profilePath = Path.Combine(_userDir, "profile.json");

            playedMaps.Clear();
            playedRankHash.Clear();

            History.Load(_historyPath);

            if (File.Exists(_scoresPath))
            {
                string jsonString = File.ReadAllText(_scoresPath, Encoding.UTF8);
                var collection = JsonSerializer.Deserialize<ScoreSaber.PlayerScoreCollection>(jsonString);
                if (collection != null)
                {
                    collection.playerScores.ForEach(score =>
                    {
                        playedMaps[score.leaderboard.id] = score;
                        History.Add(score);
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
                    IsExistProfile = true;
                }
            }
            else
            {
                profile = new();
                IsExistProfile = false;
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

            History.Save(_historyPath);
        }

        /// <summary>
        /// 最新スコアをダウンロードしてローカルのスコア一覧を更新する
        /// </summary>
        /// <param name="isGetAll">取得対象 false:差分 true:全て</param>
        /// <param name="callback">進捗コールバック</param>
        /// <returns>true:取得成功、false:取得失敗、スコア一覧は更新しない</returns>
        public async Task<bool> DownloadLatestScores(bool isGetAll, Action<int, int> callback)
        {
            if (PlayerId.Length == 0)
            {
                return false;
            }

            callback(100, 1);

            var limit = 100; // １回のAPI呼び出しで取得する件数
            var getResult = ScoreSaber.GetScoresResult.CONTINUE; // スコアセイバーからの取得に成功したかどうか
            List<ScoreSaber.PlayerScoreCollection> collections = new();

            for (var page = 1; getResult == ScoreSaber.GetScoresResult.CONTINUE; page++)
            {
                (getResult, var collection) = await ScoreSaber.GetPlayerScores(PlayerId, limit, page);
                if (getResult == ScoreSaber.GetScoresResult.CONTINUE)
                {
                    if (collection.metadata.total > 0)
                    {
                        callback((int)collection.metadata.total, page * limit);
                    }

                    // 後でまとめて処理するために取得したデータを残しておく
                    collections.Add(collection);

                    if (!isGetAll)
                    {
                        // ローカルに保持していないデータをすべて取得できたか確認する
                        foreach (var score in collection.playerScores)
                        {
                            // 更新日が同じデータがローカルにあればすべて取得できた
                            if (playedMaps.TryGetValue(score.leaderboard.id, out var played))
                            {
                                if (played.score.timeSet == score.score.timeSet)
                                {
                                    getResult = ScoreSaber.GetScoresResult.FINISH;
                                }
                            }
                        }
                    }
                }
            }

            // 取得対象のデータをすべて取得できた場合はローカルデータを更新する
            if (getResult == ScoreSaber.GetScoresResult.FINISH)
            {
                collections.ForEach(collection =>
                {
                    collection.playerScores.ForEach(score =>
                    {
                        playedMaps[score.leaderboard.id] = score;
                        History.Add(score);

                        if (score.leaderboard.ranked)
                        {
                            playedRankHash.Add(score.leaderboard.songHash + score.leaderboard.difficulty.difficulty);
                        }
                    });
                });
            }

            callback(100, 100);

            return true;
        }

        public async Task<ScoreSaber.PlayerProfile> GetPlayerProfile()
        {
            profile = await ScoreSaber.GetPlayerInfo(PlayerId) ?? new ScoreSaber.PlayerProfile();
            var jsonString = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_profilePath, jsonString);
            IsExistProfile = true;
            return profile;
        }

        public ScoreSaber.PlayerProfile GetPlayerProfileFromLocal()
        {
            return profile;
        }

        public class PlayResult
        {
            public long id { get; set; }

            public long leaderboardId { get; set; }

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

            public DateTime timeSet { get; set; }
        }

        public class DifficultyResults
        {
            readonly SortedList<DateTime, PlayResult> _results = new();

            public void Add(PlayResult result)
            {
                _ = _results.TryAdd(result.timeSet, result);
            }

            public int Count { get { return _results.Count; } }

            public long LatestChange()
            {
                if (_results.Count > 1)
                {
                    return _results.Values[^1].modifiedScore - _results.Values[^2].modifiedScore;
                }
                else if (_results.Count == 1)
                {
                    return _results.Values[0].modifiedScore;
                }
                else
                {
                    return 0;
                }
            }

            public bool IsFirstClear()
            {
                if (_results.Count > 1)
                {
                    return (!IsFailre(_results.Values[^1].modifiers) && IsFailre(_results.Values[^2].modifiers));
                }
                else if (_results.Count == 1)
                {
                    return !IsFailre(_results.Values[0].modifiers);
                }
                else
                {
                    return false;
                }
            }

            private bool IsFailre(string modifiers)
            {
                if (modifiers.Length > 0)
                {
                    foreach (var f in Config.Failures)
                    {
                        if (modifiers.Contains(f))
                        {
                            return true;
                        }
                    }
                }
                return false;

            }
        }

        public class AllHistory
        {
            readonly Dictionary<long, DifficultyResults> _resultsByLeaderboardId = new();
            readonly SortedList<DateTime, PlayResult> _allResults = new();

            public void Load(string path)
            {
                _resultsByLeaderboardId.Clear();
                _allResults.Clear();

                if (File.Exists(path))
                {
                    string jsonString = File.ReadAllText(path, Encoding.UTF8);
                    var list = JsonSerializer.Deserialize<List<PlayResult>>(jsonString);
                    if (list != null)
                    {
                        list.ForEach(result => Add(result));
                    }
                }
            }

            public void Save(string path)
            {
                var jsonString = JsonSerializer.Serialize(_allResults.Values, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, jsonString);
            }

            public void Add(PlayResult result)
            {
                if (!_resultsByLeaderboardId.ContainsKey(result.leaderboardId))
                {
                    _resultsByLeaderboardId[result.leaderboardId] = new();
                }
                _resultsByLeaderboardId[result.leaderboardId].Add(result);
                _ = _allResults.TryAdd(result.timeSet, result);
            }

            public void Add(ScoreSaber.PlayerScore score)
            {
                Add(new PlayResult
                {
                    id = score.score.id,
                    leaderboardId = score.leaderboard.id,
                    rank = score.score.rank,
                    baseScore = score.score.baseScore,
                    modifiedScore = score.score.modifiedScore,
                    pp = score.score.pp,
                    weight = score.score.weight,
                    modifiers = score.score.modifiers,
                    multiplier = score.score.multiplier,
                    badCuts = score.score.badCuts,
                    missedNotes = score.score.missedNotes,
                    maxCombo = score.score.maxCombo,
                    fullCombo = score.score.fullCombo,
                    hmd = score.score.hmd,
                    hasReplay = score.score.hasReplay,
                    timeSet = score.score.timeSet
                });
            }

            public DifficultyResults GetDifficultyResults(long leaderboardId)
            {
                if (_resultsByLeaderboardId.ContainsKey(leaderboardId))
                {
                    return _resultsByLeaderboardId[leaderboardId];
                }
                return new();
            }
        }
    }
}
