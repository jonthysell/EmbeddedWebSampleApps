// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace EmbeddedWebSampleApps.Common;


public sealed class Logger
{
    private static readonly Logger _logger = new Logger();

    private event EventHandler<LogEventArgs>? _logEvent;

    private Logger()
    {
        _logEvent += LogToConsole;
        _logEvent += LogToTrace;
    }

    public static event EventHandler<LogEventArgs>? LogEvent
    {
        add
        {
            _logger._logEvent += value;
        }
        remove
        {
            _logger._logEvent -= value;
        }
    }

    private static void LogToConsole(object? sender, LogEventArgs e)
    {
        Console.WriteLine(e.ToString());
    }

    private static void LogToTrace(object? sender, LogEventArgs e)
    {
        Trace.WriteLine(e.ToString());
    }

    public static void LogLine(string source, string message)
    {
        _logger._logEvent?.Invoke(_logger, new LogEventArgs(DateTime.Now, source, message));
    }

    public static void Close()
    {
        Console.Out.Flush();
        Trace.Flush();
    }
}

public class LogEventArgs: EventArgs
{
    public readonly DateTime Timestamp;
    public readonly string Source;
    public readonly string Message;

    public LogEventArgs(DateTime timestamp, string source, string message) : base()
    {
        Timestamp = timestamp;
        Source = source;
        Message = message;
    }

    public override string ToString()
    {
        return $"{Timestamp:O}\t{Source}\t{Message}";
    }
}