using Microsoft.Win32;
using MyBeatSaberScore.APIs;
using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Model;
using MyBeatSaberScore.Utility;
using Newtonsoft.Json.Linq;
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

        private Dictionary<string, DataGridColumn> _dataGridColumnsDic;

        private int _preDisplayIndex;

        public static PageMain? Instance { get; private set; }

        public PageMain()
        {
            InitializeComponent();
            _viewModel = (PageMainViewModel)DataContext;
            _viewModel.GridItemsViewSource.Filter += new FilterEventHandler(DataGridFilter);

            _dataGridColumnsDic = new();
            foreach (var column in XaDataGrid.Columns)
            {
                var tagname1 = TagBehavior.GetTag(column).ToString();
                if (tagname1 != null)
                {
                    _dataGridColumnsDic.Add(tagname1, column);
                }
            }

            Instance = this;
        }

        internal List<Config.GridColumnParam>? GetGridColumnParams()
        {
            if (XaDataGrid != null)
            {
                var list = new List<Config.GridColumnParam>();
                foreach (var column in XaDataGrid.Columns)
                {
                    list.Add(new Config.GridColumnParam(column));
                }
                return list;
            }
            else
            {
                return null;
            }
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

        private void DataGridFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is IntegrationScore item)
            {
                e.Accepted =
                    AppData.MainPageFilter.Value.MapFullName.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapBsr.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapHash.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapRankStatus.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapMode.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapDifficulty.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapStar.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapDuration.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapBpm.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapNotes.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapBombs.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapWalls.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapNps.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapNjs.IsShow(item) &&
                    AppData.MainPageFilter.Value.MapRankedDate.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayUpdateDate.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayResult.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayFullCombo.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayPp.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayAcc.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayWorldRank.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayMissPlusBad.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayMiss.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayBad.IsShow(item) &&
                    AppData.MainPageFilter.Value.PlayModifiers.IsShow(item) &&
                    AppData.MainPageFilter.Value.EtcCheckedOnly.IsShow(item);
            }
        }

        private void UpdateDataGridColumnVisibility()
        {
            foreach (var column in XaDataGrid.Columns)
            {
                var tagname = TagBehavior.GetTag(column).ToString();
                column.Visibility = (tagname != null && Config.Grid.notDisplayColumns.Contains(tagname)) ? Visibility.Hidden : Visibility.Visible;
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

        private void RestoreColumnParam(List<Config.GridColumnParam> columnParams, bool restoreOrder, bool restoreWidth)
        {
            debugPrintColumns("0-1: ");
            var orderdList = columnParams.OrderBy(c => c.displayIndex);
            foreach (var param in orderdList)
            {
                try
                {
                    _dataGridColumnsDic[param.name].DisplayIndex = param.displayIndex;
                    _dataGridColumnsDic[param.name].Width = param.width.Equals("Auto") ? DataGridLength.Auto : double.Parse(param.width);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex);
                }
                debugPrintColumns("0-2: ");
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

            // 設定にあわせて列の表示/非表示、行幅を変更
            UpdateDataGridColumnVisibility();
            XaDataGridColumnCover.Width = Config.Grid.rowHeight;
            XaDataGrid.RowHeight = Config.Grid.rowHeight;

            if (AppData.IsFirst)
            {
                // 起動直後の場合
                AppData.IsFirst = false;

                // 設定にあわせて列の順番・幅を変更
                if (Config.Grid.columnRestore.mode == Config.RestoreMode.Last)
                {
                    RestoreColumnParam(Config.Grid.columnRestore.lastParams, restoreOrder: true, restoreWidth: true);
                }
                else if (Config.Grid.columnRestore.mode == Config.RestoreMode.Saved)
                {
                    RestoreColumnParam(Config.Grid.columnRestore.savedParams, restoreOrder: true, restoreWidth: true);
                }
                else
                {
                    // do nothing, use default
                }

                // 設定にあわせてプロフィールを変更
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

                // 列幅変更のイベントハンドラ登録
                var evsetter = new EventSetter();
                evsetter.Event = FrameworkElement.SizeChangedEvent;
                evsetter.Handler = new SizeChangedEventHandler(XaDataGrid_ColumnSizeChanged);

                var headerStyle = new Style();
                headerStyle.TargetType = new System.Windows.Controls.Primitives.DataGridColumnHeader().GetType();
                headerStyle.Setters.Add(evsetter);

                XaDataGrid.ColumnHeaderStyle = headerStyle;

                // 行に表示するデータを設定
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
            await DownaloadAndRefleshView(XaRadioGetModeAll.IsChecked ?? false);
            XaButtonGetData.IsEnabled = true;
        }

        private async Task DownaloadAndRefleshView(bool isGetAll)
        {
            static string statusText(bool ssProfile, bool ssScores, bool bsProfile, bool bsScores)
            {
                static string okng(bool a) { return a ? "OK" : "NG"; }
                return $"ScoreSaber: {{ Profile: {okng(ssProfile)}, Scores: {okng(ssScores)} }}, BeatLeader: {{ Profile: {okng(bsProfile)}, Scores: {okng(bsScores)} }}";
            }

            _viewModel.StatusText = "";
            _viewModel.Task1ProgressValue = 0;
            _viewModel.Task2ProgressValue = 0;
            _viewModel.Task3ProgressValue = 0;

            var scoreSaberUserData = AppData.SelectedUser.ScoreSaber;
            var beatLeaderUserData = AppData.SelectedUser.BeatLeader;

            // ProfileID入力欄を手動で変更している場合
            if (!AppData.SelectedUser.ProfileId.Equals(XaProfileId.Text))
            {
                // 表示切替用に新しいProfileIDのUserDataを生成、有効なProfileIDだとわかるまで一時データとして扱う
                scoreSaberUserData = new ScoreSaberUserData(XaProfileId.Text);
                beatLeaderUserData = new BeatLeaderUserData(XaProfileId.Text);

                // プロフィール、スコアの表示をクリア
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _viewModel.SetPlayerProfile(new APIs.ScoreSaber.PlayerProfile() { id = XaProfileId.Text });
                    _viewModel.GridItemsViewSource.Source = new ObservableCollection<IntegrationScore>();
                    XaDataGrid.ItemsSource = _viewModel.GridItemsViewSource.View;
                });
            }

            // 譜面リストを取得
            Task downloadRankedMaps = TaskDownloadMapList();

            // ScoreSaberのプロフィール取得
            Task<bool> fetchScoreSaberLatestProfile = scoreSaberUserData.FetchLatestProfileAsync();

            // BeatLeaderのプロフィール取得
            Task<bool> fetchBeatLeaderLatestProfile = beatLeaderUserData.FetchLatestProfileAsync();

            await Task.WhenAll(fetchScoreSaberLatestProfile, fetchBeatLeaderLatestProfile);

            if (scoreSaberUserData.IsExistProfile || beatLeaderUserData.IsExistProfile)
            {
                Config.ScoreSaberProfileId = XaProfileId.Text;
                AppData.SelectedUser.ProfileId = XaProfileId.Text;
                AppData.SelectedUser.BeatLeader = beatLeaderUserData;
                AppData.SelectedUser.ScoreSaber = scoreSaberUserData;

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
                Task<(bool ssResult, bool blResult)> downloadLatestScores = TaskDownloadLatestScores(isGetAll);

                await Task.WhenAll(downloadRankedMaps, downloadLatestScores);

                _viewModel.StatusText = statusText(fetchScoreSaberLatestProfile.Result, downloadLatestScores.Result.ssResult, fetchBeatLeaderLatestProfile.Result, downloadLatestScores.Result.blResult);

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
            else
            {
                await downloadRankedMaps;
                _viewModel.Task1ProgressValue = 0;
                _viewModel.Task2ProgressValue = 0;
                _viewModel.Task3ProgressValue = 0;
                _viewModel.StatusText = statusText(fetchScoreSaberLatestProfile.Result, false, fetchBeatLeaderLatestProfile.Result, false);
                MessageBox.Show($"プロフィールデータを取得できません。IDが間違っているもしくは通信に失敗した可能性があります。");
            }
        }

        private async Task<(bool ssResult, bool blResult)> TaskDownloadLatestScores(bool isGetAll)
        {
            var ssExecuter = AppData.SelectedUser.ScoreSaber.FetchLatestScores(isGetAll);
            var blExecuter = AppData.SelectedUser.BeatLeader.FetchLatestScores(isGetAll);
            int progressMax = ssExecuter.TotalStepCount + blExecuter.TotalStepCount + 2;
            _viewModel.Task2ProgressMax = progressMax;

            Task<bool> ss = Task.Run<bool>(() =>
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
                _viewModel.Task2ProgressValue += 1;
                return ssExecuter.CurrentStatus == IStepExecuter.Status.Completed;
            });

            Task<bool> bl = Task.Run<bool>(() =>
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
                _viewModel.Task2ProgressValue += 1;
                return blExecuter.CurrentStatus == IStepExecuter.Status.Completed;
            });

            await Task.WhenAll(ss, bl);

            _viewModel.Task2ProgressValue = progressMax;

            return (ss.Result, bl.Result);
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

        class CsvTarget
        {
            public string Target = "";
            public string Name = "";
            public string Format = "";

            public CsvTarget(string target, string? name = null, string? format = null)
            {
                Target = target;
                Name = name ?? target;
                if (format != null)
                {
                    Format = format;
                }
                else
                {
                    Format = "{0}";
                    if (target == Config.ColumnTagMapScoreSaberRankedDate ||
                        target == Config.ColumnTagMapBeatLeaderRankedDate ||
                        target == Config.ColumnTagScoreSaberDate ||
                        target == Config.ColumnTagBeatLeaderDate)
                    {
                        Format = "{0:yyyy/MM/dd HH:mm:ss (ddd)}";
                    }
                    if (target == Config.ColumnTagMapSongName ||
                        target == "Map.SongSubName" ||
                        target == "Map.SongAuthorName" ||
                        target == "Map.MapperName" ||
                        target == Config.ColumnTagScoreSaberModifiers ||
                        target == Config.ColumnTagBeatLeaderModifiers)
                    {
                        Format = "\"{0}\"";
                    }
                }
            }
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

            List<CsvTarget> csvTargets = new();
            if (System.IO.File.Exists(System.IO.Path.Combine("data", "csv_format.json")))
            {
                try
                {
                    string jsonText = System.IO.File.ReadAllText(System.IO.Path.Combine("data", "csv_format.json"));
                    JArray jsonArray = JArray.Parse(jsonText);
                    foreach (JObject obj in jsonArray.Cast<JObject>())
                    {
                        string? name = (string?)obj["Name"];
                        string? target = (string?)obj["Target"];
                        string? format = (string?)obj["Format"];
                        if (name != null && target != null)
                        {
                            csvTargets.Add(new CsvTarget(target, name, format));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex.ToString());
                }
            }
            if (csvTargets.Count == 0)
            {
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapBsr));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapSongName));
                csvTargets.Add(new CsvTarget("Map.SongSubName"));
                csvTargets.Add(new CsvTarget("Map.SongAuthorName"));
                csvTargets.Add(new CsvTarget("Map.MapperName"));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapMode));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapDifficulty));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapDuration));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapBpm));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapNotes));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapNps));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapNjs));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapBombs));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapWalls));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapHash));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapScoreSaberRankedDate));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapScoreSaberStar));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapBeatLeaderRankedDate));
                csvTargets.Add(new CsvTarget(Config.ColumnTagMapBeatLeaderStar));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberDate));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberScore));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberAcc));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberAccDiff));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberMissPlusBad));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberFullCombo));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberPp));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberModifiers));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberScoreCount));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberMiss));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberBad));
                csvTargets.Add(new CsvTarget(Config.ColumnTagScoreSaberWorldRank));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderDate));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderScore));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderAcc));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderAccDiff));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderMissPlusBad));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderFullCombo));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderPp));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderModifiers));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderScoreCount));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderMiss));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderBad));
                csvTargets.Add(new CsvTarget(Config.ColumnTagBeatLeaderWorldRank));
            }

            string delmiter = ",";
            StringBuilder sb = new();

            // ヘッダーを設定
            foreach (var csvTarget in csvTargets)
            {
                sb.Append(csvTarget.Name);
                sb.Append(delmiter);
            }
            if (sb.Length > 0)
            {
                sb.Length--; // 最後の文字を削除
            }
            sb.Append(Environment.NewLine);

            // データを設定
            foreach (var item in XaDataGrid.Items)
            {
                if (item is IntegrationScore i)
                {
                    foreach (var csvTarget in csvTargets)
                    {
                        try
                        {
                            switch (csvTarget.Target)
                            {
                                case Config.ColumnTagMapBsr:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Key));
                                    break;
                                case Config.ColumnTagMapSongName:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.SongName.Replace("\"", "").Replace(",", "")));
                                    break;
                                case "Map.SongSubName":
                                    sb.Append(string.Format(csvTarget.Format, i.Map.SongSubName.Replace("\"", "").Replace(",", "")));
                                    break;
                                case "Map.SongAuthorName":
                                    sb.Append(string.Format(csvTarget.Format, i.Map.SongAuthorName.Replace("\"", "").Replace(",", "")));
                                    break;
                                case "Map.MapperName":
                                    sb.Append(string.Format(csvTarget.Format, i.Map.MapperName.Replace("\"", "").Replace(",", "")));
                                    break;
                                case Config.ColumnTagMapMode:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.MapMode));
                                    break;
                                case Config.ColumnTagMapDifficulty:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.MapDifficulty));
                                    break;
                                case Config.ColumnTagMapDuration:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Duration));
                                    break;
                                case Config.ColumnTagMapBpm:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Bpm));
                                    break;
                                case Config.ColumnTagMapNotes:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Notes));
                                    break;
                                case Config.ColumnTagMapNps:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Nps));
                                    break;
                                case Config.ColumnTagMapNjs:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Njs));
                                    break;
                                case Config.ColumnTagMapBombs:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Bombs));
                                    break;
                                case Config.ColumnTagMapWalls:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Walls));
                                    break;
                                case Config.ColumnTagMapHash:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.Hash));
                                    break;
                                case Config.ColumnTagMapScoreSaberRankedDate:
                                    sb.Append(i.Map.ScoreSaber.RankedTime == null ? "" : string.Format(csvTarget.Format, i.Map.ScoreSaber.RankedTime?.ToLocalTime()));
                                    break;
                                case Config.ColumnTagMapScoreSaberStar:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.ScoreSaber.Star));
                                    break;
                                case Config.ColumnTagMapBeatLeaderRankedDate:
                                    sb.Append(i.Map.BeatLeader.RankedTime == null ? "" : string.Format(csvTarget.Format, i.Map.BeatLeader.RankedTime?.ToLocalTime()));
                                    break;
                                case Config.ColumnTagMapBeatLeaderStar:
                                    sb.Append(string.Format(csvTarget.Format, i.Map.BeatLeader.Star));
                                    break;
                                case Config.ColumnTagScoreSaberDate:
                                    sb.Append(i.ScoreSaber.TimeSet == null ? "" : string.Format(csvTarget.Format, i.ScoreSaber.TimeSet?.ToLocalTime()));
                                    break;
                                case Config.ColumnTagScoreSaberScore:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.ModifiedScore));
                                    break;
                                case Config.ColumnTagScoreSaberAcc:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.Acc));
                                    break;
                                case Config.ColumnTagScoreSaberAccDiff:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.AccDifference));
                                    break;
                                case Config.ColumnTagScoreSaberMissPlusBad:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.MissPlusBad));
                                    break;
                                case Config.ColumnTagScoreSaberFullCombo:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.FullCombo));
                                    break;
                                case Config.ColumnTagScoreSaberPp:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.PP));
                                    break;
                                case Config.ColumnTagScoreSaberModifiers:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.Modifiers.Replace("\"", "").Replace(",", ".")));
                                    break;
                                case Config.ColumnTagScoreSaberScoreCount:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.ScoreCount));
                                    break;
                                case Config.ColumnTagScoreSaberMiss:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.Miss));
                                    break;
                                case Config.ColumnTagScoreSaberBad:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.Bad));
                                    break;
                                case Config.ColumnTagScoreSaberWorldRank:
                                    sb.Append(string.Format(csvTarget.Format, i.ScoreSaber.WorldRank));
                                    break;
                                case Config.ColumnTagBeatLeaderDate:
                                    sb.Append(i.BeatLeader.TimeSet == null ? "" : string.Format(csvTarget.Format, i.BeatLeader.TimeSet?.ToLocalTime()));
                                    break;
                                case Config.ColumnTagBeatLeaderScore:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.ModifiedScore));
                                    break;
                                case Config.ColumnTagBeatLeaderAcc:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.Acc));
                                    break;
                                case Config.ColumnTagBeatLeaderAccDiff:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.AccDifference));
                                    break;
                                case Config.ColumnTagBeatLeaderMissPlusBad:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.MissPlusBad));
                                    break;
                                case Config.ColumnTagBeatLeaderFullCombo:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.FullCombo));
                                    break;
                                case Config.ColumnTagBeatLeaderPp:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.PP));
                                    break;
                                case Config.ColumnTagBeatLeaderModifiers:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.Modifiers.Replace("\"", "").Replace(",", ".")));
                                    break;
                                case Config.ColumnTagBeatLeaderScoreCount:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.ScoreCount));
                                    break;
                                case Config.ColumnTagBeatLeaderMiss:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.Miss));
                                    break;
                                case Config.ColumnTagBeatLeaderBad:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.Bad));
                                    break;
                                case Config.ColumnTagBeatLeaderWorldRank:
                                    sb.Append(string.Format(csvTarget.Format, i.BeatLeader.WorldRank));
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Warn($"Target:{csvTarget.Target}, Format:{csvTarget.Format}, Exception:{ex}");
                        }
                        sb.Append(delmiter);
                    }
                    if (sb.Length > 0)
                    {
                        sb.Length--; // 最後の文字を削除
                    }
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

        private void debugPrintColumns(string pre)
        {
            var s = pre;
            foreach (var c in XaDataGrid.Columns)
            {
                s += $"{TagBehavior.GetTag(c).ToString()}={c.DisplayIndex}, ";
            }
            System.Diagnostics.Debug.WriteLine(s);
        }

        private void XaDataGrid_ColumnDisplayIndexChanged(object? sender, DataGridColumnEventArgs e)
        {
            // do nothing
        }

        private void XaDataGrid_ColumnReordering(object sender, DataGridColumnReorderingEventArgs e)
        {
            _preDisplayIndex = e.Column.DisplayIndex;
            debugPrintColumns("1: ");
        }

        private void XaDataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            var alreadyMovedTagName = TagBehavior.GetTag(e.Column).ToString();
            if (alreadyMovedTagName == null)
            {
                return;
            }

            var pairTagName = Config.GetPairTagName(alreadyMovedTagName);
            debugPrintColumns("2: ");

            if (!pairTagName.Equals("unknown"))
            {
                // ScoreSaber、BeatLeader のペアが存在する列のいずれかが移動されたとき
                // 仮にペアを「T1, T2」として「T1, T2, A, B, C」という列が存在したとすると
                // T1 または T2 のいずれかの列が移動されたときは  T1 と T2 が隣り合い T1, T2 の順番となるようにしたい
                // 例1. GUI で T1 を左に動かしたとき
                //   ① A, B, C, T1, T2: GUIで動かす前
                //   ② A, T1, B, C, T2: GUIで動かした後 (この関数が開始した時点の順番)
                //   ③ A, T1, T2, B, C: T2 を T1 の右にくるように移動させる (この関数の処理)
                //   → T2 に設定する DisplayIndex は②時点の T1 の DisplayIndex + 1 となる
                // 例2. GUI で T2 を左に動かしたとき
                //   ① A, B, C, T1, T2: GUIで動かす前
                //   ② A, T2, B, C, T1: GUIで動かした後 (この関数が開始した時点の順番)
                //   ③ A, T1, T2, B, C: T1 を T2 の左にくるように移動させる (この関数の処理)
                //   → T1 に設定する DisplayIndex は②時点の T2 の DisplayIndex となる
                // 例3. GUI で T1 を右に動かしたとき
                //   ① T1, T2, A, B, C: GUIで動かす前
                //   ② T2, A, T1, B, C: GUIで動かした後 (この関数が開始した時点の順番)
                //   ③ A, T1, T2, B, C: T2 を T1 の右にくるように移動させる (この関数の処理)
                //   → T2 を動かすと A, T1 の位置が左にずれるので T2 に設定する DisplayIndex は②時点の T1 の DisplayIndex となる
                // 例4. GUI で T2 を右に動かしたとき
                //   ① T1, T2, A, B, C: GUIで動かす前
                //   ② T1, A, T2, B, C: GUIで動かした後 (この関数が開始した時点の順番)
                //   ③ A, T1, T2, B, C: T1 を T2 の左にくるように移動させる (この関数の処理)
                //   → T1 を動かすと A, T2 の位置が左にずれるので T1 に設定する DisplayIndex は②時点の T2 の DisplayIndex - 1 となる

                // GUI で移動した列の DisplayIndex に対してペアとなる列の DisplayIndex の差分。
                static int diffrenceDisplayIndex(bool moveNowItemIsScoreSaber, bool isMoveToRight)
                {
                    int diff = 0;

                    // ScoreSaberの項目を移動させるときはBeatLeaderのDisplyIndexを指定すればよい。同じ値になったBeatLeaderは勝手に右にずれる。
                    // BeatLeaderの項目を移動させるときはScoreSaberのDisplayIndex+1を指定すればよい。
                    diff += (moveNowItemIsScoreSaber) ? 0 : 1;

                    // 左から右に移動するときは自身が抜けて他の列が左にずれる分を考慮してマイナスが必要。
                    diff += (isMoveToRight) ? -1 : 0;
                    return diff;
                }

                var moveNowItemIsScoreSaber = pairTagName.Contains("ScoreSaber");
                bool isMoveToRight = _preDisplayIndex < e.Column.DisplayIndex;
                _dataGridColumnsDic[pairTagName].DisplayIndex = e.Column.DisplayIndex + diffrenceDisplayIndex(moveNowItemIsScoreSaber, isMoveToRight);
            }

            // さらに ScoreSaber と BeatLeader のペアとなる列が隣り合わなくなっている場所は隣りあうように移動させる。
            static void moveSideBySide(Dictionary<string, DataGridColumn> d, bool viewTargetIsScoreSaber, string scoresaberTag, string beatleaderTag)
            {
                if (d[beatleaderTag].DisplayIndex != d[scoresaberTag].DisplayIndex + 1)
                {
                    if (viewTargetIsScoreSaber)
                    {
                        // ScoreSaber の列を表示しているときは非表示の BeatLeader の列を移動させる
                        d[beatleaderTag].DisplayIndex = d[scoresaberTag].DisplayIndex + 1;
                    }
                    else
                    {
                        // BeatLeader の列を表示しているときは非表示の ScoreSaber の列を移動させる
                        d[scoresaberTag].DisplayIndex = d[beatleaderTag].DisplayIndex - 1;
                    }
                }
            }
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberDate, Config.ColumnTagBeatLeaderDate);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberWorldRank, Config.ColumnTagBeatLeaderWorldRank);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberScore, Config.ColumnTagBeatLeaderScore);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberAcc, Config.ColumnTagBeatLeaderAcc);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberAccDiff, Config.ColumnTagBeatLeaderAccDiff);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberMissPlusBad, Config.ColumnTagBeatLeaderMissPlusBad);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberFullCombo, Config.ColumnTagBeatLeaderFullCombo);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberPp, Config.ColumnTagBeatLeaderPp);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberModifiers, Config.ColumnTagBeatLeaderModifiers);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberScoreCount, Config.ColumnTagBeatLeaderScoreCount);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberMiss, Config.ColumnTagBeatLeaderMiss);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagScoreSaberBad, Config.ColumnTagBeatLeaderBad);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagMapScoreSaberRankedDate, Config.ColumnTagMapBeatLeaderRankedDate);
            moveSideBySide(_dataGridColumnsDic, _viewModel.ViewTargetHasScoreSaber, Config.ColumnTagMapScoreSaberStar, Config.ColumnTagMapBeatLeaderStar);

            debugPrintColumns("3: ");
        }

        private void XaDataGrid_ColumnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            // do nothing
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

        public MainPageFilterViewModel FilterValue { get => AppData.MainPageFilter; }

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
