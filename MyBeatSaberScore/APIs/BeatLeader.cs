using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeatSaberScore.APIs
{
    public static class BeatLeader
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public enum GetScoresResult
        {
            FAIL,
            CONTINUE,
            FINISH,
            RETRY,
        }

        public static async Task<(GetScoresResult, ScoreResponseWithMyScoreResponseWithMetadata)> GetPlayerScores(string id, int page, int count, string? sortBy = null, string? order = null, string? search = null, string? diff = null, string? type = null, double? stars_from = null, double? stars_to = null, int? time_from = null, int? time_to = null, int? eventId = null)
        {
            var opt_page = $"page={page}";
            var opt_count = $"&count={count}";
            var opt_sortBy = (sortBy == null) ? "" : $"&sortBy={sortBy}";
            var opt_order = (order == null) ? "" : $"&order={order}";
            var opt_search = (search == null) ? "" : $"&search={search}";
            var opt_diff = (diff == null) ? "" : $"&diff={diff}";
            var opt_type = (type == null) ? "" : $"&type={type}";
            var opt_stars_from = (stars_from == null) ? "" : $"&stars_from={stars_from}";
            var opt_stars_to = (stars_to == null) ? "" : $"&stars_to={stars_to}";
            var opt_time_from = (time_from == null) ? "" : $"&time_from={time_from}";
            var opt_time_to = (time_to == null) ? "" : $"&time_to={time_to}";
            var opt_eventId = (eventId == null) ? "" : $"&eventId={eventId}";
            string url = $"https://api.beatleader.xyz/player/{id}/scores?{opt_page}{opt_count}{opt_sortBy}{opt_order}{opt_search}{opt_diff}{opt_type}{opt_stars_from}{opt_stars_to}{opt_time_from}{opt_time_to}{opt_eventId}";

            try
            {
                _logger.Info(url);
                var response = await HttpTool.GetAndDeserialize<ScoreResponseWithMyScoreResponseWithMetadata>(url);
                var result = (response.data.Length == 0) ? GetScoresResult.FINISH : GetScoresResult.CONTINUE;
                return (result, response);
            }
            catch (HttpTool.HttpFailuerException ex)
            {
                _logger.Warn($"{url}: {ex}");
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return (GetScoresResult.RETRY, new ScoreResponseWithMyScoreResponseWithMetadata());
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return (GetScoresResult.FAIL, new ScoreResponseWithMyScoreResponseWithMetadata());
        }

        public static async Task<PlayerResponseFull> GetPlayerInfo(string id)
        {
            string url = $"https://api.beatleader.xyz/player/{id}?stats=true";

            try
            {
                _logger.Info(url);
                var result = await HttpTool.GetAndDeserialize<PlayerResponseFull>(url);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new PlayerResponseFull();
        }

        public static async Task<LeaderboardsResponse> GetLeaderboardsByHash(string hash)
        {
            string url = $"https://api.beatleader.xyz/leaderboards/hash/{hash}";

            try
            {
                _logger.Info(url);
                var result = await HttpTool.GetAndDeserialize<LeaderboardsResponse>(url);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Warn($"{url}: {ex}");
            }

            return new LeaderboardsResponse();
        }

        public class LeaderboardsResponse
        {
            public List<LeaderboardsInfoResponse> leaderboards { get; set; } = new();
        }

        public class LeaderboardsInfoResponse
        {
            public string id { get; set; } = "";
            public DifficultyDescription difficulty { get; set; } = new();
        }

        public class DifficultyDescription
        {
            public int id { get; set; }
            public int value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    mapDifficulty = value switch
                    {
                        1 => BeatMapDifficulty.Easy,
                        3 => BeatMapDifficulty.Normal,
                        5 => BeatMapDifficulty.Hard,
                        7 => BeatMapDifficulty.Expert,
                        9 => BeatMapDifficulty.ExpertPlus,
                        _ => BeatMapDifficulty.Unknown,
                    };
                }
            }
            public int mode
            {
                get
                {
                    return _mode;
                }
                set
                {
                    _mode = value;
                    mapMode = value switch
                    {
                        1 => BeatMapMode.Standard,
                        2 => BeatMapMode.OneSaber,
                        3 => BeatMapMode.NoArrows,
                        4 => BeatMapMode.Degree90,
                        5 => BeatMapMode.Degree360,
                        6 => BeatMapMode.Lightshow,
                        7 => BeatMapMode.Lawless,
                        _ => BeatMapMode.Unknown,
                    };
                }
            }
            public string? difficultyName { get; set; }
            public string? modeName { get; set; }
            public int status { get; set; }
            public ModifiersMap modifierValues { get; set; } = new();
            public int nominatedTime { get; set; }
            public int qualifiedTime { get; set; }
            public int rankedTime { get; set; }
            public double? stars { get; set; }
            public double? predictedAcc { get; set; }
            public double? passRating { get; set; }
            public double? accRating { get; set; }
            public double? techRating { get; set; }
            public int type { get; set; }
            public double njs { get; set; }
            public double nps { get; set; }
            public int notes { get; set; }
            public int bombs { get; set; }
            public int walls { get; set; }
            public int maxScore { get; set; }
            public double duration { get; set; }

            private int _value;
            private int _mode;

            internal BeatMapDifficulty mapDifficulty;
            internal BeatMapMode mapMode;
        }

        public class ScoreResponseWithMyScoreResponseWithMetadata
        {
            public Metadata metadata { get; set; } = new();
            public ScoreResponseWithMyScore[] data { get; set; } = Array.Empty<ScoreResponseWithMyScore>();
        }

        public class Metadata
        {
            public int itemsPerPage { get; set; }
            public int page { get; set; }
            public int total { get; set; }
        }

        public class ScoreResponseWithMyScore
        {
            //public object myScore { get; set; }
            public LeaderboardResponse leaderboard { get; set; } = new();
            public double weight { get; set; }
            public double accLeft { get; set; }
            public double accRight { get; set; }
            public int id { get; set; }
            public int baseScore { get; set; }
            public int modifiedScore { get; set; }
            public double accuracy { get; set; }
            public string? playerId { get; set; }
            public double pp { get; set; }
            public double bonusPp { get; set; }
            public int rank { get; set; }
            public int countryRank { get; set; }
            public string? country { get; set; }
            public double fcAccuracy { get; set; }
            public double fcPp { get; set; }
            public string? replay { get; set; }
            public string? modifiers { get; set; }
            public int badCuts { get; set; }
            public int missedNotes { get; set; }
            public int bombCuts { get; set; }
            public int wallsHit { get; set; }
            public int pauses { get; set; }
            public bool fullCombo { get; set; }
            public string? platform { get; set; }
            public int maxCombo { get; set; }
            public int? maxStreak { get; set; }
            public int hmd { get; set; }
            public int controller { get; set; }
            public string? leaderboardId { get; set; }
            public string? timeset { get; set; }
            public int timepost { get; set; }
            public int replaysWatched { get; set; }
            public PlayerResponse player { get; set; } = new();
            public ScoreImprovement scoreImprovement { get; set; } = new();
            //public object rankVoting { get; set; }
            //public object metadata { get; set; }
            public ReplayOffsets offsets { get; set; } = new();
        }

        public class LeaderboardResponse
        {
            public string? id { get; set; }
            public Song song { get; set; } = new();
            public DifficultyDescription difficulty { get; set; } = new();
            //public object scores { get; set; }
            //public object changes { get; set; }
            //public object qualification { get; set; }
            //public object reweight { get; set; }
            //public object leaderboardGroup { get; set; }
            public int plays { get; set; }
        }

        public class Song
        {
            public string? id { get; set; }
            public string? hash
            {
                get
                {
                    return _hash;
                }
                set
                {
                    if (value != null)
                    {
                        _hash = value?.ToLower();
                    }
                }
            }
            public string? name { get; set; }
            //public object description { get; set; }
            public string? subName { get; set; }
            public string? author { get; set; }
            public string? mapper { get; set; }
            public int mapperId { get; set; }
            public string? coverImage { get; set; }
            public string? downloadUrl { get; set; }
            public double bpm { get; set; }
            public double duration { get; set; }
            public string? tags { get; set; }
            public string? createdTime { get; set; }
            public int uploadTime { get; set; }
            public DifficultyDescription[] difficulties { get; set; } = Array.Empty<DifficultyDescription>();

            private string? _hash;
        }

        public class ModifiersMap
        {
            public int modifierId { get; set; }
            public double da { get; set; }
            public double fs { get; set; }
            public double ss { get; set; }
            public double sf { get; set; }
            public double gn { get; set; }
            public double na { get; set; }
            public double nb { get; set; }
            public double nf { get; set; }
            public double no { get; set; }
            public double pm { get; set; }
            public double sc { get; set; }
            public double sa { get; set; }
        }

        public class PlayerResponse
        {
            public string? id { get; set; }
            public string? name { get; set; }
            public string? platform { get; set; }
            public string? avatar { get; set; }
            public string? country { get; set; }
            public double pp { get; set; }
            public int rank { get; set; }
            public int countryRank { get; set; }
            public string? role { get; set; }
            //public object[] socials { get; set; }
            //public object patreonFeatures { get; set; }
            //public object profileSettings { get; set; }
            public ClanResponse[] clans { get; set; } = Array.Empty<ClanResponse>();
        }

        public class ClanResponse
        {
            public int id { get; set; }
            public string? tag { get; set; }
            public string? color { get; set; }
        }

        public class ScoreImprovement
        {
            public int id { get; set; }
            public string? timeset { get; set; }
            public int score { get; set; }
            public double accuracy { get; set; }
            public double pp { get; set; }
            public double bonusPp { get; set; }
            public int rank { get; set; }
            public double accRight { get; set; }
            public double accLeft { get; set; }
            public double averageRankedAccuracy { get; set; }
            public double totalPp { get; set; }
            public int totalRank { get; set; }
            public int badCuts { get; set; }
            public int missedNotes { get; set; }
            public int bombCuts { get; set; }
            public int wallsHit { get; set; }
            public int pauses { get; set; }
        }

        public class ReplayOffsets
        {
            public int id { get; set; }
            public int frames { get; set; }
            public int notes { get; set; }
            public int walls { get; set; }
            public int heights { get; set; }
            public int pauses { get; set; }
        }


        public class PlayerResponseFull
        {
            public int mapperId { get; set; }
            public bool banned { get; set; }
            public bool inactive { get; set; }
            //public object banDescription { get; set; }
            public string? externalProfileUrl { get; set; }
            //public object history { get; set; }
            //public object[] badges { get; set; }
            //public object pinnedScores { get; set; }
            //public object[] changes { get; set; }
            public PlayerScoreStats scoreStats { get; set; } = new();
            public double lastWeekPp { get; set; }
            public int lastWeekRank { get; set; }
            public int lastWeekCountryRank { get; set; }
            public EventPlayer[] eventsParticipating { get; set; } = Array.Empty<EventPlayer>();
            public string? id { get; set; }
            public string? name { get; set; }
            public string? platform { get; set; }
            public string? avatar { get; set; }
            public string? country { get; set; }
            public double pp { get; set; }
            public int rank { get; set; }
            public int countryRank { get; set; }
            public string? role { get; set; }
            //public object[] socials { get; set; }
            //public object patreonFeatures { get; set; }
            //public object profileSettings { get; set; }
            public ClanResponse[] clans { get; set; } = Array.Empty<ClanResponse>();
        }

        public class PlayerScoreStats
        {
            public int id { get; set; }
            public long totalScore { get; set; }
            public long totalUnrankedScore { get; set; }
            public long totalRankedScore { get; set; }
            public int lastScoreTime { get; set; }
            public int lastUnrankedScoreTime { get; set; }
            public int lastRankedScoreTime { get; set; }
            public double averageRankedAccuracy { get; set; }
            public double averageWeightedRankedAccuracy { get; set; }
            public double averageUnrankedAccuracy { get; set; }
            public double averageAccuracy { get; set; }
            public double medianRankedAccuracy { get; set; }
            public double medianAccuracy { get; set; }
            public double topRankedAccuracy { get; set; }
            public double topUnrankedAccuracy { get; set; }
            public double topAccuracy { get; set; }
            public double topPp { get; set; }
            public double topBonusPP { get; set; }
            public int peakRank { get; set; }
            public int rankedMaxStreak { get; set; }
            public int unrankedMaxStreak { get; set; }
            public int maxStreak { get; set; }
            public double averageLeftTiming { get; set; }
            public double averageRightTiming { get; set; }
            public int rankedPlayCount { get; set; }
            public int unrankedPlayCount { get; set; }
            public int totalPlayCount { get; set; }
            public double averageRankedRank { get; set; }
            public double averageWeightedRankedRank { get; set; }
            public double averageUnrankedRank { get; set; }
            public double averageRank { get; set; }
            public int sspPlays { get; set; }
            public int ssPlays { get; set; }
            public int spPlays { get; set; }
            public int sPlays { get; set; }
            public int aPlays { get; set; }
            public string topPlatform { get; set; } = string.Empty;
            public int topHMD { get; set; }
            public int dailyImprovements { get; set; }
            public int authorizedReplayWatched { get; set; }
            public int anonimusReplayWatched { get; set; }
            public int watchedReplays { get; set; }
        }

        public class EventPlayer
        {
            public int id { get; set; }
            public int eventId { get; set; }
            public string? name { get; set; }
            public string? playerId { get; set; }
            public string? country { get; set; }
            public int rank { get; set; }
            public int countryRank { get; set; }
            public double pp { get; set; }
        }
    }
}
