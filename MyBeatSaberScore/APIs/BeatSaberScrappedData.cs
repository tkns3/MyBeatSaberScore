using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatSaberScrappedData
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _combinedScrappedDataJsonPath = Path.Combine(_mapsDir, "combinedScrappedData.json");

        public static async Task<Response?> GetAllMaps(string? etag)
        {
            string url = "https://github.com/andruzzzhka/BeatSaberScrappedData/raw/master/combinedScrappedData.zip";
            string entryName = "combinedScrappedData.json";

            try
            {
                _logger.Info(url);
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

/*
	{
		"Key":"1",
		"Hash":"FDA568FC27C20D21F8DC6F3709B49B5CC96723BE",
		"SongName":"me & u",
		"SongSubName":"",
		"SongAuthorName":"succducc",
		"LevelAuthorName":"datkami",
		"Diffs":[
			{
				"Diff":"Hard",
				"Char":"Standard",
				"Stars":2.81,
				"Ranked":true,
				"RankedUpdateTime":"2018-05-08T14:28:56Z",
				"Bombs":28,
				"Notes":337,
				"Obstacles":11,
				"Njs":10,
				"NjsOffset":0,
				"Requirements":[]
			}
		],
		"Chars":["Standard"],
		"Uploaded":"2018-05-08T14:28:56Z",
		"Uploader":"datkami",
		"Bpm":160,
		"Downloads":0,
		"Upvotes":587,
		"Downvotes":116,
		"Duration":144
	},
*/

        public class Response
        {
            public string? Etag;
            public List<MapInfo> Maps = new();
        }

        public class MapInfo
        {
            public string Key
            {
                get
                {
                    return _key;
                }
                set
                {
                    _key = value.ToLower();
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
            public string LevelAuthorName { get; set; } = "";
            public List<Difficulty> Diffs { get; set; } = new();
            public DateTimeOffset Uploaded { get; set; }
            public double Bpm { get; set; }
            public int Downloads { get; set; }
            public int Upvotes { get; set; }
            public int Downvotes { get; set; }
            public double Duration { get; set; }
            /// <summary>
            /// 勝手に追加したプロパティ。削除やリパブリッシュによってBeatServerからDLできないHashになっている場合にtrue。
            /// </summary>
            public bool Deleted { get; set; }

            private string _key = "";
            private string _hash = "";

            public Difficulty GetDifficulty(BeatMapMode mode, BeatMapDifficulty difficulty)
            {
                var diff = Diffs.Find(d => d.MapMode == mode && d.MapDifficulty == difficulty);
                return diff ?? new Difficulty();
            }
        }

        public class Difficulty
        {
            public string Diff
            {
                get
                {
                    return _diff;
                }
                set
                {
                    _diff = value;
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
            public string Char
            {
                get
                {
                    return _char;
                }
                set
                {
                    _char = value;
                    MapMode = value switch
                    {
                        "Standard" => BeatMapMode.Standard,
                        "OneSaber" => BeatMapMode.OneSaber,
                        "NoArrows" => BeatMapMode.NoArrows,
                        "Lightshow" => BeatMapMode.Lightshow,
                        "90Degree" => BeatMapMode.Degree90,
                        "360Degree" => BeatMapMode.Degree360,
                        "Lawless" => BeatMapMode.Lawless,
                        _ => BeatMapMode.Unknown,
                    };
                }
            }
            public double Stars { get; set; }
            public bool Ranked { get; set; }
            public DateTimeOffset RankedUpdateTime { get; set; }
            public int Bombs { get; set; }
            public int Notes { get; set; }
            public int Obstacles { get; set; }
            public double Njs { get; set; }
            public double NjsOffset { get; set; }

            private string _char = "";
            private string _diff = "";

            internal BeatMapDifficulty MapDifficulty;
            internal BeatMapMode MapMode;
        }
    }
}
