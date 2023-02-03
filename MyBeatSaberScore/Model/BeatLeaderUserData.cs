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
    internal class BeatLeaderUserData
    {
        /// <summary>
        /// BeatLeaderのProfileId
        /// </summary>
        public string ProfileId { get; private set; } = "";

        /// <summary>
        /// BeatLeaderのProfile
        /// </summary>
        public BeatLeader.PlayerResponseFull Profile { get; private set; } = new();

        /// <summary>
        /// Profileをロード済みかどうか
        /// </summary>
        public bool IsExistProfile { get; private set; } = false;

        /// <summary>
        /// プレイ済みマップ。キー「ScoreResponseWithMyScore.leaderboardId」
        /// </summary>
        public Dictionary<string, BeatLeader.ScoreResponseWithMyScore> PlayedScores { get; } = new();

        /// <summary>
        /// プレイ(スコア更新)の履歴
        /// </summary>
        public BeatLeaderPlayHistory PlayHistory { get; private set; } = new();

        /// <summary>
        /// ユーザ用ディレクトリのパス
        /// </summary>
        private readonly string _userDir = "";

        /// <summary>
        /// ScoreSaberから取得した全スコアの保存先パス
        /// </summary>
        private readonly string _scoresPath = "";

        /// <summary>
        /// プレイ履歴の保存先パス
        /// </summary>
        private readonly string _historyPath = "";

        /// <summary>
        /// プロファイルデータの保存先パス
        /// </summary>
        private readonly string _profilePath = "";

        public BeatLeaderUserData()
        {
        }

        public BeatLeaderUserData(string profileId)
        {
            ProfileId = profileId;
            _userDir = Path.Combine("data", "users", $"{ProfileId}");
            _scoresPath = Path.Combine(_userDir, "bl_scores.json");
            _historyPath = Path.Combine(_userDir, "bl_history.json");
            _profilePath = Path.Combine(_userDir, "bl_profile.json");
            IsExistProfile = LoadProfileFromLocalFile();
        }

        private bool LoadProfileFromLocalFile()
        {
            bool result = false;
            if (File.Exists(_profilePath))
            {
                var tmp = Json.DeserializeFromLocalFile<BeatLeader.PlayerResponseFull>(_profilePath);
                if (tmp != null)
                {
                    Profile = tmp;
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
                var collection = Json.DeserializeFromLocalFile<BeatLeader.ScoreResponseWithMyScoreResponseWithMetadata>(_scoresPath);
                if (collection != null)
                {
                    foreach (var score in collection.data)
                    {
                        PlayedScores[score.leaderboardId] = score;
                        PlayHistory.Add(score);
                    }
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

            var collection = new BeatLeader.ScoreResponseWithMyScoreResponseWithMetadata()
            {
                data = PlayedScores.Values.ToArray()
            };
            collection.metadata.total = PlayedScores.Values.Count;

            Json.SerializeToLocalFile(collection, _scoresPath, Formatting.Indented);

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
            var profile = await BeatLeader.GetPlayerInfo(ProfileId);
            if (profile.id.Length > 0)
            {
                Profile = profile;
                Json.SerializeToLocalFile(Profile, _profilePath, Formatting.Indented);
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
            private readonly BeatLeaderUserData _self;
            private int _totalStep;
            private int _finishedStep;
            private bool _isGetAll;
            private BeatLeader.GetScoresResult _getResult = BeatLeader.GetScoresResult.CONTINUE; // BeatLeaderからの取得に成功したかどうか
            private List<BeatLeader.ScoreResponseWithMyScoreResponseWithMetadata> _collections = new();
            private int _page = 1;
            private IStepExecuter.Status _status = IStepExecuter.Status.Processing;

            public FetchLatestScoresExecuter(BeatLeaderUserData self, bool isGetAll)
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
                if (_getResult == BeatLeader.GetScoresResult.CONTINUE)
                {
                    FetchNextPartScores();
                    _finishedStep = Math.Max(_finishedStep + 1, _totalStep - 1);
                    _status = IStepExecuter.Status.Processing;
                    return _status;
                }
                else if (_getResult == BeatLeader.GetScoresResult.FINISH)
                {
                    _collections.ForEach(collection =>
                    {
                        foreach (var score in collection.data)
                        {
                            _self.PlayedScores[score.leaderboardId] = score;
                            _self.PlayHistory.Add(score);
                        }
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
                var task = BeatLeader.GetPlayerScores(_self.ProfileId, page: _page, count: 100, sortBy: "date", order: "desc");
                task.Wait();
                (_getResult, var collection) = task.Result;

                if (_getResult == BeatLeader.GetScoresResult.CONTINUE)
                {
                    // 後でまとめて処理するために取得したデータを残しておく
                    _collections.Add(collection);

                    if (!_isGetAll)
                    {
                        // ローカルに保持していないデータをすべて取得できたか確認する
                        foreach (var score in collection.data)
                        {
                            // 更新日が同じデータがローカルにあればすべて取得できた
                            if (_self.PlayedScores.TryGetValue(score.leaderboardId, out var played))
                            {
                                if (played.timeset == score.timeset)
                                {
                                    _getResult = BeatLeader.GetScoresResult.FINISH;
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
