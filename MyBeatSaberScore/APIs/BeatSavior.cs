using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatSavior
    {
        private static readonly HttpClient _client = new();

        public static async Task<RankedMapCollection> GetRankedMaps()
        {
            string url = @"https://www.beatsavior.io/api/maps/ranked";

            try
            {
                HttpContent requestContent = new StringContent("{\"minStar\":0,\"maxStar\":20,\"playlist\":true}", Encoding.UTF8, "application/json");
                var httpsResponse = await _client.PostAsync(url, requestContent);
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
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + url);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
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

            public void Normalize()
            {
                hash = hash.ToLower();
                key = key.ToLower();
                coverURL = "https://cdn.scoresaber.com/covers/" + hash.ToUpper() + ".png";
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

        [DataContract]
        public class RankedDifficulty
        {
            [DataMember]
            public double Stars { get; set; }

            public RankedDifficulty()
            {
            }
        }
    }
}
