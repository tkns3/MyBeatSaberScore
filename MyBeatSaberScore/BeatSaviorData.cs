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

namespace MyBeatSaberScore
{
    internal class BeatSaviorData
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _rankedPath = Path.Combine(_mapsDir, "ranked.json");

        public RankedMapCollection rankedMapCollection = new RankedMapCollection();

        public BeatSaviorData()
        {
            //
        }

        public void LoadLocalFile()
        {
            try
            {
                if (File.Exists(_rankedPath))
                {
                    string jsonString = File.ReadAllText(_rankedPath, Encoding.UTF8);
                    var collection = JsonSerializer.Deserialize<RankedMapCollection>(jsonString);
                    if (collection != null)
                    {
                        foreach (var map in collection.maps)
                        {
                            map.hash = map.hash.ToLower();
                            map.coverURL = "https://cdn.scoresaber.com/covers/" + map.hash.ToUpper() + ".png";
                        }
                        rankedMapCollection = collection;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }
        }

        public void SaveLocalFile()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize<RankedMapCollection>(rankedMapCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_rankedPath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }
        }

        public async Task DownloadRankedMaps()
        {
            rankedMapCollection = await GetRankedMaps();
        }

        public static async Task<RankedMapCollection> GetRankedMaps()
        {
            string url = @"https://www.beatsavior.io/api/maps/ranked";

            try
            {
                HttpContent requestContent = new StringContent("{\"minStar\":0,\"maxStar\":20,\"playlist\":true}", Encoding.UTF8, "application/json");
                var httpsResponse = await _client.PostAsync(url, requestContent);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RankedMapCollection>(responseContent);

                if (result?.maps?.Length > 0)
                {
                    foreach (var map in result.maps)
                    {
                        map.hash = map.hash.ToLower();
                        map.coverURL = "https://cdn.scoresaber.com/covers/" + map.hash.ToUpper() + ".png";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + url);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }

            return new RankedMapCollection();
        }
    }

    [DataContract]
    public class RankedMapCollection
    {
        [DataMember]
        public RankedMap[] maps { get; set; }

        public RankedMapCollection()
        {
            maps = Array.Empty<RankedMap>();
        }
    }

    [DataContract]
    public class RankedMap
    {
        [DataMember]
        public string hash { get; set; }

        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string levelAuthorName { get; set; }

        [DataMember]
        public string songName { get; set; }

        [DataMember]
        public string songSubName { get; set; }

        [DataMember]
        public string songAuthorName { get; set; }

        [DataMember]
        public string coverURL { get; set; }

        [DataMember]
        public RankedDifficulties diffs { get; set; }

        public RankedMap()
        {
            hash = "";
            key = "";
            levelAuthorName = "";
            songName = "";
            songSubName = "";
            songAuthorName = "";
            coverURL = "";
            diffs = new RankedDifficulties();
        }
    }

    [DataContract]
    public class RankedDifficulties
    {
        [DataMember]
        public RankedDifficulty? easy { get; set; }

        [DataMember]
        public RankedDifficulty? normal { get; set; }

        [DataMember]
        public RankedDifficulty? hard { get; set; }

        [DataMember]
        public RankedDifficulty? expert { get; set; }

        [DataMember]
        public RankedDifficulty? expertplus { get; set; }

        public RankedDifficulties()
        {
            easy = null;
            normal = null;
            hard = null;
            expert = null;
            expertplus = null;
        }
    }

    public class RankedDifficulty
    {
        [DataMember]
        public double Stars { get; set; }

        public RankedDifficulty()
        {
        }
    }
 }
