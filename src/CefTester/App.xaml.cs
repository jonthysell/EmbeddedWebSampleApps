// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

using Xilium.CefGlue;
using Xilium.CefGlue.Common;

namespace CefTester;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        SetupCef();
    }

    private static string CefCachePath = Path.GetTempPath();

    private static void SetupCef()
    {
        // Move the various chrome cache's out of the working directory
        CefCachePath = Path.Combine(Path.GetTempPath(), "CefGlue_" + Guid.NewGuid().ToString().Replace("-", null));

        AppDomain.CurrentDomain.ProcessExit += delegate { CleanupCef(); };

        var settings = new CefSettings()
        {
            RootCachePath = CefCachePath,
            CachePath = CefCachePath,

        };

        CefRuntimeLoader.Initialize(settings);
    }

    private static void CleanupCef()
    {
        CefRuntime.Shutdown(); // must shutdown cef to free cache files (so that cleanup is able to delete files)

        try
        {
            var dirInfo = new DirectoryInfo(CefCachePath);
            if (dirInfo.Exists)
            {
                dirInfo.Delete(true);
            }
        }
        catch (Exception)
        {
            // ignore
        }
    }
}

