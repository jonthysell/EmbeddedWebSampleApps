// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using EmbeddedWebSampleApps.Common;

namespace EmbeddedWebSampleApps.WebTester;

public class WebTesterNativeAPI
{
    private readonly App _app;

    public WebTesterNativeAPI(App app)
    {
        _app = app;
    }

    public void ExitApp()
    {
        Logger.LogLine(nameof(WebTesterNativeAPI), nameof(ExitApp));
        _app.Dispatcher.Invoke(() =>
        {
            Logger.Close();
            _app.MainWindow.Close();
        });
    }
}
