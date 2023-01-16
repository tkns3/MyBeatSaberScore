using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatSavior
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static async Task<RankedMapCollection> GetRankedMaps()
        {
            string url = @"https://www.beatsavior.io/api/maps/ranked";
            HttpContent requestContent = new StringContent("{\"minStar\":0,\"maxStar\":20,\"playlist\":true}", Encoding.UTF8, "application/json");

            try
            {
                var result = await HttpTool.PostAndDeserialize<RankedMapCollection>(url, requestContent);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new RankedMapCollection();
        }

        public class RankedMapCollection
        {
            public List<RankedMap> maps { get; set; } = new();
        }

        public class RankedMap
        {
            public string hash
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
            public string key
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
            public string levelAuthorName { get; set; } = "";
            public string songName { get; set; } = "";
            public string songSubName { get; set; } = "";
            public string songAuthorName { get; set; } = "";
            public string coverURL { get; set; } = "";
            public Dictionary<string, RankedDifficulty> diffs { get; set; } = new();

            private string _hash = "";
            private string _key = "";
        }

        public class RankedDifficulty
        {
            public double Stars { get; set; }
            public string Diff { get; set; } = "";
        }
    }
}
