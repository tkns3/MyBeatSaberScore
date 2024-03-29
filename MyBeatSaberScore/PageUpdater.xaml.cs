﻿using MyBeatSaberScore.Utility;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MyBeatSaberScore
{
    /// <summary>
    /// PageUpdater.xaml の相互作用ロジック
    /// </summary>
    public partial class PageUpdater : Page
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public PageUpdater()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            XaTextCurrentVersion.Text = $"現在のバージョン：{Updater.CurrentVersion}";
            XaTextLatestVersion.Text = $"最新のバージョン：{Updater.LatestVersion}";

            XaButtonUpdate.IsEnabled = Updater.LatestVersion > Updater.CurrentVersion;

            string t = "";
            foreach (var r in Updater.ReleasesCache)
            {
                t += "----\n";
                t += r.tag_name + "\n";
                t += r.body + "\n\n";
            }
            XaTextUpdateHistory.Text = t;
        }

        private async void XaButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new MessageBoxEx();
                dlg.TextBlock.Inlines.Add("アップデートを開始しますか？\n");
                dlg.TextBlock.Inlines.Add("最新の実行ファイルをダウンロードして自動的に再起動します。");
                dlg.Owner = Application.Current.MainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.Button = MessageBoxButton.OKCancel;
                dlg.Image = MessageBoxImage.Question;
                dlg.Result = MessageBoxResult.No;
                dlg.ShowDialog();
                if (dlg.Result == MessageBoxResult.OK)
                {
                    await Updater.StartUpdate();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
            }
        }
    }
}
