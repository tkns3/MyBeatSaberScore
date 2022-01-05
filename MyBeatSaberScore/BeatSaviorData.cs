using MyBeatSaberScore.APIs;
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
        private static readonly string _mapsDir = Path.Combine("data", "maps");
        private static readonly string _rankedPath = Path.Combine(_mapsDir, "ranked.json");

        public BeatSavior.RankedMapCollection rankedMapCollection = new();
        public HashSet<string> rankedMapHashSet = new(); // ランクマップのHashSet。キーは「hash + difficulty(1～9)」。

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
                    var collection = JsonSerializer.Deserialize<BeatSavior.RankedMapCollection>(jsonString);
                    if (collection != null)
                    {
                        rankedMapHashSet.Clear();
                        foreach (var map in collection.maps)
                        {
                            map.hash = map.hash.ToLower();
                            map.coverURL = "https://cdn.scoresaber.com/covers/" + map.hash.ToUpper() + ".png";
                            if (map.diffs.easy != null) rankedMapHashSet.Add(map.hash + "1");
                            if (map.diffs.normal != null) rankedMapHashSet.Add(map.hash + "3");
                            if (map.diffs.hard != null) rankedMapHashSet.Add(map.hash + "5");
                            if (map.diffs.expert != null) rankedMapHashSet.Add(map.hash + "7");
                            if (map.diffs.expertplus != null) rankedMapHashSet.Add(map.hash + "9");
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
                var jsonString = JsonSerializer.Serialize<BeatSavior.RankedMapCollection>(rankedMapCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_rankedPath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }
        }

        public async Task DownloadRankedMaps()
        {
            rankedMapCollection = await BeatSavior.GetRankedMaps();

            rankedMapHashSet.Clear();
            foreach (var map in rankedMapCollection.maps)
            {
                if (map.diffs.easy != null) rankedMapHashSet.Add(map.hash + "1");
                if (map.diffs.normal != null) rankedMapHashSet.Add(map.hash + "3");
                if (map.diffs.hard != null) rankedMapHashSet.Add(map.hash + "5");
                if (map.diffs.expert != null) rankedMapHashSet.Add(map.hash + "7");
                if (map.diffs.expertplus != null) rankedMapHashSet.Add(map.hash + "9");
            }
        }

    }
 }
