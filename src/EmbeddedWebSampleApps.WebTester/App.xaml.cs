// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

using Xilium.CefGlue.Common;
using Xilium.CefGlue;
using EmbeddedWebSampleApps.Common;

namespace EmbeddedWebSampleApps.WebTester;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    internal readonly AppSettings Settings;

    private ProcessMonitor? _processMonitor = null;

    private StreamWriter? _logFile = null;

    public App(AppSettings settings) : base()
    {
        Settings = settings;
        if (!string.IsNullOrWhiteSpace(settings.LogFile))
        {
            _logFile = new StreamWriter(settings.LogFile);
            Logger.LogEvent += Application_WriteToLogFile_LogEvent;
        }
    }

    private void Application_WriteToLogFile_LogEvent(object? sender, LogEventArgs e)
    {
        _logFile?.WriteLine(e.ToString());
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Logger.LogLine(nameof(App), nameof(Application_Startup));

        TryEnablePerformanceLogging();

        Window? window = null;
        if (Settings.WebHost == WebHostType.CEF)
        {
            // Set CefGlue's cache to a folder next to the exe if possible, otherwise put in temp folder
            var cachePath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? Path.GetTempPath(), $"{AppInfo.Name}.exe.CefGlue");

            var settings = new CefSettings()
            {
                RootCachePath = cachePath,
                CachePath = cachePath,
            };

            Logger.LogLine(nameof(App), "CefRuntimeLoader.Initialize()");
            CefRuntimeLoader.Initialize(settings);

            window = new CefWindow(this);
        }
        else if (Settings.WebHost == WebHostType.WV2)
        {
            window = new WebView2Window(this);
        }

        if (window is not null)
        {
            window.Title = $"{AppInfo.Name} [{ Settings.WebHost }] { Settings.StartingUri }";
            window.Width = Settings.WindowSize.Width;
            window.Height = Settings.WindowSize.Height;
            Logger.LogLine(nameof(App), "window.Show()");
            window.Show();
        }
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        TryDisablePerformanceLogging();

        _logFile?.Flush();
        _logFile?.Close();
    }

    internal void TryEnablePerformanceLogging()
    {
        if (Settings.LogPerformance)
        {
            Logger.LogLine(nameof(App), $"Enable {nameof(Settings.LogPerformance)}");

            TryDisablePerformanceLogging();

            _processMonitor = new ProcessMonitor(Process.GetCurrentProcess(), true);
            _processMonitor.ProcessMonitorEvent += ProcessMonitor_ProcessMonitorEvent;
            _processMonitor.Start();
        }
    }

    private void ProcessMonitor_ProcessMonitorEvent(object? sender, ProcessMonitorEventArgs e)
    {
        Logger.LogLine("Performance", $"CPU: {e.CpuUsage:0.00}%, RAM: {e.RamUsageMB:0.0} MB");
    }

    internal void TryDisablePerformanceLogging()
    {
        if (_processMonitor is not null)
        {
            Logger.LogLine(nameof(App), $"Disable {nameof(Settings.LogPerformance)}");
            _processMonitor.Stop();
            _processMonitor = null;
        }
    }
}