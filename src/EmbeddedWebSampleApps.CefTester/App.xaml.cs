// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

using Xilium.CefGlue;
using Xilium.CefGlue.Common;

namespace EmbeddedWebSampleApps.CefTester;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        SetupCef();
    }

    private static void SetupCef()
    {
        // Set CefGlue's cache to a folder next to the exe if possible, otherwise put in temp folder
        var cachePath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? Path.GetTempPath(), "EmbeddedWebSampleApps.CefTester.CefGlue");

        var settings = new CefSettings()
        {
            RootCachePath = cachePath,
            CachePath = cachePath,
        };

        CefRuntimeLoader.Initialize(settings);
    }
}

