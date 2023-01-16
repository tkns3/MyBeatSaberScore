using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatLeaderRankedMaps
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static async Task<Response?> GetRankedMaps(string? etag)
        {
            string url = "https://github.com/tkns3/BeatLeaderRankedData/releases/latest/download/rankedmaps.zip";
            string entryName = "rankedmaps.json";

            try
            {
                var (res, maps) = await HttpTool.DownloadZipAndDeserialize<List<MapInfo>>(url, entryName, etag);
                if (res.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    return new() { Etag = res?.Headers?.ETag?.ToString() };
                }
                else
                {
                    return new() { Etag = res?.Headers?.ETag?.ToString(), Maps = maps ?? new() };
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }
            return null;
        }

        public class Response
        {
            public string? Etag;
            public List<MapInfo> Maps = new();
        }


        public class MapInfo
        {
            public string BeatLeaderId
            {
                get
                {
                    return _beatLeaderId;
                }
                set
                {
                    _beatLeaderId = value.ToLower();
                }
            }
            public string BeatSaverId
            {
                get
                {
                    return _beatSaberId;
                }
                set
                {
                    _beatSaberId = value.ToLower();
                }
            }
            public string Hash
            {
                get
                {
                    return _hash;
                }
                set
                {
                    _hash = value.ToLower();
                }
            }
            public string SongName { get; set; } = "";
            public string SongSubName { get; set; } = "";
            public string SongAuthorName { get; set; } = "";
            public string MapperName { get; set; } = "";
            public DateTime UploadedTime { get; set; } = new();
            public double Bpm { get; set; }
            public double Duration { get; set; }
            public string DifficultyName
            {
                get
                {
                    return _difficultyName;
                }
                set
                {
                    _difficultyName = value;
                    MapDifficulty = value switch
                    {
                        "Easy" => BeatMapDifficulty.Easy,
                        "Normal" => BeatMapDifficulty.Normal,
                        "Hard" => BeatMapDifficulty.Hard,
                        "Expert" => BeatMapDifficulty.Expert,
                        "ExpertPlus" => BeatMapDifficulty.ExpertPlus,
                        _ => BeatMapDifficulty.Unknown,
                    };
                }
            }
            public string ModeName
            {
                get
                {
                    return _modeName;
                }
                set
                {
                    _modeName = value;
                    MapMode = value switch
                    {
                        "Standard" => BeatMapMode.Standard,
                        "OneSaber" => BeatMapMode.OneSaber,
                        "NoArrows" => BeatMapMode.NoArrows,
                        "90Degree" => BeatMapMode.Degree90,
                        "360Degree" => BeatMapMode.Degree360,
                        "Lightshow" => BeatMapMode.Lightshow,
                        "Lawless" => BeatMapMode.Lawless,
                        _ => BeatMapMode.Unknown,
                    };
                }
            }
            public double Stars { get; set; }
            public bool Ranked { get; set; }
            public DateTime RankedTime { get; set; } = new();
            public long Bombs { get; set; }
            public long Notes { get; set; }
            public long Walls { get; set; }
            public double Njs { get; set; }
            public double Nps { get; set; }
            public long MaxScore { get; set; }
            /// <summary>
            /// 勝手に追加したプロパティ。削除やリパブリッシュによってBeatServerからDLできないHashになっている場合にtrue。
            /// </summary>
            public bool Deleted { get; set; }

            private string _beatLeaderId = "";
            private string _beatSaberId = "";
            private string _hash = "";
            private string _difficultyName = "";
            private string _modeName = "";

            internal BeatMapDifficulty MapDifficulty;
            internal BeatMapMode MapMode;
        }
    }
}
