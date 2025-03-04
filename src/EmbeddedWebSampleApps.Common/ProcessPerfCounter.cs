// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO;

namespace EmbeddedWebSampleApps.Common
{
    // Adapted from https://weblog.west-wind.com/posts/2014/Sep/27/Capturing-Performance-Counter-Data-for-a-Process-by-Process-Id
    public class ProcessPerfCounter
    {
        public static PerformanceCounter? GetPerfCounterForProcessId(int processId, string processCounterName = "% Processor Time")
        {
            var instance = GetInstanceNameForProcessId(processId);

            if (string.IsNullOrEmpty(instance))
            {
                return null;
            }

            return new PerformanceCounter("Process", processCounterName, instance);
        }

        public static string? GetInstanceNameForProcessId(int processId)
        {
            var process = Process.GetProcessById(processId);
            string processName = Path.GetFileNameWithoutExtension(process.ProcessName);

            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
            string[] instances = cat.GetInstanceNames()
                .Where(inst => inst.StartsWith(processName))
                .ToArray();

            foreach (string instance in instances)
            {
                using PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true);

                int val = (int)cnt.RawValue;
                if (val == processId)
                {
                    return instance;
                }
            }

            return null;
        }
    }
}
