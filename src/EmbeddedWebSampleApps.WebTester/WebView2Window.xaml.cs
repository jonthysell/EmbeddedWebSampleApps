// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;

using EmbeddedWebSampleApps.Common;

namespace EmbeddedWebSampleApps.WebTester;

/// <summary>
/// Interaction logic for WebView2Window.xaml
/// </summary>
public partial class WebView2Window : Window
{
    private readonly AppSettings _settings;

    public WebView2Window(AppSettings settings)
    {
        _settings = settings;

        InitializeComponent();
    }

    private async void WebHost_Loaded(object sender, RoutedEventArgs e)
    {
        await WebHost.EnsureCoreWebView2Async();
        if (_settings.LogWebConsole)
        {
            var helper = WebHost.CoreWebView2.GetDevToolsProtocolHelper();
            helper.Console.MessageAdded += WebHost_ConsoleMessageAdded;
            await helper.Console.EnableAsync();
        }
        WebHost.Source = _settings.StartingUri;
    }

    private void WebHost_ConsoleMessageAdded(object? sender, Microsoft.Web.WebView2.Core.DevToolsProtocolExtension.Console.MessageAddedEventArgs e)
    {
        Logger.LogLine($"console.log() [{e.Message.Level.ToLowerInvariant()}]", e.Message.Text);
    }
}