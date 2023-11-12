using MyBeatSaberScore.APIs;
using MyBeatSaberScore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeatSaberScore.Model
{
    internal class ScoreSaberUserData
    {
        /// <summary>
        /// ScoreSaberのProfileId
        /// </summary>
        public string ProfileId { get; private set; } = "";

        /// <summary>
        /// ScoreSaberのProfile
        /// </summary>
        public APIs.ScoreSaber.PlayerProfile Profile { get; private set; } = new();

        /// <summary>
        /// Profileをロード済みかどうか
        /// </summary>
        public bool IsExistProfile { get; private set; }

        /// <summary>
        /// プレイ済みマップ。キー「PlayerScore.leaderboard.id」
        /// </summary>
        public Dictionary<long, ScoreSaber.PlayerScore> PlayedScores { get; } = new();

        /// <summary>
        /// プレイ(スコア更新)の履歴
        /// </summary>
        public ScoreSaberPlayHistory PlayHistory { get; private set; } = new();

        /// <summary>
        /// ユーザ用ディレクトリのパス
        /// </summary>
        private string _userDir = "";

        /// <summary>
        /// ScoreSaberから取得した全スコアの保存先パス
        /// </summary>
        private string _scoresPath = "";

        /// <summary>
        /// プレイ履歴の保存先パス
        /// </summary>
        private string _historyPath = "";

        /// <summary>
        /// プロファイルデータの保存先パス
        /// </summary>
        private string _profilePath = "";

        public ScoreSaberUserData()
        {
        }

        public ScoreSaberUserData(string profileId)
        {
            ProfileId = profileId;
            _userDir = Path.Combine("data", "users", $"{ProfileId}");
            _scoresPath = Path.Combine(_userDir, "scores.json");
            _historyPath = Path.Combine(_userDir, "history.json");
            _profilePath = Path.Combine(_userDir, "profile.json");
            IsExistProfile = LoadProfileFromLocalFile();
        }

        private bool LoadProfileFromLocalFile()
        {
            bool result = false;
            if (File.Exists(_profilePath))
            {
                var profile = Json.DeserializeFromLocalFile<ScoreSaber.PlayerProfile>(_profilePath);
                if (profile != null && profile.id.Equals(ProfileId))
                {
                    Profile = profile;
                    result = true;
                }
            }
            Profile.id = ProfileId;
            return result;
        }

        private void LoadScoresFromLocalFile()
        {
            if (File.Exists(_scoresPath))
            {
                var collection = Json.DeserializeFromLocalFile<ScoreSaber.PlayerScoreCollection>(_scoresPath);
                if (collection != null)
                {
                    collection.playerScores.ForEach(score =>
                    {
                        PlayedScores[score.leaderboard.id] = score;
                        PlayHistory.Add(score);
                    });
                }
            }
        }

        public void LoadAllFromLocalFile()
        {
            if (ProfileId.Length == 0) return;

            Profile = new();
            PlayedScores.Clear();
            PlayHistory.Clear();

            IsExistProfile = LoadProfileFromLocalFile();
            PlayHistory.LoadFromLocalFile(_historyPath);
            LoadScoresFromLocalFile();
        }

        public void SaveAllToLocalFile()
        {
            if (ProfileId.Length == 0) return;

            var collection = new ScoreSaber.PlayerScoreCollection()
            {
                playerScores = PlayedScores.Values.ToList()
            };
            collection.metadata.total = PlayedScores.Values.Count;

            Json.SerializeToLocalFile(collection, _scoresPath);

            PlayHistory.SaveToLocalFile(_historyPath);
        }

        public bool FetchLatestProfile()
        {
            var task = FetchLatestProfileAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<bool> FetchLatestProfileAsync()
        {
            var profile = await ScoreSaber.GetPlayerInfo(ProfileId);
            if (profile.id.Length > 0 && profile.id.Equals(ProfileId))
            {
                Profile = profile;
                Json.SerializeToLocalFile(Profile, _profilePath);
                IsExistProfile = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public FetchLatestScoresExecuter FetchLatestScores(bool isGetAll)
        {
            return new FetchLatestScoresExecuter(this, isGetAll);
        }

        public class FetchLatestScoresExecuter : IStepExecuter
        {
            private readonly ScoreSaberUserData _self;
            private int _totalStep;
            private int _finishedStep;
            private bool _isGetAll;
            private ScoreSaber.GetScoresResult _getResult = ScoreSaber.GetScoresResult.CONTINUE; // スコアセイバーからの取得に成功したかどうか
            private List<ScoreSaber.PlayerScoreCollection> _collections = new();
            private int _page = 1;
            private IStepExecuter.Status _status = IStepExecuter.Status.Processing;

            public FetchLatestScoresExecuter(ScoreSaberUserData self, bool isGetAll)
            {
                _self = self;
                _isGetAll = isGetAll;
                var predict = (isGetAll) ? (int)_self.Profile.scoreStats.totalPlayCount : (int)_self.Profile.scoreStats.totalPlayCount - _self.PlayedScores.Count;
                predict = (predict < 0) ? 1 : predict;
                _totalStep = (int)Math.Ceiling((double)predict / 100) + 1; // 通信N回 + 構築1回 ※通信回数は予測なので前後する可能性あり
                _finishedStep = 0;
            }

            public int TotalStepCount { get => _totalStep; }

            public int FinishedStepCount { get => _finishedStep; }

            public IStepExecuter.Status CurrentStatus { get => _status; }

            public IStepExecuter.Status ExecuteStep()
            {
                if (_getResult == ScoreSaber.GetScoresResult.CONTINUE)
                {
                    FetchNextPartScores();
                    _finishedStep = Math.Max(_finishedStep + 1, _totalStep - 1);
                    _status = IStepExecuter.Status.Processing;
                    return _status;
                }
                else if (_getResult == ScoreSaber.GetScoresResult.FINISH)
                {
                    _collections.ForEach(collection =>
                    {
                        collection.playerScores.ForEach(score =>
                        {
                            _self.PlayedScores[score.leaderboard.id] = score;
                            _self.PlayHistory.Add(score);
                        });
                    });

                    _finishedStep = _totalStep;
                    _status = IStepExecuter.Status.Completed;
                    return _status;
                }
                else
                {
                    _status = IStepExecuter.Status.Failed;
                    return _status;
                }
            }

            private void FetchNextPartScores()
            {
                var task = ScoreSaber.GetPlayerScores(_self.ProfileId, limit: 100, page: _page);
                task.Wait();
                (_getResult, var collection) = task.Result;

                if (_getResult == ScoreSaber.GetScoresResult.CONTINUE)
                {
                    // 後でまとめて処理するために取得したデータを残しておく
                    _collections.Add(collection);

                    if (!_isGetAll)
                    {
                        // ローカルに保持していないデータをすべて取得できたか確認する
                        foreach (var score in collection.playerScores)
                        {
                            // 更新日が同じデータがローカルにあればすべて取得できた
                            if (_self.PlayedScores.TryGetValue(score.leaderboard.id, out var played))
                            {
                                if (played.score.timeSet == score.score.timeSet)
                                {
                                    _getResult = ScoreSaber.GetScoresResult.FINISH;
                                }
                            }
                        }
                    }
                }

                _page++;
            }
        }
    }
}
