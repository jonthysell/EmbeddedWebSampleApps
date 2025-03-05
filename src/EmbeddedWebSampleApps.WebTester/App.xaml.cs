// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Configuration;
using System.Data;
using System.Windows;

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
        if (Settings.WebHost == WebHost.ChromeEmbeddedFramework)
        {
            window = new CefWindow(Settings.StartingUri);
        }
        else if (Settings.WebHost == WebHost.WebView2)
        {
            window = new WebView2Window(Settings.StartingUri);
        }

        if (window is not null)
        {
            window.Width = Settings.WindowSize.Width;
            window.Height = Settings.WindowSize.Height;
            window.Show();
        }
    }
}