using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatSaberScrappedData
    {
        private static readonly HttpClient _client = new();
        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _combinedScrappedDataJsonPath = Path.Combine(_mapsDir, "combinedScrappedData.json");

        public static async Task DownlaodCombinedScrappedData()
        {
            string url = "https://github.com/andruzzzhka/BeatSaberScrappedData/raw/master/combinedScrappedData.zip";
            string localPath = Path.Combine(_mapsDir, "combinedScrappedData.zip");

            try
            {
                HttpResponseMessage res = await _client.GetAsync(url);
                using (var fileStream = File.Create(localPath))
                {
                    using (var httpStream = await res.Content.ReadAsStreamAsync())
                    {
                        httpStream.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                }
                // 同フォルダに展開
                File.Delete(_combinedScrappedDataJsonPath);
                ZipFile.ExtractToDirectory(localPath, _mapsDir);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + url);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }
        }

        public static List<MapInfo> DeserializeCombinedScrappedData()
        {
            if (File.Exists(_combinedScrappedDataJsonPath))
            {
                string jsonString = File.ReadAllText(_combinedScrappedDataJsonPath, Encoding.UTF8);
                var mapInfoList = JsonSerializer.Deserialize<List<MapInfo>>(jsonString);
                if (mapInfoList != null)
                {
                    mapInfoList.ForEach(mapInfo => mapInfo.Normalize());
                    return mapInfoList;
                }
            }
            return new List<MapInfo>();
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

        [DataContract]
        public class MapInfo
        {
            [DataMember]
            public string Key { get; set; }

            [DataMember]
            public string Hash { get; set; }

            [DataMember]
            public string SongName { get; set; }

            [DataMember]
            public string SongSubName { get; set; }

            [DataMember]
            public string SongAuthorName { get; set; }

            [DataMember]
            public string LevelAuthorName { get; set; }

            [DataMember]
            public List<Difficulty> Diffs { get; set; }

            [DataMember]
            public string Uploaded { get; set; }

            [DataMember]
            public double Bpm { get; set; }

            [DataMember]
            public int Downloads { get; set; }

            [DataMember]
            public int Upvotes { get; set; }

            [DataMember]
            public int Downvotes { get; set; }

            [DataMember]
            public double Duration { get; set; }

            public MapInfo()
            {
                Key = "";
                Hash = "";
                SongName = "";
                SongSubName = "";
                SongAuthorName = "";
                LevelAuthorName = "";
                Diffs = new();
                Uploaded = "";
            }

            public void Normalize()
            {
                Hash = Hash.ToLower();
                Key = Key.ToLower();
                Diffs.ForEach(x => x.Normalize());
            }

            public Difficulty GetDifficulty(long difficultyRawInt)
            {
                var diff = Diffs.Find(d => d.difficultyRawInt == difficultyRawInt);
                return diff ?? new Difficulty();
            }
        }

        [DataContract]
        public class Difficulty
        {
            [DataMember]
            public string Diff { get; set; }

            [DataMember]
            public string Char { get; set; }

            [DataMember]
            public double Stars { get; set; }

            [DataMember]
            public bool Ranked { get; set; }

            [DataMember]
            public string RankedUpdateTime { get; set; }

            [DataMember]
            public int Bombs { get; set; }

            [DataMember]
            public int Notes { get; set; }

            [DataMember]
            public int Obstacles { get; set; }

            [DataMember]
            public double Njs { get; set; }

            [DataMember]
            public double NjsOffset { get; set; }

            public int difficultyRawInt { get; set; }

            public int difficultyInt { get; set; }

            public Difficulty()
            {
                Diff = "";
                Char = "";
                RankedUpdateTime = "";
            }

            private enum ECharacteristic
            {
                _UnKnown = 0,
                _Standard = 1,
                _OneSaber = 2,
                _NoArrows = 3,
                _90Degree = 4,
                _360Degree = 5,
                _Lightshow = 6,
                _Lawless = 7,
            }

            private enum EDifficulty
            {
                UnKnown = 0,
                Easy = 1,
                Normal = 3,
                Hard = 5,
                Expert = 7,
                ExpertPlus = 9,
            }

            public void Normalize()
            {
                difficultyRawInt = ToDifficultyRawInt(Char, Diff);
                difficultyInt = ToDifficultyInt(Diff);
            }

            static public int ToDifficultyInt(string difficulty)
            {
                _ = Enum.TryParse(difficulty, out EDifficulty ed);
                return (int)ed;
            }

            static public int ToDifficultyRawInt(string characteristic, string difficulty)
            {
                _ = Enum.TryParse("_" + characteristic, out ECharacteristic ec);
                _ = Enum.TryParse(difficulty, out EDifficulty ed);

                return (int)ec * 32 + (int)ed;
            }

            public int GetMaxScore()
            {
                int[] scoreArray = new int[] { 0, 115, 345, 575, 805, 1035, 1495, 1955, 2415, 2875, 3335, 3795, 4255, 4715 };
                if (Notes < 14)
                {
                    return scoreArray[Notes];
                }
                else
                {
                    return 4715 + (Notes - 13) * 920;
                }
            }
        }
    }
}
