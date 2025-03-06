// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using EmbeddedWebSampleApps.Common;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Xilium.CefGlue.Common.Events;

namespace EmbeddedWebSampleApps.WebTester;

/// <summary>
/// Interaction logic for CefWindow.xaml
/// </summary>
public partial class CefWindow : Window
{
    private readonly App _app;

    public CefWindow(App app)
    {
        _app = app;

        InitializeComponent();
    }

    private void WebHost_Loaded(object sender, RoutedEventArgs e)
    {
        Logger.LogLine(nameof(CefWindow), nameof(WebHost_Loaded));

        if (_app.Settings.LogWebConsole)
        {
            Logger.LogLine(nameof(CefWindow), $"Enable {nameof(_app.Settings.LogWebConsole)}");
            WebHost.ConsoleMessage += WebHost_ConsoleMessage;
        }

        _app.TryEnablePerformanceLogging();

        Logger.LogLine(nameof(CefWindow), $"WebHost.Address = \"{_app.Settings.StartingUri.AbsoluteUri}\"");
        WebHost.Address = _app.Settings.StartingUri.AbsoluteUri;
    }

    private void WebHost_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
    {
        Logger.LogLine($"console.log() [{e.Level.ToString().ToLowerInvariant()}]", e.Message);
    }
}