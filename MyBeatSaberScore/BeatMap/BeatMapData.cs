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
    }

    public class RankedInformation
    {
        public bool Ranked { get; set; } = false;
        public DateTimeOffset? RankedTime { get; set; }
        public double Star { get; set; }
    }
}
