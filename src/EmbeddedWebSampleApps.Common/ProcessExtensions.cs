// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace EmbeddedWebSampleApps.Common;

public static class ProcessExtensions
{
    // Adapted from https://stackoverflow.com/a/38614443
    public static IList<Process> GetChildProcesses(this Process process)
    {
        return new ManagementObjectSearcher(
                $"Select * From Win32_Process Where ParentProcessID={process.Id}")
            .Get()
            .Cast<ManagementObject>()
            .Select(mo =>
                Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])))
            .ToList();
    }

    public static PerformanceCounter? GetPerformanceCounter(this Process process, string processCounterName = "% Processor Time")
    {
        return new PerformanceCounter("Process", processCounterName, process.GetInstanceName());
    }

    // Adapted from https://weblog.west-wind.com/posts/2014/Sep/27/Capturing-Performance-Counter-Data-for-a-Process-by-Process-Id
    public static string GetInstanceName(this Process process)
    {
        string processName = Path.GetFileNameWithoutExtension(process.ProcessName);

        PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
        string[] instances = cat.GetInstanceNames()
            .Where(inst => inst.StartsWith(processName))
            .ToArray();

        foreach (string instance in instances)
        {
            using PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true);

            int val = (int)cnt.RawValue;
            if (val == process.Id)
            {
                return instance;
            }
        }

        throw new Exception($"Unable to find instance name for \"{processName}\".");
    }
}