﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyBeatSaberScore
{
    internal class PlayList
    {
        private Container _container;

        public string Title
        {
            get { return _container.playlistTitle; }
            set { _container.playlistTitle = value; }
        }

        public string Author
        {
            get { return _container.playlistAuthor; }
            set { _container.playlistAuthor = value; }
        }

        public string Description
        {
            get { return _container.playlistDescription; }
            set { _container.playlistDescription = value; }
        }

        public PlayList()
        {
            _container = new Container();
        }

        public void AddSong(string key, string hash, string songName, string levelAuthor, string mode, long difficultyInt)
        {
            _container.songs.Add(new Song()
            {
                key = key,
                songName = songName,
                levelAuthorName = levelAuthor,
                hash = hash,
                levelid = $"custom_level_{hash}",
                difficulties = new List<Difficulty>() { new Difficulty(mode, difficultyInt) }
            });
        }

        public void Save(string path)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize<Container>(_container, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + ex.ToString());
            }
        }

        [DataContract]
        public class Container
        {
            [DataMember]
            public string playlistTitle { get; set; }

            [DataMember]
            public string playlistAuthor { get; set; }

            [DataMember]
            public string playlistDescription { get; set; }

            [DataMember]
            public List<Song> songs { get; set; }

            [DataMember]
            public string image { get; set; }

            public Container()
            {
                playlistTitle = "Maps By MyBeatSaberScore";
                playlistAuthor = "MyBeatSaberScore";
                playlistDescription = "Generated by MyBeatSaberScore";
                songs = new();
                image = "base64,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";
            }
        }

        [DataContract]
        public class Song
        {
            [DataMember]
            public string key { get; set; }

            [DataMember]
            public string songName { get; set; }

            [DataMember]
            public string levelAuthorName { get; set; }

            [DataMember]
            public string hash { get; set; }

            [DataMember]
            public string levelid { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public List<Difficulty> difficulties { get; set; }

            public Song()
            {
                key = "";
                songName = "";
                levelAuthorName = "";
                hash = "";
                levelid = "";
                difficulties = new();
            }
        }

        [DataContract]
        public class Difficulty
        {
            [DataMember]
            public string characteristic { get; set; }

            [DataMember]
            public string name { get; set; }

            public Difficulty()
            {
                characteristic = "";
                name = "";
            }

            public Difficulty(string mode, long difficultyInt)
            {
                characteristic = mode.Substring(4);
                switch (difficultyInt)
                {
                    case 1: name = "Easy"; break;
                    case 3: name = "Normal"; break;
                    case 5: name = "Hard"; break;
                    case 7: name = "Expert"; break;
                    case 9: name = "ExpertPlus"; break;
                    default: name = "ExpertPlus"; break;
                }
            }
        }
    }
}