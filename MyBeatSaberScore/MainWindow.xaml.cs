using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MyBeatSaberScore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static MainWindow? Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Title = $"MyBeatSaberScore v{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}";
            RecoverWindowBounds();
            Instance = this;
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            await Task.Run(async () =>
            {
                await Updater.FetchReleasesAsync();
                BeatMapDic.Initialize();
                AppData.SelectedUser.ProfileId = Config.ScoreSaberProfileId;
                AppData.SelectedUser.BeatLeader = new(Config.ScoreSaberProfileId);
                AppData.SelectedUser.ScoreSaber = new(Config.ScoreSaberProfileId);
                AppData.SelectedUser.BeatLeader.LoadAllFromLocalFile();
                AppData.SelectedUser.ScoreSaber.LoadAllFromLocalFile();
                AppData.SelectedUser.ConstractScoresOfPlayedAndAllRanked();
            });

            var maxDelay = 1850;
            while (sw.ElapsedMilliseconds < maxDelay)
            {
                await Task.Delay(100);
            }

            XaFrame.Source = new Uri("/PageTabs.xaml", UriKind.Relative);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PageMain.Instance != null)
            {
                var columnParams = PageMain.Instance.GetGridColumnParams();
                if (columnParams != null)
                {
                    Config.Grid.columnRestore.lastParams = columnParams;
                }
            }

            Config.Window.boundsRestore.last = GetWindowBounds();

            Config.SaveToLocalFile();
        }

        internal Config.WindowBounds GetWindowBounds()
        {
            return new()
            {
                top = Top,
                left = Left,
                width = Width,
                height = Height,
                vtop = SystemParameters.VirtualScreenTop,
                vleft = SystemParameters.VirtualScreenLeft,
                vwidth = SystemParameters.VirtualScreenWidth,
                vheight = SystemParameters.VirtualScreenHeight,
                maximized = WindowState == WindowState.Maximized,
            };
        }

        private void RecoverWindowBounds()
        {
            Config.WindowBounds bounds = Config.Window.boundsRestore.mode switch
            {
                Config.RestoreMode.Last => Config.Window.boundsRestore.last,
                Config.RestoreMode.Saved => Config.Window.boundsRestore.saved,
                _ => new()
            };

            var vtop = SystemParameters.VirtualScreenTop;
            var vleft = SystemParameters.VirtualScreenLeft;
            var vwidth = SystemParameters.VirtualScreenWidth;
            var vheight = SystemParameters.VirtualScreenHeight;

            if (bounds.vtop != vtop || bounds.vleft != vleft || bounds.vwidth != vwidth || bounds.vheight != vheight)
            {
                // ディスプレイのサイズや配置が変わっている場合は復元しない
                return;
            }

            Top = bounds.top;
            Left = bounds.left;
            Width = bounds.width;
            Height = bounds.height;

            if (bounds.maximized)
            {
                // ロード後に最大化
                Loaded += (o, e) => WindowState = WindowState.Maximized;
            }
        }
    }
}
