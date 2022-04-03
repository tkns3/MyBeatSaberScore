using System;
using System.Collections.Generic;
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
    /// PageFilter.xaml の相互作用ロジック
    /// </summary>
    public partial class PageFilter : Page
    {
        readonly PageMain.FilterValue? _filterValue;

        public PageFilter()
        {
            InitializeComponent();
            _filterValue = (PageMain.FilterValue?)Application.Current.Properties["FilterValue"];
            DataContext = _filterValue;
        }
    }
}
