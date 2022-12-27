using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyBeatSaberScore
{
    /// <summary>
    /// PageSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class PageSetting : Page
    {
        private readonly bool _InitializeFinished = false;

        public PageSetting()
        {
            InitializeComponent();

            XaRowHeight.Text = Config.GridSetting.rowHeight.ToString();

            DispCheckBoxInitialize(XaDispCheckBox, Config.ColumnTagCheckBox);
            DispCheckBoxInitialize(XaDispBsr, Config.ColumnTagBsr);
            DispCheckBoxInitialize(XaDispCover, Config.ColumnTagCover);
            DispCheckBoxInitialize(XaDispSongName, Config.ColumnTagSongName);
            DispCheckBoxInitialize(XaDispDate, Config.ColumnTagDate);
            DispCheckBoxInitialize(XaDispMode, Config.ColumnTagMode);
            DispCheckBoxInitialize(XaDispDifficulty, Config.ColumnTagDifficulty);
            DispCheckBoxInitialize(XaDispStars, Config.ColumnTagStars);
            DispCheckBoxInitialize(XaDispScore, Config.ColumnTagScore);
            DispCheckBoxInitialize(XaDispAcc, Config.ColumnTagAcc);
            DispCheckBoxInitialize(XaDispAccDiff, Config.ColumnTagAccDiff);
            DispCheckBoxInitialize(XaDispMissPlusBad, Config.ColumnTagMissPlusBad);
            DispCheckBoxInitialize(XaDispFullCombo, Config.ColumnTagFullCombo);
            DispCheckBoxInitialize(XaDispPp, Config.ColumnTagPp);
            DispCheckBoxInitialize(XaDispModifiers, Config.ColumnTagModifiers);
            DispCheckBoxInitialize(XaDispScoreCount, Config.ColumnTagScoreCount);
            DispCheckBoxInitialize(XaDispCopyBsr, Config.ColumnTagCopyBsr);
            DispCheckBoxInitialize(XaDispJumpBeatSaver, Config.ColumnTagJumpBeatSaver);
            DispCheckBoxInitialize(XaDispJumpScoreSaber, Config.ColumnTagJumpScoreSaber);
            DispCheckBoxInitialize(XaDispJumpBeatLeader, Config.ColumnTagJumpBeatLeader);
            DispCheckBoxInitialize(XaDispDuration, Config.ColumnTagDuration);
            DispCheckBoxInitialize(XaDispBpm, Config.ColumnTagBpm);
            DispCheckBoxInitialize(XaDispNotes, Config.ColumnTagNotes);
            DispCheckBoxInitialize(XaDispNps, Config.ColumnTagNps);
            DispCheckBoxInitialize(XaDispNjs, Config.ColumnTagNjs);
            DispCheckBoxInitialize(XaDispBombs, Config.ColumnTagBombs);
            DispCheckBoxInitialize(XaDispObstacles, Config.ColumnTagObstacles);
            DispCheckBoxInitialize(XaDispMiss, Config.ColumnTagMiss);
            DispCheckBoxInitialize(XaDispBad, Config.ColumnTagBad);
            DispCheckBoxInitialize(XaDispHash, Config.ColumnTagHash);
            DispCheckBoxInitialize(XaDispRankedDate, Config.ColumnTagRankedDate);
            DispCheckBoxInitialize(XaDispScoreRank, Config.ColumnTagScoreRank);

            _InitializeFinished = true;
        }

        private void OnRowHeightChanged(object sender, TextChangedEventArgs e)
        {
            if (Application.Current.Properties["XaDataGrid"] is DataGrid dataGrid)
            {
                if (Int32.TryParse(XaRowHeight.Text, out int height))
                {
                    dataGrid.RowHeight = height;

                    // カバー画像のサイズもあわせて変更する
                    var column = dataGrid.Columns.SingleOrDefault(p => TagBehavior.GetTag(p).ToString() == Config.ColumnTagCover);
                    if (column != null)
                    {
                        column.Width = height;
                    }

                    if (_InitializeFinished)
                    {
                        Config.GridSetting.rowHeight = height;
                        Config.SaveLocalFile();
                    }
                }
            }
        }

        private void DispCheckBoxInitialize(CheckBox checkBox, string tagName)
        {
            if (Application.Current.Properties["XaDataGrid"] is DataGrid dataGrid)
            {
                var column = dataGrid.Columns.SingleOrDefault(p => TagBehavior.GetTag(p).ToString() == tagName);
                if (column != null)
                {
                    checkBox.IsChecked = !Config.GridSetting.notDisplayColumns.Contains(tagName);
                    column.Visibility = (checkBox.IsChecked ?? false) ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        private void DispCheckBoxChanged(CheckBox checkBox, string tagName)
        {
            if (Application.Current.Properties["XaDataGrid"] is DataGrid dataGrid)
            {
                var column = dataGrid.Columns.SingleOrDefault(p => TagBehavior.GetTag(p).ToString() == tagName);
                if (column != null)
                {
                    if (!checkBox.IsChecked ?? false)
                    {
                        Config.GridSetting.notDisplayColumns.Add(tagName);
                        column.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        column.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void OnCheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (!_InitializeFinished) return;

            Config.GridSetting.notDisplayColumns.Clear();
            DispCheckBoxChanged(XaDispCheckBox, Config.ColumnTagCheckBox);
            DispCheckBoxChanged(XaDispBsr, Config.ColumnTagBsr);
            DispCheckBoxChanged(XaDispCover, Config.ColumnTagCover);
            DispCheckBoxChanged(XaDispSongName, Config.ColumnTagSongName);
            DispCheckBoxChanged(XaDispDate, Config.ColumnTagDate);
            DispCheckBoxChanged(XaDispMode, Config.ColumnTagMode);
            DispCheckBoxChanged(XaDispDifficulty, Config.ColumnTagDifficulty);
            DispCheckBoxChanged(XaDispStars, Config.ColumnTagStars);
            DispCheckBoxChanged(XaDispScore, Config.ColumnTagScore);
            DispCheckBoxChanged(XaDispAcc, Config.ColumnTagAcc);
            DispCheckBoxChanged(XaDispAccDiff, Config.ColumnTagAccDiff);
            DispCheckBoxChanged(XaDispMissPlusBad, Config.ColumnTagMissPlusBad);
            DispCheckBoxChanged(XaDispFullCombo, Config.ColumnTagFullCombo);
            DispCheckBoxChanged(XaDispPp, Config.ColumnTagPp);
            DispCheckBoxChanged(XaDispModifiers, Config.ColumnTagModifiers);
            DispCheckBoxChanged(XaDispScoreCount, Config.ColumnTagScoreCount);
            DispCheckBoxChanged(XaDispCopyBsr, Config.ColumnTagCopyBsr);
            DispCheckBoxChanged(XaDispJumpBeatSaver, Config.ColumnTagJumpBeatSaver);
            DispCheckBoxChanged(XaDispJumpScoreSaber, Config.ColumnTagJumpScoreSaber);
            DispCheckBoxChanged(XaDispJumpBeatLeader, Config.ColumnTagJumpBeatLeader);
            DispCheckBoxChanged(XaDispDuration, Config.ColumnTagDuration);
            DispCheckBoxChanged(XaDispBpm, Config.ColumnTagBpm);
            DispCheckBoxChanged(XaDispNotes, Config.ColumnTagNotes);
            DispCheckBoxChanged(XaDispNps, Config.ColumnTagNps);
            DispCheckBoxChanged(XaDispNjs, Config.ColumnTagNjs);
            DispCheckBoxChanged(XaDispBombs, Config.ColumnTagBombs);
            DispCheckBoxChanged(XaDispObstacles, Config.ColumnTagObstacles);
            DispCheckBoxChanged(XaDispMiss, Config.ColumnTagMiss);
            DispCheckBoxChanged(XaDispBad, Config.ColumnTagBad);
            DispCheckBoxChanged(XaDispHash, Config.ColumnTagHash);
            DispCheckBoxChanged(XaDispRankedDate, Config.ColumnTagRankedDate);
            DispCheckBoxChanged(XaDispScoreRank, Config.ColumnTagScoreRank);

            Config.SaveLocalFile();
        }
    }
}
