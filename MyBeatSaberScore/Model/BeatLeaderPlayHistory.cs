using MyBeatSaberScore.APIs;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyBeatSaberScore.Model
{
    internal class BeatLeaderPlayHistory
    {
        readonly Dictionary<string, SpecificMapPlayHistory> _resultsByLeaderboardId = new();
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
            _ = _allResults.TryAdd(result.timeset, result);
        }

        public void Add(BeatLeader.ScoreResponseWithMyScore score)
        {
            Add(new PlayResult
            {
                id = score.id,
                leaderboardId = score.leaderboardId,
                weight = score.weight,
                accLeft = score.accLeft,
                accRight = score.accRight,
                baseScore = score.baseScore,
                modifiedScore = score.modifiedScore,
                accuracy = score.accuracy,
                playerId = score.playerId,
                pp = score.pp,
                bonusPp = score.bonusPp,
                rank = score.rank,
                countryRank = score.countryRank,
                country = score.country,
                fcAccuracy = score.fcAccuracy,
                fcPp = score.fcPp,
                replay = score.replay,
                modifiers = score.modifiers,
                badCuts = score.badCuts,
                missedNotes = score.missedNotes,
                bombCuts = score.bombCuts,
                wallsHit = score.wallsHit,
                pauses = score.pauses,
                fullCombo = score.fullCombo,
                platform = score.platform,
                maxCombo = score.maxCombo,
                maxStreak = score.maxStreak,
                hmd = score.hmd,
                controller = score.controller,
                timeset = new DateTime(1970, 1, 1).AddSeconds(double.Parse(score.timeset)),
                timepost = score.timepost,
                replaysWatched = score.replaysWatched,
            });
        }

        public SpecificMapPlayHistory GetSpecificMapPlayHistory(string leaderboardId)
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
            public string leaderboardId { get; set; } = string.Empty;
            public double weight { get; set; }
            public double accLeft { get; set; }
            public double accRight { get; set; }
            public int baseScore { get; set; }
            public int modifiedScore { get; set; }
            public double accuracy { get; set; }
            public string playerId { get; set; } = string.Empty;
            public double pp { get; set; }
            public double bonusPp { get; set; }
            public int rank { get; set; }
            public int countryRank { get; set; }
            public string country { get; set; } = string.Empty;
            public double fcAccuracy { get; set; }
            public double fcPp { get; set; }
            public string replay { get; set; } = string.Empty;
            public string modifiers { get; set; } = string.Empty;
            public int badCuts { get; set; }
            public int missedNotes { get; set; }
            public int bombCuts { get; set; }
            public int wallsHit { get; set; }
            public int pauses { get; set; }
            public bool fullCombo { get; set; }
            public string platform { get; set; } = string.Empty;
            public int maxCombo { get; set; }
            public int maxStreak { get; set; }
            public int hmd { get; set; }
            public int controller { get; set; }
            public DateTime timeset { get; set; }
            public int timepost { get; set; }
            public int replaysWatched { get; set; }
        }

        public class SpecificMapPlayHistory
        {
            readonly SortedList<DateTime, PlayResult> _results = new();

            public void Add(PlayResult result)
            {
                _ = _results.TryAdd(result.timeset, result);
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
