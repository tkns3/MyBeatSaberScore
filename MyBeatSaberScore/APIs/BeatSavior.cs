using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatSavior
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static async Task<RankedMapCollection> GetRankedMaps()
        {
            string url = @"https://www.beatsavior.io/api/maps/ranked";

            try
            {
                HttpContent requestContent = new StringContent("{\"minStar\":0,\"maxStar\":20,\"playlist\":true}", Encoding.UTF8, "application/json");
                var httpsResponse = await HttpTool.Client.PostAsync(url, requestContent);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var collection = JsonSerializer.Deserialize<RankedMapCollection>(responseContent);
                
                if (collection != null)
                {
                    collection.Normalize();
                    return collection;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new RankedMapCollection();
        }

        [DataContract]
        public class RankedMapCollection
        {
            [DataMember]
            public List<RankedMap> maps { get; set; }

            public RankedMapCollection()
            {
                maps = new();
            }

            public void Normalize()
            {
                maps.ForEach(map => map.Normalize());
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
            public Dictionary<string, RankedDifficulty> diffs { get; set; }

            public RankedMap()
            {
                hash = "";
                key = "";
                levelAuthorName = "";
                songName = "";
                songSubName = "";
                songAuthorName = "";
                coverURL = "";
                diffs = new();
            }

            public void Normalize()
            {
                hash = hash.ToLower();
                key = key.ToLower();
                coverURL = "https://cdn.scoresaber.com/covers/" + hash.ToUpper() + ".png";
            }
        }

        [DataContract]
        public class RankedDifficulty
        {
            [DataMember]
            public double Stars { get; set; }

            [DataMember]
            public string Diff { get; set; }

            public RankedDifficulty()
            {
                Diff = "";
            }
        }
    }
}
