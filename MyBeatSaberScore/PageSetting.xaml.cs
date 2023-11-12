using MyBeatSaberScore.Utility;
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
        readonly PageSettingViewModel _model;

        public PageSetting()
        {
            InitializeComponent();
            _model = (PageSettingViewModel)DataContext;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けを許可しない
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void ButtonSaveWindowBounds_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Instance != null)
            {
                _model.WindowBoundsSavedDate = DateTimeOffset.Now;
                Config.Window.boundsRestore.saved = MainWindow.Instance.GetWindowBounds();
                Config.SaveToLocalFile();
            }
        }

        private void ButtonSaveColumnParams_Click(object sender, RoutedEventArgs e)
        {
            if (PageMain.Instance != null)
            {
                var columnParams = PageMain.Instance.GetGridColumnParams();
                if (columnParams != null)
                {
                    _model.ColumnParamsSavedDate = DateTimeOffset.Now;
                    Config.Grid.columnRestore.savedParams.Clear();
                    Config.Grid.columnRestore.savedParams.AddRange(columnParams);
                    Config.SaveToLocalFile();
                }
            }
        }
    }

    internal class PageSettingViewModel : ObservableBase
    {
        public string GirdRowHeight
        {
            get
            {
                return Config.Grid.rowHeight.ToString();
            }
            set
            {
                if (int.TryParse(value, out int height))
                {
                    if (Config.Grid.rowHeight != height)
                    {
                        Config.Grid.rowHeight = height;
                        Config.SaveToLocalFile();
                        OnPropertyChanged();
                    }
                }
            }
        }

        public bool IsDisplayRowNumber
        {
            get
            {
                return Config.Grid.displayRowNumber;
            }
            set
            {
                Config.Grid.displayRowNumber = value;
                OnPropertyChanged();
            }
        }

        public static Config.RestoreMode WindowBoundsRestoreMode
        {
            get
            {
                return Config.Window.boundsRestore.mode;
            }
            set
            {
                Config.Window.boundsRestore.mode = value;
            }
        }

        public DateTimeOffset? WindowBoundsSavedDate
        {
            get
            {
                return Config.Window.boundsRestore.savedDate;
            }
            set
            {
                Config.Window.boundsRestore.savedDate = value;
                OnPropertyChanged();
            }
        }

        public static Config.RestoreMode ColumnParamsRestoreMode
        {
            get
            {
                return Config.Grid.columnRestore.mode;
            }
            set
            {
                Config.Grid.columnRestore.mode = value;
            }
        }

        public DateTimeOffset? ColumnParamsSavedDate
        {
            get
            {
                return Config.Grid.columnRestore.savedDate;
            }
            set
            {
                Config.Grid.columnRestore.savedDate = value;
                OnPropertyChanged();
            }
        }

        public static bool IsDisplayMapBsr { get => GetIsDisplay(Config.ColumnTagMapBsr); set => SetIsDisplay(Config.ColumnTagMapBsr, value); }
        public static bool IsDisplayMapCover { get => GetIsDisplay(Config.ColumnTagMapCover); set => SetIsDisplay(Config.ColumnTagMapCover, value); }
        public static bool IsDisplayMapSongName { get => GetIsDisplay(Config.ColumnTagMapSongName); set => SetIsDisplay(Config.ColumnTagMapSongName, value); }
        public static bool IsDisplayMapMode { get => GetIsDisplay(Config.ColumnTagMapMode); set => SetIsDisplay(Config.ColumnTagMapMode, value); }
        public static bool IsDisplayMapDifficulty { get => GetIsDisplay(Config.ColumnTagMapDifficulty); set => SetIsDisplay(Config.ColumnTagMapDifficulty, value); }
        public static bool IsDisplayMapDuration { get => GetIsDisplay(Config.ColumnTagMapDuration); set => SetIsDisplay(Config.ColumnTagMapDuration, value); }
        public static bool IsDisplayMapBpm { get => GetIsDisplay(Config.ColumnTagMapBpm); set => SetIsDisplay(Config.ColumnTagMapBpm, value); }
        public static bool IsDisplayMapNotes { get => GetIsDisplay(Config.ColumnTagMapNotes); set => SetIsDisplay(Config.ColumnTagMapNotes, value); }
        public static bool IsDisplayMapNps { get => GetIsDisplay(Config.ColumnTagMapNps); set => SetIsDisplay(Config.ColumnTagMapNps, value); }
        public static bool IsDisplayMapNjs { get => GetIsDisplay(Config.ColumnTagMapNjs); set => SetIsDisplay(Config.ColumnTagMapNjs, value); }
        public static bool IsDisplayMapBombs { get => GetIsDisplay(Config.ColumnTagMapBombs); set => SetIsDisplay(Config.ColumnTagMapBombs, value); }
        public static bool IsDisplayMapWalls { get => GetIsDisplay(Config.ColumnTagMapWalls); set => SetIsDisplay(Config.ColumnTagMapWalls, value); }
        public static bool IsDisplayMapHash { get => GetIsDisplay(Config.ColumnTagMapHash); set => SetIsDisplay(Config.ColumnTagMapHash, value); }
        public static bool IsDisplayMapScoreSaberStar { get => GetIsDisplay(Config.ColumnTagMapScoreSaberStar); set => SetIsDisplay(Config.ColumnTagMapScoreSaberStar, value); }
        public static bool IsDisplayMapScoreSaberRankedDate { get => GetIsDisplay(Config.ColumnTagMapScoreSaberRankedDate); set => SetIsDisplay(Config.ColumnTagMapScoreSaberRankedDate, value); }
        public static bool IsDisplayMapBeatLeaderStar { get => GetIsDisplay(Config.ColumnTagMapBeatLeaderStar); set => SetIsDisplay(Config.ColumnTagMapBeatLeaderStar, value); }
        public static bool IsDisplayMapBeatLeaderRankedDate { get => GetIsDisplay(Config.ColumnTagMapBeatLeaderRankedDate); set => SetIsDisplay(Config.ColumnTagMapBeatLeaderRankedDate, value); }
        public static bool IsDisplayScoreSaberDate { get => GetIsDisplay(Config.ColumnTagScoreSaberDate); set => SetIsDisplay(Config.ColumnTagScoreSaberDate, value); }
        public static bool IsDisplayScoreSaberScore { get => GetIsDisplay(Config.ColumnTagScoreSaberScore); set => SetIsDisplay(Config.ColumnTagScoreSaberScore, value); }
        public static bool IsDisplayScoreSaberAcc { get => GetIsDisplay(Config.ColumnTagScoreSaberAcc); set => SetIsDisplay(Config.ColumnTagScoreSaberAcc, value); }
        public static bool IsDisplayScoreSaberAccDiff { get => GetIsDisplay(Config.ColumnTagScoreSaberAccDiff); set => SetIsDisplay(Config.ColumnTagScoreSaberAccDiff, value); }
        public static bool IsDisplayScoreSaberMissPlusBad { get => GetIsDisplay(Config.ColumnTagScoreSaberMissPlusBad); set => SetIsDisplay(Config.ColumnTagScoreSaberMissPlusBad, value); }
        public static bool IsDisplayScoreSaberFullCombo { get => GetIsDisplay(Config.ColumnTagScoreSaberFullCombo); set => SetIsDisplay(Config.ColumnTagScoreSaberFullCombo, value); }
        public static bool IsDisplayScoreSaberPp { get => GetIsDisplay(Config.ColumnTagScoreSaberPp); set => SetIsDisplay(Config.ColumnTagScoreSaberPp, value); }
        public static bool IsDisplayScoreSaberModifiers { get => GetIsDisplay(Config.ColumnTagScoreSaberModifiers); set => SetIsDisplay(Config.ColumnTagScoreSaberModifiers, value); }
        public static bool IsDisplayScoreSaberScoreCount { get => GetIsDisplay(Config.ColumnTagScoreSaberScoreCount); set => SetIsDisplay(Config.ColumnTagScoreSaberScoreCount, value); }
        public static bool IsDisplayScoreSaberMiss { get => GetIsDisplay(Config.ColumnTagScoreSaberMiss); set => SetIsDisplay(Config.ColumnTagScoreSaberMiss, value); }
        public static bool IsDisplayScoreSaberBad { get => GetIsDisplay(Config.ColumnTagScoreSaberBad); set => SetIsDisplay(Config.ColumnTagScoreSaberBad, value); }
        public static bool IsDisplayScoreSaberWorldRank { get => GetIsDisplay(Config.ColumnTagScoreSaberWorldRank); set => SetIsDisplay(Config.ColumnTagScoreSaberWorldRank, value); }
        public static bool IsDisplayBeatLeaderDate { get => GetIsDisplay(Config.ColumnTagBeatLeaderDate); set => SetIsDisplay(Config.ColumnTagBeatLeaderDate, value); }
        public static bool IsDisplayBeatLeaderScore { get => GetIsDisplay(Config.ColumnTagBeatLeaderScore); set => SetIsDisplay(Config.ColumnTagBeatLeaderScore, value); }
        public static bool IsDisplayBeatLeaderAcc { get => GetIsDisplay(Config.ColumnTagBeatLeaderAcc); set => SetIsDisplay(Config.ColumnTagBeatLeaderAcc, value); }
        public static bool IsDisplayBeatLeaderAccDiff { get => GetIsDisplay(Config.ColumnTagBeatLeaderAccDiff); set => SetIsDisplay(Config.ColumnTagBeatLeaderAccDiff, value); }
        public static bool IsDisplayBeatLeaderMissPlusBad { get => GetIsDisplay(Config.ColumnTagBeatLeaderMissPlusBad); set => SetIsDisplay(Config.ColumnTagBeatLeaderMissPlusBad, value); }
        public static bool IsDisplayBeatLeaderFullCombo { get => GetIsDisplay(Config.ColumnTagBeatLeaderFullCombo); set => SetIsDisplay(Config.ColumnTagBeatLeaderFullCombo, value); }
        public static bool IsDisplayBeatLeaderPp { get => GetIsDisplay(Config.ColumnTagBeatLeaderPp); set => SetIsDisplay(Config.ColumnTagBeatLeaderPp, value); }
        public static bool IsDisplayBeatLeaderModifiers { get => GetIsDisplay(Config.ColumnTagBeatLeaderModifiers); set => SetIsDisplay(Config.ColumnTagBeatLeaderModifiers, value); }
        public static bool IsDisplayBeatLeaderScoreCount { get => GetIsDisplay(Config.ColumnTagBeatLeaderScoreCount); set => SetIsDisplay(Config.ColumnTagBeatLeaderScoreCount, value); }
        public static bool IsDisplayBeatLeaderMiss { get => GetIsDisplay(Config.ColumnTagBeatLeaderMiss); set => SetIsDisplay(Config.ColumnTagBeatLeaderMiss, value); }
        public static bool IsDisplayBeatLeaderBad { get => GetIsDisplay(Config.ColumnTagBeatLeaderBad); set => SetIsDisplay(Config.ColumnTagBeatLeaderBad, value); }
        public static bool IsDisplayBeatLeaderWorldRank { get => GetIsDisplay(Config.ColumnTagBeatLeaderWorldRank); set => SetIsDisplay(Config.ColumnTagBeatLeaderWorldRank, value); }
        public static bool IsDisplayCheckBox { get => GetIsDisplay(Config.ColumnTagCheckBox); set => SetIsDisplay(Config.ColumnTagCheckBox, value); }
        public static bool IsDisplayCopyBsr { get => GetIsDisplay(Config.ColumnTagCopyBsr); set => SetIsDisplay(Config.ColumnTagCopyBsr, value); }
        public static bool IsDisplayJumpBeatSaver { get => GetIsDisplay(Config.ColumnTagJumpBeatSaver); set => SetIsDisplay(Config.ColumnTagJumpBeatSaver, value); }
        public static bool IsDisplayJumpScoreSaber { get => GetIsDisplay(Config.ColumnTagJumpScoreSaber); set => SetIsDisplay(Config.ColumnTagJumpScoreSaber, value); }
        public static bool IsDisplayJumpBeatLeader { get => GetIsDisplay(Config.ColumnTagJumpBeatLeader); set => SetIsDisplay(Config.ColumnTagJumpBeatLeader, value); }

        private static bool GetIsDisplay(string name)
        {
            return !Config.Grid.notDisplayColumns.Contains(name);
        }

        private static void SetIsDisplay(string name, bool value)
        {
            if (value)
            {
                if (Config.Grid.notDisplayColumns.Contains(name))
                {
                    Config.Grid.notDisplayColumns.Remove(name);
                    Config.SaveToLocalFile();
                }
            }
            else
            {
                if (!Config.Grid.notDisplayColumns.Contains(name))
                {
                    Config.Grid.notDisplayColumns.Add(name);
                    Config.SaveToLocalFile();
                }
            }
        }
    }
}
