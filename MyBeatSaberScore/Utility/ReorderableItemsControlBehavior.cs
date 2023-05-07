using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyBeatSaberScore.Utility
{
    /// <summary>
    /// ItemsControl に対するドラッグ＆ドロップによる並べ替え動作をおこなうビヘイビアを表します。
    /// </summary>
    internal class ReorderableItemsControlBehavior
    {
        #region Callback 添付プロパティ

        /// <summary>
        /// Callback 添付プロパティの定義
        /// </summary>
        public static readonly DependencyProperty CallbackProperty = DependencyProperty.RegisterAttached("Callback", typeof(Action<int>), typeof(ReorderableItemsControlBehavior), new PropertyMetadata(null, OnCallbackPropertyChanged));

        /// <summary>
        /// Callback 添付プロパティを取得します。
        /// </summary>
        /// <param name="target">対象とする DependencyObject を指定します。</param>
        /// <returns>取得した値を返します。</returns>
        public static Action<int> GetCallback(DependencyObject target)
        {
            return (Action<int>)target.GetValue(CallbackProperty);
        }

        /// <summary>
        /// Callback 添付プロパティを設定します。
        /// </summary>
        /// <param name="target">対象とする DependencyObject を指定します。</param>
        /// <param name="value">設定する値を指定します。</param>
        public static void SetCallback(DependencyObject target, Action<int> value)
        {
            target.SetValue(CallbackProperty, value);
        }

        /// <summary>
        /// Callback 添付プロパティ変更イベントハンドラ
        /// </summary>
        /// <param name="d">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnCallbackPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = d as ItemsControl;
            if (itemsControl == null) return;

            if (GetCallback(itemsControl) != null)
            {
                itemsControl.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                itemsControl.PreviewMouseMove += OnPreviewMouseMove;
                itemsControl.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
                itemsControl.PreviewDragEnter += OnPreviewDragEnter;
                itemsControl.PreviewDragLeave += OnPreviewDragLeave;
                itemsControl.PreviewDrop += OnPreviewDrop;
            }
            else
            {
                itemsControl.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                itemsControl.PreviewMouseMove -= OnPreviewMouseMove;
                itemsControl.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
                itemsControl.PreviewDragEnter -= OnPreviewDragEnter;
                itemsControl.PreviewDragLeave -= OnPreviewDragLeave;
                itemsControl.PreviewDrop -= OnPreviewDrop;
            }
        }

        #endregion Callback 添付プロパティ

        #region イベントハンドラ

        /// <summary>
        /// ドラッグ中の一時データ
        /// </summary>
        private static DragDropObject temporaryData;

        /// <summary>
        /// ドラッグ＆ドロップに関するデータを表します。
        /// </summary>
        private class DragDropObject
        {
            /// <summary>
            /// ドラッグ開始座標を取得または設定します。
            /// </summary>
            public Point Start { get; set; }

            /// <summary>
            /// ドラッグ対象であるオブジェクトを取得または設定します。
            /// </summary>
            public FrameworkElement DraggedItem { get; set; }

            /// <summary>
            /// ドロップ可能かどうかを取得または設定します。
            /// </summary>
            public bool IsDroppable { get; set; }

            /// <summary>
            /// ドラッグを開始していいかどうかを確認します。
            /// </summary>
            /// <param name="current">現在のマウス座標を指定します。</param>
            /// <returns>十分マウスが移動している場合に true を返します。</returns>
            public bool CheckStartDragging(Point current)
            {
                return (current - this.Start).Length - MinimumDragPoint.Length > 0;
            }

            /// <summary>
            /// ドラッグ開始に必要な最短距離を示すベクトル
            /// </summary>
            private static readonly Vector MinimumDragPoint = new Vector(SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance);
        }

        /// <summary>
        /// PreviewMouseLeftButtonDown イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            temporaryData = new DragDropObject();
            temporaryData.Start = e.GetPosition(Window.GetWindow(control));
            temporaryData.DraggedItem = GetTemplatedRootElement(e.OriginalSource as FrameworkElement);
        }

        /// <summary>
        /// 指定された FrameworkElement に対するテンプレートのルート要素を取得します。
        /// </summary>
        /// <param name="element">FrameworkElement を指定します。</param>
        /// <returns>TemplatedParent を辿った先のルート要素を返します。</returns>
        private static FrameworkElement GetTemplatedRootElement(FrameworkElement element)
        {
            var parent = element.TemplatedParent as FrameworkElement;
            while (parent.TemplatedParent != null)
            {
                parent = parent.TemplatedParent as FrameworkElement;
            }
            return parent;
        }

        /// <summary>
        /// PreviewMouseMove イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (temporaryData != null)
            {
                var control = sender as FrameworkElement;
                var current = e.GetPosition(Window.GetWindow(control));
                if (temporaryData.CheckStartDragging(current))
                {
                    DragDrop.DoDragDrop(control, temporaryData.DraggedItem, DragDropEffects.Move);
                    // この先は Drop イベント処理後におこなわれる
                    temporaryData = null;
                }
            }
        }

        /// <summary>
        /// PreviewMouseLeftButtonUp イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            temporaryData = null;
        }

        /// <summary>
        /// PreviewDragEnter イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            temporaryData.IsDroppable = true;
        }

        /// <summary>
        /// PreviewDragLeave イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            temporaryData.IsDroppable = false;
        }

        /// <summary>
        /// PreviewDrop イベントハンドラ
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (temporaryData.IsDroppable)
            {
                var itemsControl = sender as ItemsControl;
                // 異なる ItemsControl 間でドロップ処理されないようにするために
                // 同一 ItemsControl 内にドラッグされたコンテナが存在することを確認する
                if (itemsControl.ItemContainerGenerator.IndexFromContainer(temporaryData.DraggedItem) >= 0)
                {
                    var targetContainer = GetTemplatedRootElement(e.OriginalSource as FrameworkElement);
                    var index = itemsControl.ItemContainerGenerator.IndexFromContainer(targetContainer);
                    if (index >= 0)
                    {
                        var callback = GetCallback(itemsControl);
                        callback(index);
                    }
                }
            }

            // 終了後は DragDrop.DoDragDrop() メソッド呼び出し元へ戻る
        }

        #endregion イベントハンドラ
    }
}
