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
/// Interaction logic for CefWindow.xaml
/// </summary>
public partial class CefWindow : Window
{
    public CefWindow(Uri startingUri)
    {
        InitializeComponent();
        cefBrowser.Address = startingUri.AbsoluteUri;
    }
}