// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;

namespace EmbeddedWebSampleApps.Common;

public class ProcessMonitor
{
    public string ProcessTitle => $"{Process.ProcessName}({Process.Id})";

    public readonly Process Process;

    public readonly bool IncludeChildren;

    public float CurrentCpuUsage { get; private set; } = 0.0f;

    public float MaxCpuUsage { get; private set; } = 0.0f;

    public float CurrentRamUsageMB { get; private set; } = 0.0f;

    public float MaxRamUsageMB { get; private set; } = 0.0f;

    private readonly Dictionary<int, PerformanceCounterValue> _counters = new Dictionary<int, PerformanceCounterValue>();


    private Task? _monitorTask = null;
    private CancellationTokenSource? _monitorCTS = null;

    public event EventHandler<ProcessMonitorEventArgs>? ProcessMonitorEvent;

    public ProcessMonitor(Process process, bool includeChildren = false)
    {
        Process = process;
        IncludeChildren = includeChildren;

        RefreshPerformanceCounters();
    }

    private void RefreshPerformanceCounters()
    {
        var processes = Process.GetChildProcesses(true).Prepend(Process).ToList();
        int[] newPids = new int[processes.Count];

        for (int i = 0; i < processes.Count; i++)
        {
            if (_counters.TryGetValue(processes[i].Id, out var counterValue))
            {
                // Counter already exists
                var instanceName = processes[i].GetInstanceName();
                if (instanceName != counterValue.InstanceName)
                {
                    // But the instance was renamed, so re-create it
                    _counters[processes[i].Id] = new PerformanceCounterValue(instanceName);
                }
            }
            else
            {
                // Counter doesn't exist yet
                _counters[processes[i].Id] = new PerformanceCounterValue(processes[i].GetInstanceName());
            }
            newPids[i] = processes[i].Id;
        }

        // Clear out old processes that aren't in the new list
        foreach (var oldPid in _counters.Keys.Where(counterPid => !newPids.Contains(counterPid)))
        {
            _counters.Remove(oldPid);
        }
    }

    public void Start()
    {
        if (_monitorCTS is not null || _monitorTask is not null)
        {
            throw new Exception("Unable to Start before Stop.");
        }

        _monitorCTS = new CancellationTokenSource();
        _monitorTask = Task.Run(async () => await MonitorAsync(_monitorCTS.Token), _monitorCTS.Token);
    }

    public void Stop()
    {
        if (_monitorCTS is not null)
        {
            _monitorCTS.Cancel();
            try
            {
                _monitorTask?.Wait(_monitorCTS.Token);
            }
            catch (OperationCanceledException) { }
        }

        _monitorCTS = null;
        _monitorTask = null;
    }

    public void Reset()
    {
        if (_monitorCTS is not null || _monitorTask is not null)
        {
            throw new Exception("Unable to Reset before Stop.");
        }

        CurrentCpuUsage = 0.0f;
        MaxCpuUsage = 0.0f;
        CurrentRamUsageMB = 0.0f;
        MaxRamUsageMB = 0.0f;
    }

    private async Task MonitorAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                RefreshPerformanceCounters();

                var cpu = 0.0f;
                var ram = 0.0f;
                foreach (var pcv in _counters.Values)
                {
                    cpu += pcv.CpuCounter.NextValue();
                    ram += pcv.RamCounter.NextValue();
                }

                cpu = Math.Min(100.0f, cpu / Environment.ProcessorCount);
                ram = ram / 1024 / 1024;

                CurrentCpuUsage = cpu;
                MaxCpuUsage = Math.Max(MaxCpuUsage, cpu);
                CurrentRamUsageMB = ram;
                MaxRamUsageMB = Math.Max(MaxRamUsageMB, ram);
            }
            catch { }

            ProcessMonitorEvent?.Invoke(this, new ProcessMonitorEventArgs(CurrentCpuUsage, MaxRamUsageMB));

            await Task.Delay(500, cancellationToken);
        }
    }

    private class PerformanceCounterValue
    {
        public readonly string InstanceName;
        public readonly PerformanceCounter CpuCounter;
        public readonly PerformanceCounter RamCounter;

        public PerformanceCounterValue(string instanceName)
        {
            InstanceName = instanceName;
            CpuCounter = new PerformanceCounter("Process", "% Processor Time", instanceName);
            RamCounter = new PerformanceCounter("Process", "Working Set", instanceName);
        }


        public override string ToString()
        {
            return InstanceName;
        }
    }

}

public class ProcessMonitorEventArgs : EventArgs
{
    public readonly float CpuUsage;
    public readonly float RamUsageMB;

    public ProcessMonitorEventArgs(float cpuUsage, float ramUsageMB) : base()
    {
        CpuUsage = cpuUsage;
        RamUsageMB = ramUsageMB;
    }
}
