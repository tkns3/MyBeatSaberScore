using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyBeatSaberScore.UserControls
{
    /// <summary>
    /// ClearableTextBox.xaml の相互作用ロジック
    /// </summary>
    public partial class ClearableTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ClearableTextBox), new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(ClearableTextBox), new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty TextChangedProperty =
            DependencyProperty.Register("TextChanged", typeof(TextChangedEventHandler), typeof(ClearableTextBox));

        public TextChangedEventHandler TextChanged
        {
            get { return (TextChangedEventHandler)GetValue(TextChangedProperty); }
            set { SetValue(TextChangedProperty, value); }
        }


        public bool ImeFlag => TextBox._imeFlag;

        public ClearableTextBox()
        {
            InitializeComponent();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Text = string.Empty;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ClearButton.Visibility = string.IsNullOrEmpty(((PlaceholderTextBox)sender).Text) ? Visibility.Collapsed : Visibility.Visible;
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
        }

        public void SetTextBoxUpdateSourceTrigger(UpdateSourceTrigger trigger)
        {
            var newBinding = new Binding("Text")
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = trigger
            };

            TextBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, newBinding);
        }
    }
}
