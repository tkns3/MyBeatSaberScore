﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyBeatSaberScore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // UIスレッドの未処理例外で発生
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            // UIスレッド以外の未処理例外で発生
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            // それでも処理されない例外で発生
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            HandleException(exception);
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception.InnerException as Exception;
            HandleException(exception);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            HandleException(exception);
        }

        private void HandleException(Exception? e)
        {
            MessageBox.Show($"エラーが発生しました\n{e?.ToString()}");
            Environment.Exit(1);
        }
    }
}
