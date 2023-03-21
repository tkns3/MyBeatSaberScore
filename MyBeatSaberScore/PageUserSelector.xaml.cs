using MyBeatSaberScore.APIs;
using MyBeatSaberScore.Model;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyBeatSaberScore
{
    /// <summary>
    /// PageUserSelector.xaml の相互作用ロジック
    /// </summary>
    public partial class PageUserSelector : Page
    {
        static readonly CollectionViewSource _usersSource = new() { Source = Config.FavoriteUsers };

        public PageUserSelector()
        {
            InitializeComponent();

            XaListView.ItemsSource = _usersSource.View;
        }

        private async void OnClickAddUser(object sender, RoutedEventArgs e)
        {
            var scoreSaberUserData = new ScoreSaberUserData(XaProfileId.Text);
            var beatLeaderUserData = new BeatLeaderUserData(XaProfileId.Text);

            await Task.Run(() =>
            {
                if (!scoreSaberUserData.IsExistProfile)
                {
                    scoreSaberUserData.FetchLatestProfile();
                }

                if (!beatLeaderUserData.IsExistProfile)
                {
                    beatLeaderUserData.FetchLatestProfile();
                }
            });

            if (scoreSaberUserData.IsExistProfile || beatLeaderUserData.IsExistProfile)
            {
                Config.FavoriteUsers.Add(new Config.User(XaProfileId.Text, beatLeaderUserData.Profile, scoreSaberUserData.Profile));
                Config.SaveToLocalFile();
            }
            else
            {
                MessageBox.Show($"プロフィールデータを取得できません。IDが間違っているもしくは通信に失敗した可能性があります。");
            }
        }

        private void OnClickDelUser(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Config.User obj)
            {
                Config.FavoriteUsers.Remove(obj);
                Config.SaveToLocalFile();
            }
        }

        private void OnClickSelectUser(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Config.User obj)
            {
                Config.ScoreSaberProfileId = obj.id;
                PageTabs.Instance?.SelectMainTab();
            }
        }

        private async void OnClickReload(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Config.User obj)
            {
                var beatLeaderProfile = await BeatLeader.GetPlayerInfo(obj.id);
                if (beatLeaderProfile != null && beatLeaderProfile.id.Length > 0)
                {
                    obj.beatLeaderName = beatLeaderProfile.name;
                    obj.beatLeaderAvatar = beatLeaderProfile.avatar;
                }
                var scoreSaberProfile = await ScoreSaber.GetPlayerInfo(obj.id);
                if (scoreSaberProfile != null && scoreSaberProfile.id.Length > 0)
                {
                    obj.scoreSaberName = scoreSaberProfile.name;
                    obj.scoreSaberAvatar = scoreSaberProfile.profilePicture;
                }
                _usersSource.View.Refresh();
                Config.SaveToLocalFile();
            }
        }
    }
}
