using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace MyBeatSaberScore
{
    /// <summary>
    /// PageFilter.xaml の相互作用ロジック
    /// </summary>
    public partial class PageFilter : Page
    {
        public PageFilter()
        {
            InitializeComponent();
        }
    }

    public class PageFilterViewModel : ObservableBase
    {
        public FilterValue FilterValue { get => AppData.FilterValue; }
    }
}
