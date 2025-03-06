// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EmbeddedWebSampleApps.Common;

public static class AppInfo
{
    public static Assembly Assembly => _assembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    private static Assembly? _assembly = null;

    public static string Name => _name ??= Assembly.GetName().Name ?? nameof(Name);
    private static string? _name = null;

    public static string Version
    {
        get
        {
            if (_version is null)
            {
                Version? vers = Assembly.GetName().Version;
                _version = vers is null ? "0.0.0" : $"{vers.Major}.{vers.Minor}.{vers.Build}";
            }
            return _version;
        }
    }

    private static string? _version = null;

    public static string Product => _product ??= Assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? nameof(Product);
    private static string? _product = null;

    public static string Copyright => _copyright ??= Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? nameof(Copyright);
    private static string? _copyright = null;

    public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public static readonly bool IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public static string EntryAssemblyPath => _entryAssemblyPath ??= Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? Environment.CurrentDirectory;
    private static string? _entryAssemblyPath = null;

}