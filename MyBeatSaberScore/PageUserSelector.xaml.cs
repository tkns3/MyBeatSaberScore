﻿using MyBeatSaberScore.APIs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// PageUserSelector.xaml の相互作用ロジック
    /// </summary>
    public partial class PageUserSelector : Page
    {
        static CollectionViewSource _usersSource = new() { Source = Config.FavoriteUsers };

        public PageUserSelector()
        {
            InitializeComponent();

            xaListView.ItemsSource = _usersSource.View;
        }

        private async void Button_Click_AddUser(object sender, RoutedEventArgs e)
        {
            var profile = await ScoreSaber.GetPlayerInfo(xaProfileId.Text);
            if (profile.id.Length > 0)
            {
                Config.FavoriteUsers.Add(new Config.User(profile.id, profile.name));
                Config.SaveLocalFile();
            }
            else
            {
                MessageBox.Show($"ユーザーのデータを取得できません。IDが間違っているもしくは通信に失敗した可能性があります。");
            }
        }

        private void Button_Click_DelUser(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Config.User obj)
            {
                Config.FavoriteUsers.Remove(obj);
                Config.SaveLocalFile();
            }
        }

        private void Button_Click_SelectUser(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Config.User obj)
            {
                Config.ScoreSaberProfileId = obj.id;

                if (Application.Current.Properties["xaTabMain"] is TabItem tabItem)
                {
                    tabItem.IsSelected = true;
                }
            }
        }
    }
}