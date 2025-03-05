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

namespace EmbeddedWebSampleApps.WebTester;

/// <summary>
/// Interaction logic for WebView2Window.xaml
/// </summary>
public partial class WebView2Window : Window
{
    public WebView2Window(Uri startingUri)
    {
        InitializeComponent();
        webView2.Source = startingUri;
    }
}