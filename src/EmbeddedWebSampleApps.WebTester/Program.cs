// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace EmbeddedWebSampleApps.WebTester;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var app = new App(new AppSettings());
        app.InitializeComponent();
        app.Run();

        return 0;
    }
}