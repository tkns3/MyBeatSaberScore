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
    public static class BeatSaver
    {
        private static readonly HttpClient _client = new();

        public static async Task<MapDetail> GetMapDetailByHash(string hash)
        {
            hash = hash.ToLower();
            string url = $"https://beatsaver.com/api/maps/hash/{hash}";

            try
            {
                var httpsResponse = await _client.GetAsync(url);
                var responseContent = await httpsResponse.Content.ReadAsStringAsync();
                var detail = JsonSerializer.Deserialize<MapDetail>(responseContent);
                if (detail != null)
                {
                    detail.Normalize();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + url);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }

            return new MapDetail();
        }

        [DataContract]
        public class MapCollection
        {
            [DataMember]
            public List<MapDetail> mapDetails { get; set; }

            public MapCollection()
            {
                mapDetails = new();
            }

            public void Normalize()
            {
                if (mapDetails == null)
                {
                    mapDetails = new();
                }
                mapDetails.ForEach(mapDetail => mapDetail.Normalize());
            }
        }

        [DataContract]
        public class MapDetail
        {
            [DataMember]
            public bool automapper { get; set; }

            [DataMember]
            public string createdAt { get; set; }

            [DataMember]
            public string curator { get; set; }

            [DataMember]
            public string deletedAt { get; set; }

            [DataMember]
            public string description { get; set; }

            [DataMember]
            public string id { get; set; }

            [DataMember]
            public string lastPublishedAt { get; set; }

            [DataMember]
            public MapDetailMetadata metadata { get; set; }

            [DataMember]
            public string name { get; set; }

            [DataMember]
            public bool qualified { get; set; }

            [DataMember]
            public bool ranked { get; set; }

            [DataMember]
            public MapStats stats { get; set; }

            [DataMember]
            public string updatedAt { get; set; }

            [DataMember]
            public string uploaded { get; set; }

            [DataMember]
            public UserDetail uploader { get; set; }

            [DataMember]
            public List<MapVersion> versions { get; set; }

            [DataMember]
            public string error { get; set; }

            public MapDetail()
            {
                createdAt = "";
                curator = "";
                deletedAt = "";
                description = "";
                id = "";
                lastPublishedAt = "";
                metadata = new();
                name = "";
                stats = new();
                updatedAt = "";
                uploaded = "";
                uploader = new();
                versions = new();
                error = "";
            }

            public void Normalize()
            {
                if (id == null)
                {
                    id = "";
                }
                if (versions == null)
                {
                    versions = new();
                }
                if (error == null)
                {
                    error = "";
                }
                id = id.ToLower();
                versions.ForEach(v => v.Normalize());
            }
        }

        [DataContract]
        public class Instant
        {
            [DataMember]
            public int epochSeconds { get; set; }

            [DataMember]
            public int nanosecondsOfSecond { get; set; }

            [DataMember]
            public string value { get; set; }

            public Instant()
            {
                value = "";
            }
        }

        [DataContract]
        public class MapDetailMetadata
        {
            [DataMember]
            public double bpm { get; set; }

            [DataMember]
            public int duration { get; set; }

            [DataMember]
            public string levelAuthorName { get; set; }

            [DataMember]
            public string songAuthorName { get; set; }

            [DataMember]
            public string songName { get; set; }

            [DataMember]
            public string songSubName { get; set; }

            public MapDetailMetadata()
            {
                levelAuthorName = "";
                songAuthorName = "";
                songName = "";
                songSubName = "";
            }
        }

        [DataContract]
        public class MapStats
        {
            [DataMember]
            public int downloads { get; set; }

            [DataMember]
            public int downvotes { get; set; }

            [DataMember]
            public int plays { get; set; }

            [DataMember]
            public double score { get; set; }

            [DataMember]
            public int upvotes { get; set; }

            public MapStats()
            {
            }
        }

        [DataContract]
        public class UserDetail
        {
            [DataMember]
            public string avatar { get; set; }

            [DataMember]
            public string email { get; set; }

            [DataMember]
            public string hash { get; set; }

            [DataMember]
            public int id { get; set; }

            [DataMember]
            public string name { get; set; }

            [DataMember]
            public UserStats stats { get; set; }

            [DataMember]
            public bool testplay { get; set; }

            [DataMember]
            public string type { get; set; }

            [DataMember]
            public bool uniqueSet { get; set; }

            [DataMember]
            public int uploadLimit { get; set; }

            public UserDetail()
            {
                avatar = "";
                email = "";
                hash = "";
                name = "";
                stats = new UserStats();
                type = "";
            }
        }

        [DataContract]
        public class UserStats
        {
            [DataMember]
            public double avgBpm { get; set; }

            [DataMember]
            public double avgDuration { get; set; }

            [DataMember]
            public double avgScore { get; set; }

            [DataMember]
            public UserDiffStats diffStats { get; set; }

            [DataMember]
            public string firstUpload { get; set; }

            [DataMember]
            public string lastUpload { get; set; }

            [DataMember]
            public int rankedMaps { get; set; }

            [DataMember]
            public int totalDownvotes { get; set; }

            [DataMember]
            public int totalMaps { get; set; }

            [DataMember]
            public int totalUpvotes { get; set; }

            public UserStats()
            {
                diffStats = new UserDiffStats();
                firstUpload = "";
                lastUpload = "";
            }
        }

        [DataContract]
        public class UserDiffStats
        {
            [DataMember]
            public int easy { get; set; }

            [DataMember]
            public int expert { get; set; }

            [DataMember]
            public int expertPlus { get; set; }

            [DataMember]
            public int hard { get; set; }

            [DataMember]
            public int normal { get; set; }

            [DataMember]
            public int total { get; set; }
        }

        [DataContract]
        public class MapVersion
        {
            [DataMember]
            public string coverURL { get; set; }

            [DataMember]
            public string createdAt { get; set; }

            [DataMember]
            public List<MapDifficulty> diffs { get; set; }

            [DataMember]
            public string downloadURL { get; set; }

            [DataMember]
            public string feedback { get; set; }

            [DataMember]
            public string hash { get; set; }

            [DataMember]
            public string key { get; set; }

            [DataMember]
            public string previewURL { get; set; }

            [DataMember]
            public int sageScore { get; set; }

            [DataMember]
            public string scheduledAt { get; set; }

            [DataMember]
            public string state { get; set; }

            [DataMember]
            public string testplayAt { get; set; }

            [DataMember]
            public List<MapTestplay> testplays { get; set; }

            public MapVersion()
            {
                coverURL = "";
                createdAt = "";
                diffs = new();
                downloadURL = "";
                feedback = "";
                hash = "";
                key = "";
                previewURL = "";
                scheduledAt = "";
                state = "";
                testplayAt = "";
                testplays = new();
            }

            public void Normalize()
            {
                if (diffs == null)
                {
                    diffs = new();
                }
                hash = hash.ToLower();
                key = key.ToLower();
                diffs.ForEach(d => d.Normalize());
            }
        }

        [DataContract]
        public class MapDifficulty
        {
            [DataMember]
            public int bombs { get; set; }

            [DataMember]
            public string characteristic { get; set; }

            [DataMember]
            public bool chroma { get; set; }

            [DataMember]
            public bool cinema { get; set; }

            [DataMember]
            public string difficulty { get; set; }

            [DataMember]
            public int events { get; set; }

            [DataMember]
            public double length { get; set; }

            [DataMember]
            public bool me { get; set; }

            [DataMember]
            public bool ne { get; set; }

            [DataMember]
            public double njs { get; set; }

            [DataMember]
            public int notes { get; set; }

            [DataMember]
            public double nps { get; set; }

            [DataMember]
            public int obstacles { get; set; }

            [DataMember]
            public double offset { get; set; }

            [DataMember]
            public MapParitySummary paritySummary { get; set; }

            [DataMember]
            public double seconds { get; set; }

            [DataMember]
            public double stars { get; set; }

            [IgnoreDataMember]
            public int difficultyRawInt { get; set; }

            public MapDifficulty()
            {
                characteristic = "";
                difficulty = "";
                paritySummary = new MapParitySummary();
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
                difficultyRawInt = ToDifficultyRawInt(characteristic, difficulty);
            }

            static public int ToDifficultyRawInt(string characteristic, string difficulty)
            {
                ECharacteristic ec = ECharacteristic._UnKnown;
                _ = Enum.TryParse("_" + characteristic, out ec);

                EDifficulty ed = EDifficulty.UnKnown;
                _ = Enum.TryParse(difficulty, out ed);

                return (int)ec * 32 + (int)ed;
            }
        }

        [DataContract]
        public class MapParitySummary
        {
            [DataMember]
            public int errors { get; set; }

            [DataMember]
            public int resets { get; set; }

            [DataMember]
            public int warns { get; set; }
        }

        [DataContract]
        public class MapTestplay
        {
            [DataMember]
            public string createdAt { get; set; }

            [DataMember]
            public string feedback { get; set; }

            [DataMember]
            public string feedbackAt { get; set; }

            [DataMember]
            public UserDetail user { get; set; }

            [DataMember]
            public string video { get; set; }

            public MapTestplay()
            {
                createdAt = "";
                feedback = "";
                feedbackAt = "";
                user = new UserDetail();
                video = "";
            }
        }
    }
}
