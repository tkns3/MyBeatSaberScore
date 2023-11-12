using MyBeatSaberScore.APIs;
using MyBeatSaberScore.Model;
using MyBeatSaberScore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeatSaberScore.BeatMap
{
    internal static class BeatMapDic
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static Dictionary<string, BeatMapData> _dic = new();
        private static BeatSaberScrappedData.Response _beatSaberScrappedDataCache = new();
        private static BeatLeaderRankedMaps.Response _beatLeaderRankedMapsCache = new();

        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _beatSaberScrappedDataChachePath = Path.Combine(_mapsDir, "cache1.json");
        private static readonly string _beatLeaderRankedMapsChachePath = Path.Combine(_mapsDir, "cache2.json");

        internal static IEnumerable<BeatMapData> Values { get { return _dic.Values; } }

        internal static void Initialize()
        {
            _beatSaberScrappedDataCache = Json.DeserializeFromLocalFile<BeatSaberScrappedData.Response>(_beatSaberScrappedDataChachePath) ?? new();
            _beatLeaderRankedMapsCache = Json.DeserializeFromLocalFile<BeatLeaderRankedMaps.Response>(_beatLeaderRankedMapsChachePath) ?? new();
            UpdateDictionary();
        }

        private static string GetDictionaryKey(string hash, BeatMapMode mode, BeatMapDifficulty difficulty)
        {
            return $"{hash}{(int)mode}{(int)difficulty}";
        }

        internal static UpdateDirectoryExecuter UpdateDirectory()
        {
            return new UpdateDirectoryExecuter();
        }

        internal static BeatMapData? Get(string hash, BeatMapMode mode, BeatMapDifficulty difficulty)
        {
            if (_dic.TryGetValue(GetDictionaryKey(hash, mode, difficulty), out var data))
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        private static async Task<bool> UpdateBeatSaberScrapedDataCache()
        {
            var beatSaberScrappdData = await BeatSaberScrappedData.GetAllMaps(_beatSaberScrappedDataCache.Etag);
            if (beatSaberScrappdData != null)
            {
                if (beatSaberScrappdData.Maps.Count > 0)
                {
                    Dictionary<string, BeatSaberScrappedData.MapInfo> dic = new();
                    foreach (var map in _beatSaberScrappedDataCache.Maps)
                    {
                        map.Deleted = true; // キャッシュのマップはいったん削除フラグたてておく
                        _ = dic.TryAdd(map.Hash, map);
                    }
                    foreach (var map in beatSaberScrappdData.Maps)
                    {
                        map.Deleted = false; // 新規で取得したマップは存在するのでフラグを落とす
                        if (dic.ContainsKey(map.Hash))
                        {
                            dic[map.Hash] = map;
                        }
                        else
                        {
                            dic.Add(map.Hash, map);
                        }
                    }
                    _beatSaberScrappedDataCache.Etag = beatSaberScrappdData.Etag;
                    _beatSaberScrappedDataCache.Maps = dic.Values.ToList();
                    return true;
                }
            }
            return false;
        }

        private static async Task<bool> UpdateBeatLeaderRankedMapsCache()
        {
            var beatLeaderRankedMaps = await BeatLeaderRankedMaps.GetRankedMaps(_beatLeaderRankedMapsCache.Etag);
            if (beatLeaderRankedMaps != null)
            {
                if (beatLeaderRankedMaps.Maps.Count > 0)
                {
                    _beatLeaderRankedMapsCache.Etag = beatLeaderRankedMaps.Etag;
                    _beatLeaderRankedMapsCache.Maps = beatLeaderRankedMaps.Maps;
                    return true;
                }
            }
            return false;
        }

        private static void UpdateDictionary()
        {
            _dic.Clear();

            _beatSaberScrappedDataCache.Maps.ForEach(map =>
            {
                map.Diffs.ForEach(diff =>
                {
                    var dicKey = GetDictionaryKey(map.Hash, diff.MapMode, diff.MapDifficulty);
                    var mapData = new BeatMapData()
                    {
                        Key = map.Key,
                        Hash = map.Hash,
                        SongName = map.SongName,
                        SongSubName = map.SongSubName,
                        SongAuthorName = map.SongAuthorName,
                        MapperName = map.LevelAuthorName,
                        UploadedTime = map.Uploaded,
                        Bpm = map.Bpm,
                        Duration = map.Duration,
                        MapDifficulty = diff.MapDifficulty,
                        MapMode = diff.MapMode,
                        Bombs = diff.Bombs,
                        Notes = diff.Notes,
                        Walls = diff.Obstacles,
                        Njs = diff.Njs,
                        Nps = (map.Duration == 0) ? 0 : diff.Notes / map.Duration,
                        MaxScore = MaxScore(diff.Notes),
                        Deleted = false,
                    };
                    mapData.ScoreSaber.Ranked = diff.Ranked && diff.MapMode == BeatMapMode.Standard; // ScrappedDataはStandard以外のRankedがtrueになるバグがあるので除外しておく (ScoreSaberのRanked要件にStandardであることという条件がある)
                    mapData.ScoreSaber.RankedTime = (diff.Ranked && diff.MapMode == BeatMapMode.Standard) ? diff.RankedUpdateTime : null;
                    mapData.ScoreSaber.Star = diff.Stars;

                    if (!_dic.TryAdd(dicKey, mapData))
                    {
                        _dic[dicKey] = mapData;
                    }
                });
            });

            _beatLeaderRankedMapsCache.Maps.ForEach(map =>
            {
                var dicKey = GetDictionaryKey(map.Hash, map.MapMode, map.MapDifficulty);
                if (_dic.TryGetValue(dicKey, out var alreadyMapData))
                {
                    alreadyMapData.BeatLeader.Ranked = map.Ranked;
                    alreadyMapData.BeatLeader.RankedTime = map.RankedTime;
                    alreadyMapData.BeatLeader.Star = map.Stars;
                    alreadyMapData.Deleted = false;
                }
                else
                {
                    var mapData = new BeatMapData()
                    {
                        Key = map.BeatSaverId,
                        Hash = map.Hash,
                        SongName = map.SongName,
                        SongSubName = map.SongSubName,
                        SongAuthorName = map.SongAuthorName,
                        MapperName = map.MapperName,
                        UploadedTime = map.UploadedTime,
                        Bpm = map.Bpm,
                        Duration = map.Duration,
                        MapDifficulty = map.MapDifficulty,
                        MapMode = map.MapMode,
                        Bombs = map.Bombs,
                        Notes = map.Notes,
                        Walls = map.Walls,
                        Njs = map.Njs,
                        Nps = map.Nps,
                        MaxScore = map.MaxScore,
                        Deleted = false,
                    };
                    mapData.BeatLeader.Ranked = map.Ranked;
                    mapData.BeatLeader.RankedTime = (map.Ranked) ? map.RankedTime : null;
                    mapData.BeatLeader.Star = map.Stars;

                    _ = _dic.TryAdd(dicKey, mapData);
                }
            });
        }

        public class UpdateDirectoryExecuter : IStepExecuter
        {
            private int _totalStep;
            private int _finishedStep;
            private IStepExecuter.Status _status = IStepExecuter.Status.Processing;
            private bool _t1Result;
            private bool _t2Result;

            public UpdateDirectoryExecuter()
            {
                _totalStep = 4;
                _finishedStep = 0;
            }

            public int TotalStepCount { get => _totalStep; }

            public int FinishedStepCount { get => _finishedStep; }

            public IStepExecuter.Status CurrentStatus { get => _status; }

            public IStepExecuter.Status ExecuteStep()
            {
                switch (_finishedStep)
                {
                    case 0:
                        var t1 = UpdateBeatSaberScrapedDataCache();
                        var t2 = UpdateBeatLeaderRankedMapsCache();
                        Task.WaitAll(t1, t2);
                        _t1Result = t1.Result;
                        _t2Result = t2.Result;
                        _finishedStep += 1;
                        break;
                    case 1:
                        UpdateDictionary();
                        _finishedStep += 1;
                        break;
                    case 2:
                        if (_t1Result)
                        {
                            Json.SerializeToLocalFile(_beatSaberScrappedDataCache, _beatSaberScrappedDataChachePath);
                        }
                        _finishedStep += 1;
                        break;
                    case 3:
                        if (_t2Result)
                        {
                            Json.SerializeToLocalFile(_beatLeaderRankedMapsCache, _beatLeaderRankedMapsChachePath);
                        }
                        _finishedStep += 1;
                        _status = IStepExecuter.Status.Completed;
                        break;
                    default:
                        break;
                }
                return _status;
            }
        }

        private static long MaxScore(int notes)
        {
            int[] scoreArray = new int[] { 0, 115, 345, 575, 805, 1035, 1495, 1955, 2415, 2875, 3335, 3795, 4255, 4715 };
            if (notes < 14)
            {
                return scoreArray[notes];
            }
            else
            {
                return 4715 + (notes - 13) * 920;
            }
        }
    }
}
