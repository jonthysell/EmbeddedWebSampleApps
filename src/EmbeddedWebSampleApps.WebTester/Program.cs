// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Windows;

using EmbeddedWebSampleApps.Common;

namespace EmbeddedWebSampleApps.WebTester;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        if (!TryParseArgs(args, out var settings))
        {
            PrintUsage();
            return -1;
        }

        var app = new App(settings);
        app.InitializeComponent();
        app.Run();

        return 0;
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Console.Error.WriteLine($"Unhandled exception: {e.ExceptionObject}");
    }

    private static bool TryParseArgs(string[] args, out AppSettings settings)
    {
        settings = new AppSettings();

        try
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--log-performance":
                        settings.LogPerformance = true;
                        break;
                    case "--log-web-console":
                        settings.LogWebConsole = true;
                        break;
                    case "--post-load-js":
                        settings.PostLoadJs = ++i < args.Length ? args[i] : throw new Exception($"Missing argument after {args[i-1].ToLower()}.");
                        break;
                    case "--starting-uri":
                        settings.StartingUri = ++i < args.Length && Uri.TryCreate(args[i], UriKind.Absolute, out var uri) ? uri : throw new Exception($"Unable to parse argument \"{args[i]}\".");
                        break;
                    case "--web-host":
                        settings.WebHost = ++i < args.Length && Enum.TryParse<WebHostType>(args[i], out var webHost) ? webHost : throw new Exception($"Unable to parse argument \"{args[i]}\".");
                        break;
                    case "--window-size":
                        settings.WindowSize = ++i < args.Length && SizeHelpers.TryParse(args[i], out var size) ? size : throw new Exception($"Unable to parse argument \"{args[i]}\".");
                        break;
                    case "-?":
                    case "--help":
                        return false;
                    default:
                        throw new Exception($"Unknown argument \"{args[i]}\".");
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return false;
        }

        return true;
    }

    private static void PrintUsage()
    {
        var defaultSettings = new AppSettings();
        Console.WriteLine($"{AppInfo.Name}.exe [options]");
        Console.WriteLine("Options:");
        Console.WriteLine();
        Console.WriteLine($"--log-performance     Enable performance logging (default: {defaultSettings.LogPerformance})");
        Console.WriteLine($"--log-web-console     Enable web console logging (default: {defaultSettings.LogWebConsole})");
        Console.WriteLine($"--post-load-js [file] Inject the given JS file after the Starting URI has loaded (default: {defaultSettings.PostLoadJs})");
        Console.WriteLine($"--starting-uri [uri]  Starting URI to load (default: {defaultSettings.StartingUri})");
        Console.WriteLine($"--web-host [WV2|CEF]  Control to host web content (default: {defaultSettings.WebHost})");
        Console.WriteLine($"--window-size [WxH]   Window size (default: {defaultSettings.WindowSize.Width:0}x{defaultSettings.WindowSize.Height:0})");
        Console.WriteLine($"-?, --help            Display this help");
    }
}