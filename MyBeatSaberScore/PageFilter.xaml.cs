using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyBeatSaberScore
{
    /// <summary>
    /// PageFilter.xaml の相互作用ロジック
    /// </summary>
    public partial class PageFilter : Page
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly string _dataDir = Path.Combine("data");
        private static readonly string _filtersPath = Path.Combine(_dataDir, "filters.json");

        PageFilterViewModel _model;

        public PageFilter()
        {
            InitializeComponent();
            _model = (PageFilterViewModel)DataContext;
            _model.Save = SaveToFile;
            LoadFromFile();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var message = $"「{_model.FilterList[_model.CurrentIndex].FilterName}」を削除しますか？\n";
                var result = SaveDeleteConfirm(message);
                if (result == MessageBoxResult.OK)
                {
                    _model.FilterList.RemoveAt(_model.CurrentIndex);
                    _model.OnPropertyChanged("IsListSelected");
                    _model.FilterListSource.View.Refresh();
                    SaveToFile();
                    _model.Status = $"{DateTime.Now:HH:mm:ss} Delete Success.";
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                _model.Status = $"{DateTime.Now:HH:mm:ss} Delete Fail.";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var message = $"「{_model.CurrentFilterValue.FilterName}」に上書きしますか？\n";
                if (_model.FilterList[_model.CurrentIndex].FilterName != _model.CurrentFilterValue.FilterName)
                {
                    message += $"Filter Name が「{_model.FilterList[_model.CurrentIndex].FilterName}」から「{_model.CurrentFilterValue.FilterName}」に変わります。\n";
                }
                var result = SaveDeleteConfirm(message);
                if (result == MessageBoxResult.OK)
                {
                    _model.FilterList[_model.CurrentIndex].Value.CopyFrom(_model.CurrentFilterValue.Value);
                    _model.OnPropertyChanged("IsListSelected");
                    _model.FilterListSource.View.Refresh();
                    SaveToFile();
                    _model.Status = $"{DateTime.Now:HH:mm:ss} Save Success.";
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                _model.Status = $"{DateTime.Now:HH:mm:ss} Save Fail.";
            }
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var message = $"「{_model.CurrentFilterValue.FilterName}」を作成しますか？\n";
                var result = SaveDeleteConfirm(message);
                if (result == MessageBoxResult.OK)
                {
                    var filterListItem = new FilterListItem();
                    filterListItem.Value.CopyFrom(_model.CurrentFilterValue.Value);
                    _model.FilterList.Add(filterListItem);
                    _model.OnPropertyChanged("IsListSelected");
                    _model.FilterListSource.View.Refresh();
                    SaveToFile();
                    _model.Status = $"{DateTime.Now:HH:mm:ss} Save Success.";
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                _model.Status = $"{DateTime.Now:HH:mm:ss} Save Fail.";
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            _model.CurrentFilterValue.UpdateValue(_model.FilterList[_model.CurrentIndex].Value);
            _model.Status = $"{DateTime.Now:HH:mm:ss} Load Success.";
        }

        private MessageBoxResult SaveDeleteConfirm(string message)
        {
            var result = MessageBoxResult.OK;
            if (_model.IsConfirmBeforeSaveAndDelete)
            {
                var dlg = new MessageBoxEx();
                dlg.TextBlock.Inlines.Add(message);
                dlg.Owner = Application.Current.MainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.Button = MessageBoxButton.OKCancel;
                dlg.Image = MessageBoxImage.Question;
                dlg.Result = MessageBoxResult.No;
                dlg.ShowDialog();
                result = dlg.Result;
            }
            return result;
        }

        private void LoadFromFile()
        {
            _model.FilterList.Clear();
            if (File.Exists(_filtersPath))
            {
                try
                {
                    string jsonText = File.ReadAllText(_filtersPath);
                    JObject o = JObject.Parse(jsonText);
                    var confirm = (bool?)o["Confirm"];
                    var filters = (JArray?)o["Filters"];
                    if (filters != null)
                    {
                        foreach (JObject obj in filters.Cast<JObject>())
                        {
                            FilterListItem item = new();
                            item.Value.Parse(obj);
                            _model.FilterList.Add(item);
                        }
                    }
                    _model.IsConfirmBeforeSaveAndDelete = (confirm != null) && (bool)confirm;
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex.ToString());
                }
            }

            _model.OnPropertyChanged("IsListSelected");
            _model.FilterListSource.View.Refresh();
        }

        private void SaveToFile()
        {
            try
            {
                var o = new JObject
                {
                    new JProperty("Confirm", _model.IsConfirmBeforeSaveAndDelete),
                    new JProperty("Filters", GetJArray())
                };
                File.WriteAllText(_filtersPath, o.ToString());

            }
            catch (Exception ex)
            {
                _logger.Warn(ex.ToString());
            }
        }

        private JArray GetJArray()
        {
            var array = new JArray();
            foreach (var item in _model.FilterList)
            {
                array.Add(item.Value.GetJObject());

            }
            return array;
        }

        private void ConfirmCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }

        private void ConfirmCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }
    }

    public class FilterListItem
    {
        public MainPageFilterValue Value = new();
        public string FilterName { get { return Value.FilterName; } }
    }

    public class PageFilterViewModel : ObservableBase
    {
        public PageFilterViewModel()
        {
            FilterListSource = new() { Source = FilterList };
        }

        private bool _IsConfirmBeforeSaveAndDelete = false;
        private int _currentIndex;
        private string _status = "";

        public Action? Save;

        public bool IsConfirmBeforeSaveAndDelete { get => _IsConfirmBeforeSaveAndDelete; set => SetProperty(ref _IsConfirmBeforeSaveAndDelete, value); }

        public MainPageFilterViewModel CurrentFilterValue { get => AppData.MainPageFilter; }

        public ObservableCollection<FilterListItem> FilterList { get; set; } = new();

        public readonly CollectionViewSource FilterListSource;

        public ICollectionView FilterListItems { get => FilterListSource.View; }

        public int CurrentIndex
        {
            get { return this._currentIndex; }
            set { SetProperty(ref this._currentIndex, value); OnPropertyChanged("IsListSelected"); }
        }

        public bool IsListSelected { get => (0 <= _currentIndex && _currentIndex < FilterList.Count); }

        public Action<int> DropCallback { get => OnDrop; }

        private void OnDrop(int index)
        {
            if (index >= 0)
            {
                this.FilterList.Move(this.CurrentIndex, index);
                Save?.Invoke();
            }
        }

        public string Status { get => _status; set => SetProperty(ref _status, value); }

        public string DefaultTime { get => Config.Filter.defaultTime; }
    }
}
