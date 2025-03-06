﻿// Copyright (c) Microsoft Corporation.
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
    AppSettings Settings;

    public App(AppSettings settings) : base()
    {
        Settings = settings;
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
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

            CefRuntimeLoader.Initialize(settings);

            window = new CefWindow(Settings);
        }
        else if (Settings.WebHost == WebHostType.WV2)
        {
            window = new WebView2Window(Settings);
        }

        if (window is not null)
        {
            window.Title = $"{AppInfo.Name} [{ Settings.WebHost }] { Settings.StartingUri }";
            window.Width = Settings.WindowSize.Width;
            window.Height = Settings.WindowSize.Height;
            window.Show();
        }
    }
}