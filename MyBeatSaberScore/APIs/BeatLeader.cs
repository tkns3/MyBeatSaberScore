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
    public static class BeatLeader
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static readonly HttpClient _client = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <returns></returns>
        public static async Task<LeaderboardsResponse> GetLeaderboardsByHash(string hash)
        {
            string url = $"https://api.beatleader.xyz/leaderboards/hash/{hash}";

            try
            {
                var httpsResponse = await _client.GetAsync(url);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var leaderboardsResponse = JsonSerializer.Deserialize<LeaderboardsResponse>(responseContent);
                if (leaderboardsResponse != null)
                {
                    return leaderboardsResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new LeaderboardsResponse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="difficulty">1:Easy, 3:Normal, 5:Hard, 7:Expert 9:ExpertPlus</param>
        /// <param name="modeName">SoloStandard, SoloLawless, SoloOneSaber, SoloLightShow, Solo90Degree, Solo360Degree, SoloNoArrows</param>
        /// <returns></returns>
        public static string GetLeaderboardId(LeaderboardsResponse response, int difficulty, string modeName)
        {
            int mode = 1;
            switch (modeName)
            {
                case "SoloStandard": mode = 1; break;
                case "SoloLawless": mode = 7; break;
                case "SoloOneSaber": mode = 2; break;
                case "SoloLightShow": mode = 6; break;
                case "Solo90Degree": mode = 4; break;
                case "Solo360Degree": mode = 5; break;
                case "SoloNoArrows": mode = 3; break;
                default: mode = 1; break;
            }

            string? id = response.leaderboards.Find(l => l.difficulty.value == difficulty && l.difficulty.mode == mode)?.id;
            return id ?? "";
        }

        [DataContract]
        public class LeaderboardsResponse
        {
            [DataMember]
            public List<LeaderboardsInfoResponse> leaderboards { get; set; }


            public LeaderboardsResponse()
            {
                leaderboards = new();
            }
        }

        [DataContract]
        public class LeaderboardsInfoResponse
        {
            [DataMember]
            public string id { get; set; }

            [DataMember]
            public DifficultyDescription difficulty { get; set; }

            public LeaderboardsInfoResponse()
            {
                id = "";
                difficulty = new DifficultyDescription();
            }
        }

        public class DifficultyDescription
        {
            [DataMember]
            public int id { get; set; }

            [DataMember]
            public int value { get; set; }

            [DataMember]
            public int mode { get; set; }
        }
    }
}
