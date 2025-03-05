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

    private static bool TryParseArgs(string[] args, out AppSettings settings)
    {
        settings = new AppSettings();

        try
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-h":
                    case "--host":
                        settings.WebHost = ++i < args.Length && Enum.TryParse<WebHost>(args[i], out var webHost) ? webHost : throw new Exception($"Unable to parse argument \"{args[i]}\".");
                        break;
                    case "-u":
                    case "--uri":
                        settings.StartingUri = ++i < args.Length && Uri.TryCreate(args[i], UriKind.Absolute, out var uri) ? uri : throw new Exception($"Unable to parse argument \"{args[i]}\".");
                        break;
                    case "-s":
                    case "--size":
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
        Console.WriteLine("Usage:");
    }
}