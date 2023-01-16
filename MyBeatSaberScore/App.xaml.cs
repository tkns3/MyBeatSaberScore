using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace MyBeatSaberScore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public App()
        {
            // 埋め込みリソースからロガーの設定を取得する
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifes‌​tResourceNames().FirstOrDefault(x => x.EndsWith("log4net.config"));
            if (resourceName != null)
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                log4net.Config.XmlConfigurator.Configure(stream);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Updater.Initialize(e.Args);
            Utility.HttpTool.Client.DefaultRequestHeaders.Add("User-Agent", $"MyBeatSaberScore/" + Updater.CurrentVersion);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _logger.Info("Start");

            // UIスレッドの未処理例外で発生
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            // UIスレッド以外の未処理例外で発生
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            // それでも処理されない例外で発生
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            HandleException(exception);
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception.InnerException as Exception;
            HandleException(exception);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            HandleException(exception);
        }

        private void HandleException(Exception? e)
        {
            _logger.Error(e?.ToString());
            MessageBox.Show($"エラーが発生しました\n{e?.ToString()}");
            Environment.Exit(1);
        }
    }
}
