// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Windows;

namespace EmbeddedWebSampleApps.WebTester;

public enum WebHostType
{
    CEF,
    WV2,
}

public class AppSettings
{
    public WebHostType WebHost = WebHostType.WV2;
    public Uri StartingUri = new Uri("https://microsoft.com/");
    public Size WindowSize = new Size(640, 480);
    public bool LogWebConsole = false;
    public bool LogPerformance = false;
    public string PostLoadJs = "";
}
