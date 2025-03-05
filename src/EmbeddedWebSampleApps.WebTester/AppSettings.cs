// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Windows;

namespace EmbeddedWebSampleApps.WebTester;

public enum WebHost
{
    ChromeEmbeddedFramework,
    WebView2,
}

public class AppSettings
{
    public WebHost WebHost = WebHost.WebView2;
    public Uri StartingUri = new Uri("https://microsoft.com/");
    public Size WindowSize = new Size(640, 480);
}
