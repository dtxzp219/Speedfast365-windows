﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

using Shadowsocks.Controller;
using Shadowsocks.Util;
using Shadowsocks.View;
using Shadowsocks.Extensions;
using Newtonsoft.Json.Linq;
using Shadowsocks.Model;
using Shadowsocks.Exceptions;

namespace Shadowsocks
{
    static class Program
    {
        private static ShadowsocksController _controller;
        // XXX: Don't change this name
        private static MenuViewController _viewController;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Check OS since we are using dual-mode socket
            if (!Utils.CheckFIPSAuthentication()) return;
            if (!Utils.IsWinVistaOrHigher())
            {
                MessageBox.Show(I18N.GetString("Unsupported operating system, use Windows Vista at least."),
                "Shadowsocks Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Utils.ReleaseMemory(true);
            using (Mutex mutex = new Mutex(false, $"Global\\Shadowsocks_{Application.StartupPath.GetHashCode()}"))
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                // handle UI exceptions
                Application.ThreadException += Application_ThreadException;
                // handle non-UI exceptions
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.ApplicationExit += Application_ApplicationExit;
                SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (!mutex.WaitOne(0, false))
                {
                    Process[] oldProcesses = Process.GetProcessesByName("Shadowsocks");
                    if (oldProcesses.Length > 0)
                    {
                        Process oldProcess = oldProcesses[0];
                    }
                    MessageBox.Show(I18N.GetString("Find Speedfast365 icon in your notify tray.")
                        + Environment.NewLine
                        + I18N.GetString("If you want to start multiple Speedfast365, make a copy in another directory."),
                        I18N.GetString("Speedfast365 is already running."));
                    return;
                }
                Directory.SetCurrentDirectory(Application.StartupPath);
#if DEBUG
                Logging.OpenLogFile();

                // truncate privoxy log file while debugging
                string privoxyLogFilename = Utils.GetTempPath("privoxy.log");
                if (File.Exists(privoxyLogFilename))
                    using (new FileStream(privoxyLogFilename, FileMode.Truncate)) { }
#else
                Logging.OpenLogFile();
#endif
                Test(); // TODO
                _controller = ShadowsocksController.one();
                _viewController = MenuViewController.one();
                NotificationRunner.one().Start();
                HotKeys.Init();
                _controller.Start();
                // 程序退出前把代理设置清除
                Application.ApplicationExit += new EventHandler((s, e) => {
                    SystemProxy.Disable();
                });
                Application.Run();
            }
        }
        private static void Test()
        {
        }

        private static int exited = 0;
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (Interlocked.Increment(ref exited) == 1)
            {
                string errMsg = e.ExceptionObject.ToString();
                Logging.Error(errMsg);
                MessageBox.Show(
                    $"{I18N.GetString("Unexpected error, Speedfast365 will exit. Please report to")} QQ技术售后:\"997651981\" {Environment.NewLine}{errMsg}",
                    "Shadowsocks non-UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (Interlocked.Increment(ref exited) == 1)
            {
                string errorMsg = $"Exception Detail: {Environment.NewLine}{e.Exception}";
                Logging.Error(errorMsg);
                MessageBox.Show(
                    $"{I18N.GetString("Unexpected error, Speedfast365 will exit. Please report to")} QQ技术售后:\"997651981\" {Environment.NewLine}{errorMsg}",
                    "Shadowsocks UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    Logging.Info("os wake up");
                    if (_controller != null)
                    {
                        System.Timers.Timer timer = new System.Timers.Timer(10 * 1000);
                        timer.Elapsed += Timer_Elapsed;
                        timer.AutoReset = false;
                        timer.Enabled = true;
                        timer.Start();
                    }
                    break;
                case PowerModes.Suspend:
                    if (_controller != null)
                    {
                        _controller.Stop();
                        Logging.Info("controller stopped");
                    }
                    Logging.Info("os suspend");
                    break;
            }
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (_controller != null)
                {
                    _controller.Start();
                    Logging.Info("controller started");
                }
            }
            catch (Exception ex)
            {
                Logging.LogUsefulException(ex);
            }
            finally
            {
                try
                {
                    System.Timers.Timer timer = (System.Timers.Timer)sender;
                    timer.Enabled = false;
                    timer.Stop();
                    timer.Dispose();
                }
                catch (Exception ex)
                {
                    Logging.LogUsefulException(ex);
                }
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            // detach static event handlers
            Application.ApplicationExit -= Application_ApplicationExit;
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            Application.ThreadException -= Application_ThreadException;
            HotKeys.Destroy();
            if (_controller != null)
            {
                _controller.Stop();
                _controller = null;
            }
        }
    }
}
