// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
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

using Xilium.CefGlue;
using Xilium.CefGlue.Common.Events;

using EmbeddedWebSampleApps.Common;

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
    }

    private void WebHost_BrowserInitialized()
    {
        Logger.LogLine(nameof(CefWindow), nameof(WebHost_BrowserInitialized));

        if (_app.Settings.LogWebConsole)
        {
            Logger.LogLine(nameof(CefWindow), $"Enable {nameof(_app.Settings.LogWebConsole)}");
            WebHost.ConsoleMessage += WebHost_ConsoleMessage;
        }

        WebHost.LoadEnd += WebHost_LoadEnd;

        Logger.LogLine(nameof(CefWindow), $"WebHost.Address = \"{_app.Settings.StartingUri.AbsoluteUri}\"");
        WebHost.Address = _app.Settings.StartingUri.AbsoluteUri;
    }

    private void WebHost_LoadEnd(object sender, LoadEndEventArgs e)
    {
        Logger.LogLine(nameof(CefWindow), nameof(WebHost_LoadEnd));

        WebHost.RegisterJavascriptObject(new WebTesterNativeAPI(_app), nameof(WebTesterNativeAPI));

        WebHost.ExecuteJavaScript($"const {nameof(WebTesterNativeAPI)} = window.{nameof(WebTesterNativeAPI)};");

        if (e.Frame.IsValid && !string.IsNullOrWhiteSpace(_app.Settings.PostLoadJs))
        {
            if (File.Exists(_app.Settings.PostLoadJs))
            {
                Logger.LogLine(nameof(CefWindow), $"Loading \"{_app.Settings.PostLoadJs}\"");
                var js = File.ReadAllText(_app.Settings.PostLoadJs);
                WebHost.ExecuteJavaScript(js);
            }
        }

        WebHost.LoadEnd -= WebHost_LoadEnd;
    }

    private void WebHost_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
    {
        Logger.LogLine("WebConsole", $"console.{CefLogSeverityToConsoleMethod(e.Level)}(): {e.Message}");
    }

    private static string CefLogSeverityToConsoleMethod(CefLogSeverity cls)
    {
        switch (cls)
        {
            case CefLogSeverity.Warning:
                return "warn";
            case CefLogSeverity.Error:
                return "error";
            case CefLogSeverity.Verbose:
                return "debug";
            case CefLogSeverity.Info:
            default:
                return "log";
        }
    }
}