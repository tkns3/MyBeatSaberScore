using Microsoft.Win32;
using MyBeatSaberScore.APIs;
using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Model;
using MyBeatSaberScore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MyBeatSaberScore
{
    /// <summary>
    /// PageMain.xaml の相互作用ロジック
    /// </summary>
    public partial class PageMain : Page
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private PageMainViewModel _viewModel;

        public PageMain()
        {
            InitializeComponent();
            _viewModel = (PageMainViewModel)DataContext;
            _viewModel.GridItemsViewSource.Filter += new FilterEventHandler(DataGridFilter);
        }

        private void RefreshGrid()
        {
            try
            {
                if (XaDataGrid != null)
                {
                    XaDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                    _viewModel.GridItemsViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                // 強制終了よりはましそうなので例外を握りつぶす
                _logger.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// itemが曲名、BSR、HASHの指定がすべて満たしているかどうか。
        /// </summary>
        /// <param name="item"></param>
        /// <returns>曲名、BSR、HASHの指定をすべて満たしたitemである場合にtrue</returns>
        private bool FilterBySearch(IntegrationScore item)
        {
            if (AppData.FilterValue.SongName.Length > 0)
            {
                if (!item.SongFullName.Contains(AppData.FilterValue.SongName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (AppData.FilterValue.Bsr.Length > 0)
            {
                if (!item.Map.Key.Contains(AppData.FilterValue.Bsr, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (AppData.FilterValue.Hash.Length > 0)
            {
                if (!item.Map.Hash.Contains(AppData.FilterValue.Hash, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private bool FilterByMapGameMode(IntegrationScore item)
        {
            bool isShow = item.Map.MapMode switch
            {
                BeatMapMode.Standard => AppData.FilterValue.IsShowStandard,
                BeatMapMode.OneSaber => AppData.FilterValue.IsShowOneSaber,
                BeatMapMode.NoArrows => AppData.FilterValue.IsShowNoArrows,
                BeatMapMode.Degree90 => AppData.FilterValue.IsShow90Degree,
                BeatMapMode.Degree360 => AppData.FilterValue.IsShow360Degree,
                BeatMapMode.Lightshow => AppData.FilterValue.IsShowLightShow,
                BeatMapMode.Lawless => AppData.FilterValue.IsShowLawless,
                _ => true,
            };
            return isShow;
        }

        private bool FilterByMapGameDifficulty(IntegrationScore item)
        {
            bool isShow = item.Map.MapDifficulty switch
            {
                BeatMapDifficulty.Easy => AppData.FilterValue.IsShowEasy,
                BeatMapDifficulty.Normal => AppData.FilterValue.IsShowNormal,
                BeatMapDifficulty.Hard => AppData.FilterValue.IsShowHard,
                BeatMapDifficulty.Expert => AppData.FilterValue.IsShowExpert,
                BeatMapDifficulty.ExpertPlus => AppData.FilterValue.IsShowExpertPlus,
                _ => true,
            };
            return isShow;
        }

        private bool FilterByMapStatus(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                if (item.Map.BeatLeader.Ranked)
                {
                    return AppData.FilterValue.IsShowRank;
                }
                else
                {
                    return AppData.FilterValue.IsShowUnRank;
                }
            }
            else
            {
                if (item.Map.ScoreSaber.Ranked)
                {
                    return AppData.FilterValue.IsShowRank;
                }
                else
                {
                    return AppData.FilterValue.IsShowUnRank;
                }
            }
        }

        private bool FilterByMapStar(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                if (item.Map.BeatLeader.Ranked)
                {
                    return (AppData.FilterValue.MinStar <= item.Map.BeatLeader.Star && item.Map.BeatLeader.Star < AppData.FilterValue.MaxStar);
                }

                return true;
            }
            else
            {
                if (item.Map.ScoreSaber.Ranked)
                {
                    return (AppData.FilterValue.MinStar <= item.Map.ScoreSaber.Star && item.Map.ScoreSaber.Star < AppData.FilterValue.MaxStar);
                }

                return true;
            }
        }

        private bool FilterByMapRankedDate(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                if (item.Map.BeatLeader.RankedTime != null)
                {
                    if (AppData.FilterValue.RankedDateStart != null)
                    {
                        if (item.Map.BeatLeader.RankedTime < AppData.FilterValue.RankedDateStart)
                        {
                            return false;
                        }
                    }

                    if (AppData.FilterValue.RankedDateEnd != null)
                    {
                        if (item.Map.BeatLeader.RankedTime > AppData.FilterValue.RankedDateEnd)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                if (item.Map.ScoreSaber.RankedTime != null)
                {
                    if (AppData.FilterValue.RankedDateStart != null)
                    {
                        if (item.Map.ScoreSaber.RankedTime < AppData.FilterValue.RankedDateStart)
                        {
                            return false;
                        }
                    }

                    if (AppData.FilterValue.RankedDateEnd != null)
                    {
                        if (item.Map.ScoreSaber.RankedTime > AppData.FilterValue.RankedDateEnd)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        private bool FilterByResultPp(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                if (item.Map.BeatLeader.Ranked)
                {
                    return (AppData.FilterValue.MinPp <= item.BeatLeader.PP && item.BeatLeader.PP < AppData.FilterValue.MaxPp);
                }

                return true;
            }
            else
            {
                if (item.Map.ScoreSaber.Ranked)
                {
                    return (AppData.FilterValue.MinPp <= item.ScoreSaber.PP && item.ScoreSaber.PP < AppData.FilterValue.MaxPp);
                }

                return true;
            }
        }

        private bool IsFailureByConfig(string modifiers)
        {
            if (modifiers.Length > 0)
            {
                foreach (var f in Config.Failures)
                {
                    if (modifiers.Contains(f))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool FilterByResultFullCombo(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                if (item.BeatLeader.FullCombo.Length > 0)
                {
                    return AppData.FilterValue.IsShowFullCombo;
                }
                else
                {
                    return AppData.FilterValue.IsShowNotFullCombo;
                }
            }
            else
            {
                if (item.ScoreSaber.FullCombo.Length > 0)
                {
                    return AppData.FilterValue.IsShowFullCombo;
                }
                else
                {
                    return AppData.FilterValue.IsShowNotFullCombo;
                }
            }
        }

        private bool FilterByResultScoreUpdateDate(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                if (item.BeatLeader.TimeSet != null)
                {
                    if (AppData.FilterValue.DateStart != null)
                    {
                        if (item.BeatLeader.TimeSet < AppData.FilterValue.DateStart)
                        {
                            return false;
                        }
                    }

                    if (AppData.FilterValue.DateEnd != null)
                    {
                        if (item.BeatLeader.TimeSet > AppData.FilterValue.DateEnd)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                if (item.ScoreSaber.TimeSet != null)
                {
                    if (AppData.FilterValue.DateStart != null)
                    {
                        if (item.ScoreSaber.TimeSet < AppData.FilterValue.DateStart)
                        {
                            return false;
                        }
                    }

                    if (AppData.FilterValue.DateEnd != null)
                    {
                        if (item.ScoreSaber.TimeSet > AppData.FilterValue.DateEnd)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        private bool FilterByResultPlayStatus(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                if (item.BeatLeader.ModifiedScore < 0) // スコアなし＝プレイしていない
                {
                    return AppData.FilterValue.IsShowNotPlay;
                }
                else if (IsFailureByConfig(item.BeatLeader.Modifiers)) // モディファイに失敗相当の文字列あり＝フェイルしている
                {
                    return AppData.FilterValue.IsShowFailure;
                }
                else // 上記以外＝クリアしている
                {
                    return AppData.FilterValue.IsShowClear;
                }
            }
            else
            {
                if (item.ScoreSaber.ModifiedScore < 0) // スコアなし＝プレイしていない
                {
                    return AppData.FilterValue.IsShowNotPlay;
                }
                else if (IsFailureByConfig(item.ScoreSaber.Modifiers)) // モディファイに失敗相当の文字列あり＝フェイルしている
                {
                    return AppData.FilterValue.IsShowFailure;
                }
                else // 上記以外＝クリアしている
                {
                    return AppData.FilterValue.IsShowClear;
                }
            }
        }

        private bool FilterByResultAcc(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                return (AppData.FilterValue.MinAcc <= item.BeatLeader.Acc && item.BeatLeader.Acc < AppData.FilterValue.MaxAcc);
            }
            else
            {
                return (AppData.FilterValue.MinAcc <= item.ScoreSaber.Acc && item.ScoreSaber.Acc < AppData.FilterValue.MaxAcc);
            }
        }

        private bool FilterByResultWorldRank(IntegrationScore item)
        {
            if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
            {
                return (AppData.FilterValue.MinScoreRank <= item.BeatLeader.WorldRank && item.BeatLeader.WorldRank < AppData.FilterValue.MaxScoreRank);
            }
            else
            {
                return (AppData.FilterValue.MinScoreRank <= item.ScoreSaber.WorldRank && item.ScoreSaber.WorldRank < AppData.FilterValue.MaxScoreRank);
            }
        }

        private bool FilterByCheckbox(IntegrationScore item)
        {
            if (AppData.FilterValue.IsShowCheckedOnly)
            {
                return item.IsShowChekdOnlySelected;
            }

            return true;
        }

        private void DataGridFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is IntegrationScore item)
            {
                e.Accepted =
                    FilterBySearch(item) &&
                    FilterByMapStatus(item) &&
                    FilterByMapGameMode(item) &&
                    FilterByMapGameDifficulty(item) &&
                    FilterByMapStar(item) &&
                    FilterByMapRankedDate(item) &&
                    FilterByResultFullCombo(item) &&
                    FilterByResultScoreUpdateDate(item) &&
                    FilterByResultPlayStatus(item) &&
                    FilterByResultAcc(item) &&
                    FilterByResultPp(item) &&
                    FilterByResultWorldRank(item) &&
                    FilterByCheckbox(item);
            }
        }

        private void UpdateDataGridColumnVisibility()
        {
            foreach (var column in XaDataGrid.Columns)
            {
                var tagname = TagBehavior.GetTag(column).ToString();
                column.Visibility = (tagname != null && Config.GridSetting.notDisplayColumns.Contains(tagname)) ? Visibility.Hidden : Visibility.Visible;
                if ((tagname?.Contains("BeatLeader.") ?? false) && !AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
                {
                    column.Visibility = Visibility.Hidden;
                }
                if ((tagname?.Contains("ScoreSaber.") ?? false) && !AppData.ViewTarget.HasFlag(ViewTarget.ScoreSaber))
                {
                    column.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// 起動時、タブ切り替えのときに呼び出される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            XaButtonGetData.IsEnabled = false;

            UpdateDataGridColumnVisibility();
            XaDataGridColumnCover.Width = Config.GridSetting.rowHeight;
            XaDataGrid.RowHeight = Config.GridSetting.rowHeight;

            if (AppData.IsFirst)
            {
                // 起動直後の場合
                AppData.IsFirst = false;
                if (Config.ViewTarget.HasFlag(ViewTarget.BeatLeader))
                {
                    _viewModel.SetPlayerProfile(AppData.SelectedUser.BeatLeader.Profile);
                    _viewModel.ViewTargetHasBeatLeader = true;
                }
                else
                {
                    _viewModel.SetPlayerProfile(AppData.SelectedUser.ScoreSaber.Profile);
                    _viewModel.ViewTargetHasScoreSaber = true;
                }
                _viewModel.GridItemsViewSource.Source = AppData.SelectedUser.ScoresOfPlayedAndAllRanked;
                XaDataGrid.ItemsSource = _viewModel.GridItemsViewSource.View;
                XaDataGrid.SelectedIndex = -1;
            }
            else if (!_viewModel.ProfileId.Equals(Config.ScoreSaberProfileId))
            {
                // UserSelecterページで、もともと表示していたユーザ以外を選択した場合
                await Task.Run(() =>
                {
                    AppData.SelectedUser.ProfileId = Config.ScoreSaberProfileId;
                    AppData.SelectedUser.BeatLeader = new(AppData.SelectedUser.ProfileId);
                    AppData.SelectedUser.ScoreSaber = new(AppData.SelectedUser.ProfileId);

                    // 選択したユーザのプロフィールを表示する
                    _viewModel.ProfileId = AppData.SelectedUser.ProfileId;
                    if (AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader))
                    {
                        _viewModel.SetPlayerProfile(AppData.SelectedUser.BeatLeader.Profile);
                    }
                    else
                    {
                        _viewModel.SetPlayerProfile(AppData.SelectedUser.ScoreSaber.Profile);
                    }

                    AppData.SelectedUser.BeatLeader.LoadAllFromLocalFile();
                    AppData.SelectedUser.ScoreSaber.LoadAllFromLocalFile();

                    // グリッドをリセット
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.GridItemsViewSource.Source = new ObservableCollection<IntegrationScore>();
                        XaDataGrid.ItemsSource = _viewModel.GridItemsViewSource.View;
                    });

                    // 取得済みデータを表示
                    AppData.SelectedUser.ConstractScoresOfPlayedAndAllRanked();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.GridItemsViewSource.Source = AppData.SelectedUser.ScoresOfPlayedAndAllRanked;
                        XaDataGrid.ItemsSource = _viewModel.GridItemsViewSource.View;
                        XaDataGrid.SelectedIndex = -1;
                    });
                });
            }
            else
            {
                // ページタブを切り替えただけの場合
                // UserSelecterページで、もともと表示していたユーザを選択した場合
            }

            RefreshGrid();

            XaButtonGetData.IsEnabled = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            XaButtonGetData.IsEnabled = false;
            Config.ScoreSaberProfileId = XaProfileId.Text;
            await DownaloadAndRefleshView(XaRadioGetModeAll.IsChecked ?? false);
            XaButtonGetData.IsEnabled = true;
        }

        private async Task DownaloadAndRefleshView(bool isGetAll)
        {
            _viewModel.Task1ProgressValue = 0;
            _viewModel.Task2ProgressValue = 0;
            _viewModel.Task3ProgressValue = 0;

            Application.Current.Dispatcher.Invoke(() =>
            {
                _viewModel.GridItemsViewSource.Source = new ObservableCollection<IntegrationScore>();
            });

            // 譜面リストを取得
            Task downloadRankedMaps = TaskDownloadMapList();

            // ScoreSaberのプロフィール取得
            Task fetchScoreSaberLatestProfile = AppData.SelectedUser.ScoreSaber.FetchLatestProfileAsync();

            // BeatLeaderのプロフィール取得
            Task fetchBeatLeaderLatestProfile = AppData.SelectedUser.BeatLeader.FetchLatestProfileAsync();

            await Task.WhenAll(fetchScoreSaberLatestProfile, fetchBeatLeaderLatestProfile);

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_viewModel.ViewTargetHasBeatLeader)
                {
                    _viewModel.SetPlayerProfile(AppData.SelectedUser.BeatLeader.Profile);
                }
                if (_viewModel.ViewTargetHasScoreSaber)
                {
                    _viewModel.SetPlayerProfile(AppData.SelectedUser.ScoreSaber.Profile);
                }
            });

            // ScoreSaber、BeatLeaderから最新スコアを取得
            Task downloadLatestScores = TaskDownloadLatestScores(isGetAll);

            await Task.WhenAll(downloadRankedMaps, downloadLatestScores);

            AppData.SelectedUser.ConstractScoresOfPlayedAndAllRanked();

            // GridItemを構築。未取得のカバー画像は後で取得する。
            Application.Current.Dispatcher.Invoke(() =>
            {
                _viewModel.GridItemsViewSource.Source = AppData.SelectedUser.ScoresOfPlayedAndAllRanked;
                XaDataGrid.ItemsSource = _viewModel.GridItemsViewSource.View;
                XaDataGrid.SelectedIndex = -1;
            });

            // 未取得のカバー画像を取得しながら逐次表示を更新する
            await TaskDownloadUnacquiredCover();

            RefreshGrid();
        }

        private async Task TaskDownloadLatestScores(bool isGetAll)
        {
            _viewModel.StatusText = "";
            var ssExecuter = AppData.SelectedUser.ScoreSaber.FetchLatestScores(isGetAll);
            var blExecuter = AppData.SelectedUser.BeatLeader.FetchLatestScores(isGetAll);
            int progressMax = ssExecuter.TotalStepCount + blExecuter.TotalStepCount + 2;
            _viewModel.Task2ProgressMax = progressMax;

            Task ss = Task.Run(() =>
            {
                while (ssExecuter.CurrentStatus == IStepExecuter.Status.Processing)
                {
                    _viewModel.Task2ProgressValue += 1;
                    _ = ssExecuter.ExecuteStep();
                }

                if (ssExecuter.CurrentStatus == IStepExecuter.Status.Completed)
                {
                    AppData.SelectedUser.ScoreSaber.SaveAllToLocalFile();
                }
                else if (ssExecuter.CurrentStatus == IStepExecuter.Status.Failed)
                {
                    _viewModel.StatusText += "[ScoreSaberのスコア取得に失敗]";
                }
                _viewModel.Task2ProgressValue += 1;
            });

            Task bl = Task.Run(() =>
            {
                while (blExecuter.CurrentStatus == IStepExecuter.Status.Processing)
                {
                    _viewModel.Task2ProgressValue += 1;
                    _ = blExecuter.ExecuteStep();
                }

                if (blExecuter.CurrentStatus == IStepExecuter.Status.Completed)
                {
                    AppData.SelectedUser.BeatLeader.SaveAllToLocalFile();
                }
                else if (blExecuter.CurrentStatus == IStepExecuter.Status.Failed)
                {
                    _viewModel.StatusText += "[BeatLeaderのスコア取得に失敗]";
                }
                _viewModel.Task2ProgressValue += 1;
            });

            await Task.WhenAll(ss, bl);

            _viewModel.Task2ProgressValue = progressMax;
        }

        private async Task TaskDownloadMapList()
        {
            await Task.Run(() =>
            {
                var executer = BeatMapDic.UpdateDirectory();
                int progressMax = executer.TotalStepCount + 1;
                _viewModel.Task1ProgressMax = progressMax;

                while (executer.CurrentStatus == IStepExecuter.Status.Processing)
                {
                    _viewModel.Task1ProgressValue += 1;
                    _ = executer.ExecuteStep();
                }

                _viewModel.Task1ProgressValue = progressMax;
            });
        }

        private async Task TaskDownloadUnacquiredCover()
        {
            int count = 0;
            Dictionary<string, List<IntegrationScore>> needAcquireCovers = new();

            foreach (var item in AppData.SelectedUser.ScoresOfPlayedAndAllRanked)
            {
                if (!BeatMapCover.IsExistCoverAtLocal(item.Map.Hash))
                {
                    if (!needAcquireCovers.ContainsKey(item.Map.Hash))
                    {
                        needAcquireCovers[item.Map.Hash] = new();
                    }
                    needAcquireCovers[item.Map.Hash].Add(item);
                }
            }

            _viewModel.Task3ProgressMax = needAcquireCovers.Count;

            int next = 100;

            // カバー画像を並列で取得
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10
            };
            await Parallel.ForEachAsync(needAcquireCovers, parallelOptions, async (cover, y) =>
            {
                _ = await BeatMapCover.GetCover(cover.Key, cover.Value.First().CoverUrl);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    cover.Value.ForEach(item =>
                    {
                        item.Cover = BeatMapCover.GetCoverLocalPath(item.Map.Hash);
                    });
                }));

                {
                    count++;
                    _viewModel.Task3ProgressValue = count;
                    if (count >= next || count == needAcquireCovers.Count)
                    {
                        next += 100;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            RefreshGrid();
                        });
                    }
                }
            });
            _viewModel.Task3ProgressValue = needAcquireCovers.Count;
        }

        private void OnFilterEnableChanged(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void OnFilterSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshGrid();
        }

        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (((PlaceholderTextBox)sender)._imeFlag) return;
            RefreshGrid();
        }

        private void OnClickCopyBSR(object sender, RoutedEventArgs e)
        {
            try
            {
                IntegrationScore? obj = ((FrameworkElement)sender).DataContext as IntegrationScore;
                Clipboard.SetData(DataFormats.Text, $"!bsr {obj?.Map.Key}");
            }
            catch (Exception ex)
            {
                // クリップボード監視系のアプリを使っているとエラーが出やすい？
                // クリップボードにコピーできていてもエラーが出ることがあるので握りつぶす
                // コピーでてきていなくてもユーザーはクリック失敗したかなと思う程度なのて問題ない
                _logger.Debug(ex.ToString());
            }
        }

        private System.Diagnostics.Process? OpenUrl(string url)
        {
            System.Diagnostics.ProcessStartInfo pi = new()
            {
                FileName = url,
                UseShellExecute = true,
            };
            return System.Diagnostics.Process.Start(pi);
        }

        private void OnClickJumpBeatSaver(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((FrameworkElement)sender).DataContext is IntegrationScore item)
                {
                    if (item.Map.Key.Length > 0)
                    {
                        var url = $"https://beatsaver.com/maps/{item.Map.Key}";
                        _ = OpenUrl(url);
                    }
                    else
                    {
                        // リパブされた譜面のKeyはScrappedDataに含まれていないのでHashで検索する。
                        // 古いHashで検索するとリパブ後のページがヒットする。
                        var url = $"https://beatsaver.com/?q={item.Map.Hash.ToLower()}";
                        _ = OpenUrl(url);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Warn(ex);
            }
        }

        private async void OnClickJumpScoreSaber(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((FrameworkElement)sender).DataContext is IntegrationScore item)
                {
                    if (item.ScoreSaber.Reference?.leaderboard.id > 0)
                    {
                        var url = $"https://scoresaber.com/leaderboard/{item.ScoreSaber.Reference?.leaderboard.id}";
                        _ = OpenUrl(url);
                    }
                    else
                    {
                        // 未プレイランク譜面のLeaderbordIdはScrappedDataに含まれていないので取得してくる必要がある
                        var info = await ScoreSaber.GetLeaderboard(item.Map.Hash, item.Map.MapDifficulty, item.Map.MapMode);
                        var url = $"https://scoresaber.com/leaderboard/{info.difficulty.leaderboardId}";
                        _ = OpenUrl(url);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Warn(ex);
            }
        }

        private async void OnClickJumpBeatLeader(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((FrameworkElement)sender).DataContext is IntegrationScore item)
                {
                    if (item.BeatLeader.Reference?.leaderboardId.Length > 0)
                    {
                        var url = $"https://www.beatleader.xyz/leaderboard/global/{item.BeatLeader.Reference?.leaderboardId}";
                        _ = OpenUrl(url);
                    }
                    else
                    {
                        var response = await BeatLeader.GetLeaderboardsByHash(item.Map.Hash);
                        var leaderboardId = response.leaderboards.Find(l => l.difficulty.mapDifficulty == item.Map.MapDifficulty && l.difficulty.mapMode == item.Map.MapMode)?.id;
                        var url = $"https://www.beatleader.xyz/leaderboard/global/{leaderboardId}";
                        _ = OpenUrl(url);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Warn(ex);
            }
        }

        private void OnClickGridItemCheckBox(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is IntegrationScore item)
            {
                item.IsShowChekdOnlySelected = !item.IsShowChekdOnlySelected;
                RefreshGrid();
            }
        }

        private void OnClickCheckFiltered(object sender, RoutedEventArgs e)
        {
            XaButtonCheckFiltered.IsEnabled = XaButtonClearFiltered.IsEnabled = false;
            foreach (var item in XaDataGrid.Items)
            {
                if (item is IntegrationScore gridItem)
                {
                    gridItem.IsShowChekdOnlySelected = true;
                }
            }
            RefreshGrid();
            XaButtonCheckFiltered.IsEnabled = XaButtonClearFiltered.IsEnabled = true;
        }

        private void OnClickClearFiltered(object sender, RoutedEventArgs e)
        {
            XaButtonCheckFiltered.IsEnabled = XaButtonClearFiltered.IsEnabled = false;
            foreach (var item in XaDataGrid.Items)
            {
                if (item is IntegrationScore gridItem)
                {
                    gridItem.IsShowChekdOnlySelected = false;
                }
            }
            RefreshGrid();
            XaButtonCheckFiltered.IsEnabled = XaButtonClearFiltered.IsEnabled = true;
        }

        private void OnClickCreatePlaylist(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSONファイル(*.json)|*.json|プレイリストファイル(*.bplist)|*.bplist|全てのファイル(*.*)|*.*"
            };

            var result = dialog.ShowDialog() ?? false;

            // 保存ボタン以外が押下された場合
            if (!result)
            {
                return;
            }

            _logger.Info($"Create {dialog.FileName}, count={XaDataGrid.Items.Count}");
            var playlist = new PlayList
            {
                Title = System.IO.Path.GetFileName(dialog.FileName)
            };
            foreach (var item in XaDataGrid.Items)
            {
                if (item is IntegrationScore i)
                {
                    playlist.AddSong(i.Map.Key, i.Map.Hash, i.Map.SongName, i.Map.MapperName, i.Map.MapMode, i.Map.MapDifficulty);
                }
            }
            playlist.Save(dialog.FileName);
        }

        private void OnClickCreateCSV(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSVファイル(*.csv)|*.csv|全てのファイル(*.*)|*.*"
            };

            var result = dialog.ShowDialog() ?? false;

            // 保存ボタン以外が押下された場合
            if (!result)
            {
                return;
            }

            String delmiter = ",";
            StringBuilder sb = new();
            sb.Append("bsr").Append(delmiter);
            sb.Append("曲名").Append(delmiter);
            sb.Append("サブ曲名").Append(delmiter);
            sb.Append("曲作者").Append(delmiter);
            sb.Append("譜面作者").Append(delmiter);
            sb.Append("更新日").Append(delmiter);
            sb.Append("モード").Append(delmiter);
            sb.Append("難易度").Append(delmiter);
            sb.Append("✖").Append(delmiter);
            sb.Append("FC").Append(delmiter);
            sb.Append("スコア").Append(delmiter);
            sb.Append("精度").Append(delmiter);
            sb.Append("ミス").Append(delmiter);
            sb.Append("PP").Append(delmiter);
            sb.Append("Modifiers").Append(delmiter);
            sb.Append("Miss").Append(delmiter);
            sb.Append("Bad");
            sb.Append(Environment.NewLine);

            foreach (var item in XaDataGrid.Items)
            {
                if (item is IntegrationScore i)
                {
                    sb.Append(i.Map.Key).Append(delmiter);
                    sb.Append($"\"{TrimDoubleQuotationMarks(i.Map.SongName)}\"").Append(delmiter);
                    sb.Append($"\"{TrimDoubleQuotationMarks(i.Map.SongSubName)}\"").Append(delmiter);
                    sb.Append($"\"{TrimDoubleQuotationMarks(i.Map.SongAuthorName)}\"").Append(delmiter);
                    sb.Append($"\"{TrimDoubleQuotationMarks(i.Map.MapperName)}\"").Append(delmiter);
                    if (i.ScoreSaber.TimeSet != null)
                    {
                        sb.Append(i.ScoreSaber.TimeSet?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss (ddd)")).Append(delmiter);
                    }
                    else
                    {
                        sb.Append("").Append(delmiter);
                    }
                    sb.Append(i.Map.MapMode).Append(delmiter);
                    sb.Append(i.Map.MapDifficulty).Append(delmiter);
                    sb.Append(i.Map.ScoreSaber.Star).Append(delmiter);
                    sb.Append(i.ScoreSaber.ModifiedScore).Append(delmiter);
                    sb.Append(i.ScoreSaber.Acc).Append(delmiter);
                    sb.Append(i.ScoreSaber.MissPlusBad).Append(delmiter);
                    sb.Append(i.ScoreSaber.FullCombo).Append(delmiter);
                    sb.Append(i.ScoreSaber.PP).Append(delmiter);
                    sb.Append($"\"{i.ScoreSaber.Modifiers}\"").Append(delmiter);
                    sb.Append(i.ScoreSaber.Miss).Append(delmiter);
                    sb.Append(i.ScoreSaber.Bad);
                    sb.Append(Environment.NewLine);
                }
            }

            try
            {
                using var st = dialog.OpenFile();
                using var sw = new System.IO.StreamWriter(st, Encoding.GetEncoding("UTF-8"));
                sw.Write(sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.ToString());
            }
        }

        private static string TrimDoubleQuotationMarks(string target)
        {
            return target.Trim(new char[] { '"' });
        }

        private void XaDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // データグリッドでマウスがクリックされた位置を取得
            var point = e.GetPosition(XaDataGrid);

            // データグリッドでマウスがクリックされた位置の行オブジェクトを取得
            var row = GetDataGridObject<DataGridRow>(XaDataGrid, point);
            if (row == null)
            {
                return;
            }

            // データグリッドでマウスがクリックされた位置のセルオブジェクトを取得
            var cell = GetDataGridObject<DataGridCell>(XaDataGrid, point);
            if (cell == null)
            {
                return;
            }

            // コピーするデータを取得
            var item = (IntegrationScore)row.Item;
            var tag = TagBehavior.GetTag(cell.Column).ToString();
            var str = tag switch
            {
                Config.ColumnTagMapBsr => item.Map.Key,
                Config.ColumnTagMapSongName => item.SongFullName,
                Config.ColumnTagMapDuration => item.Map.Duration.ToString(),
                Config.ColumnTagMapBpm => item.Map.Bpm.ToString(),
                Config.ColumnTagMapNotes => item.Map.Notes.ToString(),
                Config.ColumnTagMapNps => item.Map.Nps.ToString(),
                Config.ColumnTagMapNjs => item.Map.Njs.ToString(),
                Config.ColumnTagMapBombs => item.Map.Bombs.ToString(),
                Config.ColumnTagMapWalls => item.Map.Walls.ToString(),
                Config.ColumnTagMapHash => item.Map.Hash,
                Config.ColumnTagMapScoreSaberRankedDate => item.Map.ScoreSaber.RankedTime?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss (ddd)"),
                Config.ColumnTagMapScoreSaberStar => item.Map.ScoreSaber.Star.ToString(),
                Config.ColumnTagScoreSaberDate => item.ScoreSaber.TimeSet?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss (ddd)"),
                Config.ColumnTagScoreSaberScore => item.ScoreSaber.ModifiedScore.ToString(),
                Config.ColumnTagScoreSaberAcc => item.ScoreSaber.Acc.ToString(),
                Config.ColumnTagScoreSaberAccDiff => item.ScoreSaber.AccDifference.ToString(),
                Config.ColumnTagScoreSaberMissPlusBad => item.ScoreSaber.MissPlusBad.ToString(),
                Config.ColumnTagScoreSaberFullCombo => item.ScoreSaber.FullCombo,
                Config.ColumnTagScoreSaberPp => item.ScoreSaber.PP.ToString(),
                Config.ColumnTagScoreSaberModifiers => item.ScoreSaber.Modifiers,
                Config.ColumnTagScoreSaberMiss => item.ScoreSaber.Miss.ToString(),
                Config.ColumnTagScoreSaberBad => item.ScoreSaber.Bad.ToString(),
                Config.ColumnTagScoreSaberWorldRank => item.ScoreSaber.WorldRank.ToString(),
                Config.ColumnTagMapBeatLeaderRankedDate => item.Map.BeatLeader.RankedTime?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss (ddd)"),
                Config.ColumnTagMapBeatLeaderStar => item.Map.BeatLeader.Star.ToString(),
                Config.ColumnTagBeatLeaderDate => item.BeatLeader.TimeSet?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss (ddd)"),
                Config.ColumnTagBeatLeaderScore => item.BeatLeader.ModifiedScore.ToString(),
                Config.ColumnTagBeatLeaderAcc => item.BeatLeader.Acc.ToString(),
                Config.ColumnTagBeatLeaderAccDiff => item.BeatLeader.AccDifference.ToString(),
                Config.ColumnTagBeatLeaderMissPlusBad => item.BeatLeader.MissPlusBad.ToString(),
                Config.ColumnTagBeatLeaderFullCombo => item.BeatLeader.FullCombo,
                Config.ColumnTagBeatLeaderPp => item.BeatLeader.PP.ToString(),
                Config.ColumnTagBeatLeaderModifiers => item.BeatLeader.Modifiers,
                Config.ColumnTagBeatLeaderMiss => item.BeatLeader.Miss.ToString(),
                Config.ColumnTagBeatLeaderBad => item.BeatLeader.Bad.ToString(),
                Config.ColumnTagBeatLeaderWorldRank => item.BeatLeader.WorldRank.ToString(),
                _ => "notsupport",
            };

            if (str == null || str.Equals("notsupport"))
            {
                XaDataGrid.ContextMenu = null;
                return;
            }

            // ContextMenuを作成する。
            MenuItemEx menuitem = new MenuItemEx();
            menuitem.Value = str;
            menuitem.Header = "コピー";
            menuitem.Click += menuitem_Click; // メニューのClickにイベントハンドラを追加する。
            ContextMenu contextmenu = new ContextMenu();
            contextmenu.Items.Add(menuitem); // MenuItemインスタンスを、ContextMenuインスタンスに追加する。
            XaDataGrid.ContextMenu = contextmenu; // ContextMenuインスタンスを、DataGridのContextMenuに設定する。
        }

        private void menuitem_Click(object sender, RoutedEventArgs e)
        {
            MenuItemEx menuitem = (MenuItemEx)sender; // 呼び出し元のMenuItemを取得する。
            try
            {
                Clipboard.SetData(DataFormats.Text, menuitem.Value);
                //MessageBox.Show(menuitem.Value);
            }
            catch (Exception ex)
            {
                // クリップボード監視系のアプリを使っているとエラーが出やすい？
                // クリップボードにコピーできていてもエラーが出ることがあるので握りつぶす
                // コピーでてきていなくてもユーザーはクリック失敗したかなと思う程度なのて問題ない
                _logger.Debug(ex.ToString());
            }
        }

        /**
         * @brief 引数の位置のDataGridのオブジェクトを取得します。
         * 
         * @param [in] dataGrid データグリッド
         * @param [in] point 位置
         * @return DataGridのオブジェクト
         */
        private T? GetDataGridObject<T>(DataGrid dataGrid, Point point)
        {
            T? result = default;
            var hitResultTest = VisualTreeHelper.HitTest(dataGrid, point);
            if (hitResultTest != null)
            {
                var visualHit = hitResultTest.VisualHit;
                while (visualHit != null)
                {
                    if (visualHit is T)
                    {
                        result = (T)(object)visualHit;
                        break;
                    }
                    visualHit = VisualTreeHelper.GetParent(visualHit);
                }
            }
            return result;
        }

        private void XaViewTargetScoreSaberImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ViewTargetHasBeatLeader = false;
            _viewModel.ViewTargetHasScoreSaber = true;

            _viewModel.SetPlayerProfile(AppData.SelectedUser.ScoreSaber.Profile);
            UpdateDataGridColumnVisibility();
            RefreshGrid();
        }

        private void XaViewTargetBeatLeaderImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ViewTargetHasScoreSaber = false;
            _viewModel.ViewTargetHasBeatLeader = true;

            _viewModel.SetPlayerProfile(AppData.SelectedUser.BeatLeader.Profile);
            UpdateDataGridColumnVisibility();
            RefreshGrid();
        }
    }

    internal class MenuItemEx : MenuItem
    {
        public string Value = String.Empty;
    }

    public class PageMainViewModel : ObservableBase
    {
        private string _ProfileId = "";
        private string _Name = "";
        private string _ProfilePicture = "Resources/_404.png";
        private string _Country = "JP";
        private double _Pp;
        private long _GlobalRank;
        private long _CountryRank;
        private long _TotalScore;
        private long _TotalRankedScore;
        private double _AverageRankedAccuracy;
        private long _TotalPlayCount;
        private long _RankedPlayCount;
        private long _ReplaysWatched;
        private string _StatusText = "";
        private bool _IsReadOnly = true;
        private int _Task1ProgressMax = 1;
        private int _Task1ProgressValue = 0;
        private int _Task2ProgressMax = 1;
        private int _Task2ProgressValue = 0;
        private int _Task3ProgressMax = 1;
        private int _Task3ProgressValue = 0;

        internal readonly CollectionViewSource GridItemsViewSource;

        public PageMainViewModel()
        {
            GridItemsViewSource = new() { Source = new ObservableCollection<IntegrationScore>() };
        }

        public ICollectionView GridData { get => GridItemsViewSource.View; }

        public FilterValue FilterValue { get => AppData.FilterValue; }

        public bool IsReadOnly { get => _IsReadOnly; set => SetProperty(ref _IsReadOnly, value); }

        /// <summary>
        /// プレイヤープロフィール：ID
        /// </summary>
        public string ProfileId { get => _ProfileId; set => SetProperty(ref _ProfileId, value); }

        /// <summary>
        /// プレイヤープロフィール：プレイヤー名
        /// </summary>
        public string Name { get => _Name; set => SetProperty(ref _Name, value); }

        /// <summary>
        /// プレイヤープロフィール：画像URL
        /// </summary>
        public string ProfilePicture { get => _ProfilePicture; set => SetProperty(ref _ProfilePicture, value); }

        /// <summary>
        /// プレイヤープロフィール：所属国
        /// </summary>
        public string Country { get => _Country; set => SetProperty(ref _Country, value); }

        /// <summary>
        /// プレイヤープロフィール：PP
        /// </summary>
        public double Pp { get => _Pp; set => SetProperty(ref _Pp, value); }

        /// <summary>
        /// プレイヤープロフィール：世界順位
        /// </summary>
        public long GlobalRank { get => _GlobalRank; set => SetProperty(ref _GlobalRank, value); }

        /// <summary>
        /// プレイヤープロフィール：国内順位
        /// </summary>
        public long CountryRank { get => _CountryRank; set => SetProperty(ref _CountryRank, value); }

        /// <summary>
        /// プレイヤープロフィール：全譜面合計スコア
        /// </summary>
        public long TotalScore { get => _TotalScore; set => SetProperty(ref _TotalScore, value); }

        /// <summary>
        /// プレイヤープロフィール：ランク譜面合計スコア
        /// </summary>
        public long TotalRankedScore { get => _TotalRankedScore; set => SetProperty(ref _TotalRankedScore, value); }

        /// <summary>
        /// プレイヤープロフィール：ランク譜面平均精度
        /// </summary>
        public double AverageRankedAccuracy { get => _AverageRankedAccuracy; set => SetProperty(ref _AverageRankedAccuracy, value); }

        /// <summary>
        /// プレイヤープロフィール：スコアを送信した難易度の数
        /// </summary>
        public long TotalPlayCount { get => _TotalPlayCount; set => SetProperty(ref _TotalPlayCount, value); }

        /// <summary>
        /// プレイヤープロフィール：ランク譜面のスコアを送信した難易度の数
        /// </summary>
        public long RankedPlayCount { get => _RankedPlayCount; set => SetProperty(ref _RankedPlayCount, value); }

        /// <summary>
        /// プレイヤープロフィール：他の人に再生されたリプレイの数
        /// </summary>
        public long ReplaysWatched { get => _ReplaysWatched; set => SetProperty(ref _ReplaysWatched, value); }

        /// <summary>
        /// タスク１の進捗
        /// </summary>
        public int Task1ProgressMax { get => _Task1ProgressMax; set => SetProperty(ref _Task1ProgressMax, value); }

        /// <summary>
        /// タスク１の進捗
        /// </summary>
        public int Task1ProgressValue { get => _Task1ProgressValue; set => SetProperty(ref _Task1ProgressValue, value); }

        /// <summary>
        /// タスク２の進捗
        /// </summary>
        public int Task2ProgressMax { get => _Task2ProgressMax; set => SetProperty(ref _Task2ProgressMax, value); }

        /// <summary>
        /// タスク２の進捗
        /// </summary>
        public int Task2ProgressValue { get => _Task2ProgressValue; set => SetProperty(ref _Task2ProgressValue, value); }

        /// <summary>
        /// タスク３の進捗
        /// </summary>
        public int Task3ProgressMax { get => _Task3ProgressMax; set => SetProperty(ref _Task3ProgressMax, value); }

        /// <summary>
        /// タスク３の進捗
        /// </summary>
        public int Task3ProgressValue { get => _Task3ProgressValue; set => SetProperty(ref _Task3ProgressValue, value); }

        /// <summary>
        /// 画面したのステータスバーに表示するテキスト
        /// </summary>
        public string StatusText { get => _StatusText; set => SetProperty(ref _StatusText, value); }

        /// <summary>
        /// 現在の表示ターゲットにScoreSaberが含まれているかどうか
        /// </summary>
        public bool ViewTargetHasScoreSaber
        {
            get => AppData.ViewTarget.HasFlag(ViewTarget.ScoreSaber);
            set
            {
                if (value)
                {
                    AppData.ViewTarget |= ViewTarget.ScoreSaber;
                }
                else
                {
                    AppData.ViewTarget &= ~ViewTarget.ScoreSaber;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 現在の表示ターゲットにBeatLeaderが含まれているかどうか
        /// </summary>
        public bool ViewTargetHasBeatLeader
        {
            get => AppData.ViewTarget.HasFlag(ViewTarget.BeatLeader);
            set
            {
                if (value)
                {
                    AppData.ViewTarget |= ViewTarget.BeatLeader;
                }
                else
                {
                    AppData.ViewTarget &= ~ViewTarget.BeatLeader;
                }
                OnPropertyChanged();
            }
        }

        public void SetPlayerProfile(ScoreSaber.PlayerProfile profile)
        {
            ProfileId = profile.id;
            Name = profile.name;
            ProfilePicture = (profile.profilePicture.Length == 0) ? "Resources/_404.png" : profile.profilePicture;
            Country = profile.country;
            Pp = profile.pp;
            GlobalRank = profile.rank;
            CountryRank = profile.countryRank;
            TotalScore = profile.scoreStats.totalScore;
            TotalRankedScore = profile.scoreStats.totalRankedScore;
            AverageRankedAccuracy = profile.scoreStats.averageRankedAccuracy;
            TotalPlayCount = profile.scoreStats.totalPlayCount;
            RankedPlayCount = profile.scoreStats.rankedPlayCount;
            ReplaysWatched = profile.scoreStats.replaysWatched;
        }

        public void SetPlayerProfile(BeatLeader.PlayerResponseFull profile)
        {
            ProfileId = profile.id;
            Name = profile.name;
            ProfilePicture = (profile.avatar.Length == 0) ? "Resources/_404.png" : profile.avatar;
            Country = profile.country;
            Pp = profile.pp;
            GlobalRank = profile.rank;
            CountryRank = profile.countryRank;
            TotalScore = profile.scoreStats.totalScore;
            TotalRankedScore = profile.scoreStats.totalRankedScore;
            AverageRankedAccuracy = profile.scoreStats.averageRankedAccuracy * 100;
            TotalPlayCount = profile.scoreStats.totalPlayCount;
            RankedPlayCount = profile.scoreStats.rankedPlayCount;
            ReplaysWatched = profile.scoreStats.watchedReplays;
        }
    }

    public class NumOfKey : IComparable
    {
        public long Key;

        public bool IsDeleted;

        public int CompareTo(object? obj)
        {
            return Key.CompareTo(((NumOfKey?)obj)?.Key);
        }
    }
}
