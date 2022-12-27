using Microsoft.Win32;
using MyBeatSaberScore.APIs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using MyBeatSaberScore;

namespace MyBeatSaberScore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Config.LoadLocalFile();
            this.Title = $"MyBeatSaberScore v{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}";

            Application.Current.Properties["XaTabControl"] = XaTabControl;
            Application.Current.Properties["XaTabMain"] = XaTabMain;
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            await Updater.FetchReleasesAsync();
            if (Updater.IsExistNewVersion && Updater.LatestVersion > Config.SkipVersion)
            {
                XaImageMenuUpdate.Source = new BitmapImage(new Uri("Resources/menu_update.png", UriKind.Relative));

                var dlg = new MessageBoxEx();
                dlg.DlgWidth = 500;
                dlg.TextBlock.Inlines.Add($"新しいバージョン「v{Updater.LatestVersion}」が利用可能です。\n\n");
                dlg.TextBlock.Inlines.Add("アップデート ⇒ 最新の実行ファイルをダウンロードして自動的に再起動します\n");
                dlg.TextBlock.Inlines.Add($"スキップ ⇒ 次の起動時から「v{Updater.LatestVersion}」のお知らせを表示しません\n\n");
                dlg.TextBlock.Inlines.Add("(スキップしてもメニューからいつでもアップデートできます)");
                dlg.Owner = Application.Current.MainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.Button = MessageBoxButton.OKCancel;
                dlg.Image = MessageBoxImage.Information;
                dlg.Result = MessageBoxResult.Cancel;
                dlg.ButtonTextOK = "アップデート";
                dlg.ButtonTextCancel = "スキップ";
                dlg.ShowDialog();
                if (dlg.Result == MessageBoxResult.Yes)
                {
                    await Updater.StartUpdate();
                }
                else
                {
                    if (Updater.LatestVersion != null)
                    {
                        Config.SkipVersion = Updater.LatestVersion;
                    }
                }
            }
        }
    }
}
