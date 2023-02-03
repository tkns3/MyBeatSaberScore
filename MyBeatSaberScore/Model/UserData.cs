using MyBeatSaberScore.BeatMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace MyBeatSaberScore.Model
{
    internal class UserData
    {
        public string ProfileId { get; set; } = string.Empty;

        public ScoreSaberUserData ScoreSaber = new();

        public BeatLeaderUserData BeatLeader = new();

        public ObservableCollection<IntegrationScore> ScoresOfPlayedAndAllRanked = new();

        public void ConstractScoresOfPlayedAndAllRanked()
        {
            ScoresOfPlayedAndAllRanked = new();

            Dictionary<string, IntegrationScore> _scoreDic = new();

            // BeatLeaderのプレイ済み譜面を追加
            foreach (var score in BeatLeader.PlayedScores.Values)
            {
                var results = BeatLeader.PlayHistory.GetSpecificMapPlayHistory(score.leaderboardId);
                try
                {
                    _scoreDic.Add(KeyOfScoreDictionary(score), new IntegrationScore(score, results));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            // ScoreSaberのプレイ済み譜面の情報を追加
            foreach (var score in ScoreSaber.PlayedScores.Values)
            {
                var results = ScoreSaber.PlayHistory.GetSpecificMapPlayHistory(score.leaderboard.id);
                try
                {
                    var key = KeyOfScoreDictionary(score);
                    if (_scoreDic.TryGetValue(key, out var map))
                    {
                        map.ScoreSaber.Set(map.Map, score, results);
                    }
                    else
                    {
                        _scoreDic.Add(key, new IntegrationScore(score, results));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            // 未プレイのランク譜面を追加
            foreach (var map in BeatMapDic.Values)
            {
                if (map.ScoreSaber.Ranked || map.BeatLeader.Ranked)
                {
                    var key = KeyOfScoreDictionary(map);
                    if (!_scoreDic.ContainsKey(key))
                    {
                        _scoreDic.Add(key, new IntegrationScore(map));
                    }
                }
            }

            // スコア更新日順にならべるときBeatLeaderとScoreSaberのうち更新日が新しいほうを比較対象に使う
            static DateTime? orderKeySelector(IntegrationScore score)
            {
                if (score.BeatLeader.TimeSet != null && score.ScoreSaber.TimeSet != null)
                {
                    return (score.BeatLeader.TimeSet > score.ScoreSaber.TimeSet) ? score.BeatLeader.TimeSet : score.ScoreSaber.TimeSet;
                }
                else if (score.BeatLeader.TimeSet != null)
                {
                    return score.BeatLeader.TimeSet;
                }
                else if (score.ScoreSaber != null)
                {
                    return score.ScoreSaber.TimeSet;
                }
                else
                {
                    return null;
                }
            }

            foreach (var score in _scoreDic.Values.OrderByDescending(orderKeySelector))
            {
                ScoresOfPlayedAndAllRanked.Add(score);
            }
        }

        private string KeyOfScoreDictionary(APIs.ScoreSaber.PlayerScore score)
        {
            return $"{score.leaderboard.songHash}{(int)score.leaderboard.difficulty.mapDifficulty}{(int)score.leaderboard.difficulty.mapMode}";
        }

        private string KeyOfScoreDictionary(APIs.BeatLeader.ScoreResponseWithMyScore score)
        {
            return $"{score.leaderboard.song.hash}{(int)score.leaderboard.difficulty.mapDifficulty}{(int)score.leaderboard.difficulty.mapMode}";
        }

        private string KeyOfScoreDictionary(BeatMapData map)
        {
            return $"{map.Hash}{(int)map.MapDifficulty}{(int)map.MapMode}";
        }
    }
}
