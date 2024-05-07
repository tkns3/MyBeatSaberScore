using MyBeatSaberScore.APIs;
using System;

namespace MyBeatSaberScore.BeatMap
{
    public class BeatMapData
    {
        public string Key { get; set; } = "";
        public string Hash { get; set; } = "";
        public string SongName { get; set; } = "";
        public string SongSubName { get; set; } = "";
        public string SongAuthorName { get; set; } = "";
        public string MapperName { get; set; } = "";
        public DateTimeOffset UploadedTime { get; set; } = new();
        public double Bpm { get; set; }
        public double Duration { get; set; }
        public BeatMapDifficulty MapDifficulty { get; set; }
        public BeatMapMode MapMode { get; set; }
        public long Bombs { get; set; }
        public long Notes { get; set; }
        public long Walls { get; set; }
        public double Njs { get; set; }
        public double Nps { get; set; }
        /// <summary>
        /// 理論最大スコアの値。
        /// ただしアークノーツ、チェインノーツを含むマップの場合は正しくない値が格納されている可能性あり。
        /// </summary>
        public long MaxScore { get; set; }
        public RankedInformation ScoreSaber { get; set; } = new();
        public RankedInformation BeatLeader { get; set; } = new();
        public bool Deleted { get; set; }

        public BeatMapData Clone()
        {
            var map = new BeatMapData();
            map.Key = Key;
            map.Hash = Hash;
            map.SongName = SongName;
            map.SongSubName = SongSubName;
            map.SongAuthorName = SongAuthorName;
            map.MapperName = MapperName;
            map.UploadedTime = UploadedTime;
            map.Bpm = Bpm;
            map.Duration = Duration;
            map.MapDifficulty = MapDifficulty;
            map.MapMode = MapMode;
            map.Bpm = Bpm;
            map.Notes = Notes;
            map.Walls = Walls;
            map.Njs = Njs;
            map.Nps = Nps;
            map.MaxScore = MaxScore;
            map.ScoreSaber.Ranked = ScoreSaber.Ranked;
            map.ScoreSaber.RankedTime = ScoreSaber.RankedTime;
            map.ScoreSaber.Star = ScoreSaber.Star;
            map.BeatLeader.Ranked = BeatLeader.Ranked;
            map.BeatLeader.RankedTime = BeatLeader.RankedTime;
            map.BeatLeader.Star = BeatLeader.Star;
            map.Deleted = Deleted;
            return map;
        }
    }

    public class RankedInformation
    {
        public bool Ranked { get; set; } = false;
        public DateTimeOffset? RankedTime { get; set; }
        public double Star { get; set; }
    }
}
