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
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MyBeatSaberScore"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MyBeatSaberScore;assembly=MyBeatSaberScore"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:PlaceholderTextBox/>
    ///
    /// </summary>
    public class PlaceholderTextBox : TextBox
    {
        //プレースホルダ―用のプロパティを追加
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string),
                      typeof(PlaceholderTextBox), new PropertyMetadata(null, OnPlaceHolderChanged));

        private string _placeholder = string.Empty;
        /// <summary>
        /// プレースホルダーのプロパティを設定
        /// </summary>
        public string Placeholder
        {
            get
            {
                return _placeholder;
            }
            set
            {
                _placeholder = value;
            }
        }

        /// <summary>
        /// プレースホルダ―が変更されたときの変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPlaceHolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }
            //設定された文言をプレースホルダにいれます
            var placeHolder = e.NewValue as string ?? "";
            var handler = CreateEventHandler(placeHolder);
            if (string.IsNullOrEmpty(placeHolder))
            {
                textBox.TextChanged -= handler;
            }
            else
            {
                textBox.TextChanged += handler;
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    //プレースホルダ―の背景に文字列を設定します
                    textBox.Background = CreateVisualBrush(placeHolder);
                }
            }
        }

        /// <summary>
        /// textChangedイベント作成
        /// </summary>
        /// <param name="placeHolder"></param>
        /// <returns></returns>
        private static TextChangedEventHandler CreateEventHandler(string placeHolder)
        {
            return (sender, e) =>
            {
                var textBox = (TextBox)sender;
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    //Textが入力されていなければプレースホルダーを表示します
                    textBox.Background = CreateVisualBrush(placeHolder);
                }
                else
                {
                    //Textが入力されたときに背景を真っ白にします
                    textBox.Background = new SolidColorBrush(Colors.White);
                }
            };
        }

        /// <summary>
        /// 背景色を変更します
        /// </summary>
        /// <param name="placeHolder">PlaceHolderに設定された文言</param>
        /// <returns>塗りつぶしの色を返します</returns>
        private static VisualBrush CreateVisualBrush(string placeHolder)
        {
            //入力された文言でTextBlockを生成します
            var visual = new TextBlock()
            {
                Text = placeHolder,
                Padding = new Thickness(2, 1, 500, 1),
                Foreground = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.White)
            };
            //生成された塗りつぶしの色を返します
            return new VisualBrush(visual)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Center,
            };
        }

        /// <summary>
        /// プレースホルダーをsetしたときの処理
        /// </summary>
        /// <param name="textbox">自分自身のTextBox</param>
        /// <param name="placeHolder">PlaceHolderに設定された文言</param>
        private static void SetPlaceHolderText(TextBox textbox, string placeHolder)
        {
            textbox.SetValue(PlaceholderProperty, placeHolder);
        }

        /// <summary>
        /// プレースホルダー取得処理
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns>プレースホルダーで設定された文字を返します</returns>
        public static string GetPlaceHolderText(TextBox textBox)
        {
            return textBox.GetValue(PlaceholderProperty) as string ?? "";
        }
    }
}
