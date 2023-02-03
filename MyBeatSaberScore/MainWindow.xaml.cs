using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyBeatSaberScore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"MyBeatSaberScore v{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}";
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
            Config.SaveToLocalFile();
        }
    }
}
