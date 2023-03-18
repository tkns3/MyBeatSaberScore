using MyBeatSaberScore.Utility;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyBeatSaberScore
{
    /// <summary>
    /// PageTabs.xaml の相互作用ロジック
    /// </summary>
    public partial class PageTabs : Page
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static PageTabs? Instance { get; private set; }

        public PageTabs()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            if (Updater.IsExistNewVersion)
            {
                XaImageMenuUpdate.Source = new BitmapImage(new Uri("Resources/menu_update.png", UriKind.Relative));
            }
        }

        public void SelectMainTab()
        {
            if (XaTabMain != null)
            {
                XaTabMain.IsSelected = true;
            }
        }
    }
}
