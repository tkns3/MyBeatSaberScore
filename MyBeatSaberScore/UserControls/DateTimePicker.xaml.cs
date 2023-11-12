using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MyBeatSaberScore.UserControls
{
    /// <summary>
    /// DateTimePicker.xaml の相互作用ロジック
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        public DateTimePicker()
        {
            InitializeComponent();
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += 15)
                {
                    _timeList.Add($"{hour:00}:{minute:00}");
                }
            }
            TimeListBox1.ItemsSource = _timeList;
        }

        private readonly List<string> _timeList = new List<string>();

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register(
                "SelectedDateTime", typeof(DateTimeOffset?), typeof(DateTimePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged));

        public DateTimeOffset? SelectedDateTime
        {
            get
            {
                return (DateTimeOffset?)GetValue(SelectedDateTimeProperty);
            }
            set
            {
                SetValue(SelectedDateTimeProperty, value);

                Calendar1.SelectedDatesChanged -= Calendar1_SelectedDatesChanged;
                TimeListBox1.SelectionChanged -= TimeListBox1_SelectionChanged;

                if (value == null)
                {
                    ClearButton.Visibility = Visibility.Collapsed;  
                    Calendar1.SelectedDate = null;
                    TimeListBox1.SelectedIndex = 0;
                    TextBox1.Text = "";
                }
                else
                {
                    ClearButton.Visibility = Visibility.Visible;
                    Calendar1.SelectedDate = value?.LocalDateTime;
                    TimeListBox1.SelectedIndex = _timeList.FindIndex(s => s == value?.ToString("HH:mm"));
                    TextBox1.Text = value?.ToString("yyyy/MM/dd HH:mm");
                }

                Calendar1.SelectedDatesChanged += Calendar1_SelectedDatesChanged;
                TimeListBox1.SelectionChanged += TimeListBox1_SelectionChanged;
            }
        }

        public static readonly DependencyProperty DefaultTimeProperty =
            DependencyProperty.Register("DefaultTime", typeof(string), typeof(DateTimePicker), new PropertyMetadata("00:00"));

        public string DefaultTime
        {
            get { return (string)GetValue(DefaultTimeProperty); }
            set { SetValue(DefaultTimeProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(DateTimePicker), new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker)
            {
                dateTimePicker.SelectedDateTime = (DateTimeOffset?)e.NewValue;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedDateTime = null;
        }

        private void DatePickButton_Click(object sender, RoutedEventArgs e)
        {
            DatePickupPopup.IsOpen = true;
        }

        private void TimePickButton_Click(object sender, RoutedEventArgs e)
        {
            TimePickupPopup.IsOpen = true;
        }

        private void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            SelectedDateTime = DateTimeOffset.TryParse(TextBox1.Text, out DateTimeOffset date) ? date : null;
        }

        private void Calendar1_SelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
        {
            DatePickupPopup.IsOpen = false;
            if (Calendar1.SelectedDate != null)
            {
                DateTime date = Calendar1.SelectedDate.Value;
                string time = (string)DefaultTime;
                int hour = SelectedDateTime?.Hour ?? int.Parse(time.Substring(0, 2));
                int minute = SelectedDateTime?.Minute ?? int.Parse(time.Substring(3, 2));
                SelectedDateTime = new DateTimeOffset(date.Year, date.Month, date.Day, hour, minute, 0, TimeZoneInfo.Local.BaseUtcOffset);
            }
            else
            {
                SelectedDateTime = null;
            }
        }

        private void TimeListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TimePickupPopup.IsOpen = false;
            if (TimeListBox1.SelectedItem != null)
            {
                DateTime date = Calendar1.SelectedDate ?? DateTime.Now;
                string time = (string)TimeListBox1.SelectedItem;
                int hour = int.Parse(time.Substring(0, 2));
                int minute = int.Parse(time.Substring(3, 2));
                SelectedDateTime = new DateTimeOffset(date.Year, date.Month, date.Day, hour, minute, 0, TimeZoneInfo.Local.BaseUtcOffset);
            }
        }

        private void DatePickupPopup_Opened(object sender, EventArgs e)
        {
            Calendar1.DisplayDate = SelectedDateTime?.LocalDateTime ?? DateTime.Now;
        }

        private void TimePickupPopup_Opened(object sender, EventArgs e)
        {
            if (TimeListBox1.SelectedItem != null)
            {
                TimeListBox1.ScrollIntoView(TimeListBox1.SelectedItem);
            }
        }
    }
}
