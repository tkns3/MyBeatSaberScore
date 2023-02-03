using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyBeatSaberScore
{
    /// <summary>
    /// PageSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class PageSetting : Page
    {
        public PageSetting()
        {
            InitializeComponent();
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けを許可しない
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }

    internal class PageSettingViewModel : ObservableBase
    {
        public string GirdRowHeight
        {
            get
            {
                return Config.GridSetting.rowHeight.ToString();
            }
            set
            {
                if (int.TryParse(value, out int height))
                {
                    if (Config.GridSetting.rowHeight != height)
                    {
                        Config.GridSetting.rowHeight = height;
                        Config.SaveToLocalFile();
                        OnPropertyChanged();
                    }
                }
            }
        }

        public bool IsDisplayMapBsr { get => GetIsDisplay(Config.ColumnTagMapBsr); set => SetIsDisplay(Config.ColumnTagMapBsr, value); }
        public bool IsDisplayMapCover { get => GetIsDisplay(Config.ColumnTagMapCover); set => SetIsDisplay(Config.ColumnTagMapCover, value); }
        public bool IsDisplayMapSongName { get => GetIsDisplay(Config.ColumnTagMapSongName); set => SetIsDisplay(Config.ColumnTagMapSongName, value); }
        public bool IsDisplayMapMode { get => GetIsDisplay(Config.ColumnTagMapMode); set => SetIsDisplay(Config.ColumnTagMapMode, value); }
        public bool IsDisplayMapDifficulty { get => GetIsDisplay(Config.ColumnTagMapDifficulty); set => SetIsDisplay(Config.ColumnTagMapDifficulty, value); }
        public bool IsDisplayMapDuration { get => GetIsDisplay(Config.ColumnTagMapDuration); set => SetIsDisplay(Config.ColumnTagMapDuration, value); }
        public bool IsDisplayMapBpm { get => GetIsDisplay(Config.ColumnTagMapBpm); set => SetIsDisplay(Config.ColumnTagMapBpm, value); }
        public bool IsDisplayMapNotes { get => GetIsDisplay(Config.ColumnTagMapNotes); set => SetIsDisplay(Config.ColumnTagMapNotes, value); }
        public bool IsDisplayMapNps { get => GetIsDisplay(Config.ColumnTagMapNps); set => SetIsDisplay(Config.ColumnTagMapNps, value); }
        public bool IsDisplayMapNjs { get => GetIsDisplay(Config.ColumnTagMapNjs); set => SetIsDisplay(Config.ColumnTagMapNjs, value); }
        public bool IsDisplayMapBombs { get => GetIsDisplay(Config.ColumnTagMapBombs); set => SetIsDisplay(Config.ColumnTagMapBombs, value); }
        public bool IsDisplayMapWalls { get => GetIsDisplay(Config.ColumnTagMapWalls); set => SetIsDisplay(Config.ColumnTagMapWalls, value); }
        public bool IsDisplayMapHash { get => GetIsDisplay(Config.ColumnTagMapHash); set => SetIsDisplay(Config.ColumnTagMapHash, value); }
        public bool IsDisplayMapScoreSaberStar { get => GetIsDisplay(Config.ColumnTagMapScoreSaberStar); set => SetIsDisplay(Config.ColumnTagMapScoreSaberStar, value); }
        public bool IsDisplayMapScoreSaberRankedDate { get => GetIsDisplay(Config.ColumnTagMapScoreSaberRankedDate); set => SetIsDisplay(Config.ColumnTagMapScoreSaberRankedDate, value); }
        public bool IsDisplayMapBeatLeaderStar { get => GetIsDisplay(Config.ColumnTagMapBeatLeaderStar); set => SetIsDisplay(Config.ColumnTagMapBeatLeaderStar, value); }
        public bool IsDisplayMapBeatLeaderRankedDate { get => GetIsDisplay(Config.ColumnTagMapBeatLeaderRankedDate); set => SetIsDisplay(Config.ColumnTagMapBeatLeaderRankedDate, value); }
        public bool IsDisplayScoreSaberDate { get => GetIsDisplay(Config.ColumnTagScoreSaberDate); set => SetIsDisplay(Config.ColumnTagScoreSaberDate, value); }
        public bool IsDisplayScoreSaberScore { get => GetIsDisplay(Config.ColumnTagScoreSaberScore); set => SetIsDisplay(Config.ColumnTagScoreSaberScore, value); }
        public bool IsDisplayScoreSaberAcc { get => GetIsDisplay(Config.ColumnTagScoreSaberAcc); set => SetIsDisplay(Config.ColumnTagScoreSaberAcc, value); }
        public bool IsDisplayScoreSaberAccDiff { get => GetIsDisplay(Config.ColumnTagScoreSaberAccDiff); set => SetIsDisplay(Config.ColumnTagScoreSaberAccDiff, value); }
        public bool IsDisplayScoreSaberMissPlusBad { get => GetIsDisplay(Config.ColumnTagScoreSaberMissPlusBad); set => SetIsDisplay(Config.ColumnTagScoreSaberMissPlusBad, value); }
        public bool IsDisplayScoreSaberFullCombo { get => GetIsDisplay(Config.ColumnTagScoreSaberFullCombo); set => SetIsDisplay(Config.ColumnTagScoreSaberFullCombo, value); }
        public bool IsDisplayScoreSaberPp { get => GetIsDisplay(Config.ColumnTagScoreSaberPp); set => SetIsDisplay(Config.ColumnTagScoreSaberPp, value); }
        public bool IsDisplayScoreSaberModifiers { get => GetIsDisplay(Config.ColumnTagScoreSaberModifiers); set => SetIsDisplay(Config.ColumnTagScoreSaberModifiers, value); }
        public bool IsDisplayScoreSaberScoreCount { get => GetIsDisplay(Config.ColumnTagScoreSaberScoreCount); set => SetIsDisplay(Config.ColumnTagScoreSaberScoreCount, value); }
        public bool IsDisplayScoreSaberMiss { get => GetIsDisplay(Config.ColumnTagScoreSaberMiss); set => SetIsDisplay(Config.ColumnTagScoreSaberMiss, value); }
        public bool IsDisplayScoreSaberBad { get => GetIsDisplay(Config.ColumnTagScoreSaberBad); set => SetIsDisplay(Config.ColumnTagScoreSaberBad, value); }
        public bool IsDisplayScoreSaberWorldRank { get => GetIsDisplay(Config.ColumnTagScoreSaberWorldRank); set => SetIsDisplay(Config.ColumnTagScoreSaberWorldRank, value); }
        public bool IsDisplayBeatLeaderDate { get => GetIsDisplay(Config.ColumnTagBeatLeaderDate); set => SetIsDisplay(Config.ColumnTagBeatLeaderDate, value); }
        public bool IsDisplayBeatLeaderScore { get => GetIsDisplay(Config.ColumnTagBeatLeaderScore); set => SetIsDisplay(Config.ColumnTagBeatLeaderScore, value); }
        public bool IsDisplayBeatLeaderAcc { get => GetIsDisplay(Config.ColumnTagBeatLeaderAcc); set => SetIsDisplay(Config.ColumnTagBeatLeaderAcc, value); }
        public bool IsDisplayBeatLeaderAccDiff { get => GetIsDisplay(Config.ColumnTagBeatLeaderAccDiff); set => SetIsDisplay(Config.ColumnTagBeatLeaderAccDiff, value); }
        public bool IsDisplayBeatLeaderMissPlusBad { get => GetIsDisplay(Config.ColumnTagBeatLeaderMissPlusBad); set => SetIsDisplay(Config.ColumnTagBeatLeaderMissPlusBad, value); }
        public bool IsDisplayBeatLeaderFullCombo { get => GetIsDisplay(Config.ColumnTagBeatLeaderFullCombo); set => SetIsDisplay(Config.ColumnTagBeatLeaderFullCombo, value); }
        public bool IsDisplayBeatLeaderPp { get => GetIsDisplay(Config.ColumnTagBeatLeaderPp); set => SetIsDisplay(Config.ColumnTagBeatLeaderPp, value); }
        public bool IsDisplayBeatLeaderModifiers { get => GetIsDisplay(Config.ColumnTagBeatLeaderModifiers); set => SetIsDisplay(Config.ColumnTagBeatLeaderModifiers, value); }
        public bool IsDisplayBeatLeaderScoreCount { get => GetIsDisplay(Config.ColumnTagBeatLeaderScoreCount); set => SetIsDisplay(Config.ColumnTagBeatLeaderScoreCount, value); }
        public bool IsDisplayBeatLeaderMiss { get => GetIsDisplay(Config.ColumnTagBeatLeaderMiss); set => SetIsDisplay(Config.ColumnTagBeatLeaderMiss, value); }
        public bool IsDisplayBeatLeaderBad { get => GetIsDisplay(Config.ColumnTagBeatLeaderBad); set => SetIsDisplay(Config.ColumnTagBeatLeaderBad, value); }
        public bool IsDisplayBeatLeaderWorldRank { get => GetIsDisplay(Config.ColumnTagBeatLeaderWorldRank); set => SetIsDisplay(Config.ColumnTagBeatLeaderWorldRank, value); }
        public bool IsDisplayCheckBox { get => GetIsDisplay(Config.ColumnTagCheckBox); set => SetIsDisplay(Config.ColumnTagCheckBox, value); }
        public bool IsDisplayCopyBsr { get => GetIsDisplay(Config.ColumnTagCopyBsr); set => SetIsDisplay(Config.ColumnTagCopyBsr, value); }
        public bool IsDisplayJumpBeatSaver { get => GetIsDisplay(Config.ColumnTagJumpBeatSaver); set => SetIsDisplay(Config.ColumnTagJumpBeatSaver, value); }
        public bool IsDisplayJumpScoreSaber { get => GetIsDisplay(Config.ColumnTagJumpScoreSaber); set => SetIsDisplay(Config.ColumnTagJumpScoreSaber, value); }
        public bool IsDisplayJumpBeatLeader { get => GetIsDisplay(Config.ColumnTagJumpBeatLeader); set => SetIsDisplay(Config.ColumnTagJumpBeatLeader, value); }

        private bool GetIsDisplay(string name)
        {
            return !Config.GridSetting.notDisplayColumns.Contains(name);
        }

        private void SetIsDisplay(string name, bool value)
        {
            if (value)
            {
                if (Config.GridSetting.notDisplayColumns.Contains(name))
                {
                    Config.GridSetting.notDisplayColumns.Remove(name);
                    Config.SaveToLocalFile();
                }
            }
            else
            {
                if (!Config.GridSetting.notDisplayColumns.Contains(name))
                {
                    Config.GridSetting.notDisplayColumns.Add(name);
                    Config.SaveToLocalFile();
                }
            }
        }
    }
}
