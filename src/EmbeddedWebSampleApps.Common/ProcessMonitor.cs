// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace EmbeddedWebSampleApps.Common;

public class ProcessMonitor
{
    public string ProcessTitle => string.Join('\n', ProcessTitles);

    public readonly Process Process;

    public readonly bool IncludeChildren;

    public float CurrentCpuUsage { get; private set; } = 0.0f;

    public float MaxCpuUsage { get; private set; } = 0.0f;

    public float CurrentRamUsageMB { get; private set; } = 0.0f;

    public float MaxRamUsageMB { get; private set; } = 0.0f;

    public readonly IReadOnlyList<string> ProcessTitles;

    private readonly List<PerformanceCounter> _cpuCounters;
    private readonly List<PerformanceCounter> _ramCounters;

    private Task? _monitorTask = null;
    private CancellationTokenSource? _monitorCTS = null;

    public event EventHandler<ProcessMonitorEventArgs>? ProcessMonitorEvent;

    public ProcessMonitor(Process process, bool includeChildren = false)
    {
        Process = process;
        IncludeChildren = includeChildren;

        _cpuCounters = new List<PerformanceCounter>();
        _ramCounters = new List<PerformanceCounter>();

        var processTitles = new List<string>();

        var processes = new List<Process>() { process };

        if (includeChildren)
        {
            processes.AddRange(process.GetChildProcesses(true));
        }

        foreach (var p in processes)
        {
            processTitles.Add($"{p.ProcessName}({p.Id})");

            var cpuPC = p.GetPerformanceCounter("% Processor Time");
            if (cpuPC is not null)
            {
                _cpuCounters.Add(cpuPC);
            }

            var ramPC = p.GetPerformanceCounter("Working Set");
            if (ramPC is not null)
            {
                _ramCounters.Add(ramPC);
            }
        }

        ProcessTitles = processTitles;
    }

    public void Start()
    {
        if (_monitorCTS is not null || _monitorTask is not null)
        {
            throw new Exception("Unable to Start before Stop.");
        }

        _monitorCTS = new CancellationTokenSource();
        _monitorTask = Task.Run(async () => await MonitorAsync(_monitorCTS.Token));
    }

    public void Stop()
    {
        _monitorCTS?.Cancel();
        _monitorTask?.Wait();

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
            var cpu = 0.0f;
            foreach (var cpuPC in _cpuCounters)
            {
                cpu += cpuPC.NextValue();
            }

            cpu = Math.Min(100.0f, cpu / Environment.ProcessorCount);

            var ram = 0.0f;
            foreach (var ramPC in _ramCounters)
            {
                ram += ramPC.NextValue();
            }

            ram = ram / 1024 / 1024;

            CurrentCpuUsage = cpu;
            MaxCpuUsage = Math.Max(MaxCpuUsage, cpu);
            CurrentRamUsageMB = ram;
            MaxRamUsageMB = Math.Max(MaxRamUsageMB, ram);

            ProcessMonitorEvent?.Invoke(this, new ProcessMonitorEventArgs(cpu, ram));

            await Task.Delay(100, cancellationToken);
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