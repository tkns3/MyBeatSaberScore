using MyBeatSaberScore.APIs;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyBeatSaberScore.Model
{
    public class ScoreSaberPlayHistory
    {
        readonly Dictionary<long, SpecificMapPlayHistory> _resultsByLeaderboardId = new();
        readonly SortedList<DateTime, PlayResult> _allResults = new();

        public void LoadFromLocalFile(string path)
        {
            _resultsByLeaderboardId.Clear();
            _allResults.Clear();

            if (File.Exists(path))
            {
                var list = Json.DeserializeFromLocalFile<List<PlayResult>>(path);
                list?.ForEach(result => Add(result));
            }
        }

        public void SaveToLocalFile(string path)
        {
            Json.SerializeToLocalFile(_allResults.Values, path, Newtonsoft.Json.Formatting.Indented);
        }

        public void Clear()
        {
            _resultsByLeaderboardId.Clear();
            _allResults.Clear();
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

        public SpecificMapPlayHistory GetSpecificMapPlayHistory(long leaderboardId)
        {
            if (_resultsByLeaderboardId.ContainsKey(leaderboardId))
            {
                return _resultsByLeaderboardId[leaderboardId];
            }
            return new();
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

        public class SpecificMapPlayHistory
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

    }
}
