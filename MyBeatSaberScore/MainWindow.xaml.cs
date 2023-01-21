using MyBeatSaberScore.BeatMap;
using MyBeatSaberScore.Utility;
using System;
using System.Reflection;
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
            });
            sw.Stop();

            var maxDelay = 1850;
            if (sw.ElapsedMilliseconds < maxDelay)
            {
                await Task.Delay((int)(maxDelay - sw.ElapsedMilliseconds));
            }

            XaFrame.Source = new Uri("/PageTabs.xaml", UriKind.Relative);
        }
    }
}
