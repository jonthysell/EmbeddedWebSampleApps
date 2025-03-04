// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EmbeddedWebSampleApps.Common;

/// <summary>
/// Interaction logic for PerformanceControl.xaml
/// </summary>
public partial class PerformanceControl : UserControl
{
    public string ProcessName
    {
        get
        {
            return (string)GetValue(ProcessNameProperty);
        }
        set
        {
            SetValue(ProcessNameProperty, value);
        }
    }

    public static readonly DependencyProperty ProcessNameProperty =
      DependencyProperty.Register("ProcessName", typeof(string),
        typeof(PerformanceControl), new PropertyMetadata("", OnDependencyPropertyChanged));

    public int ProcessId
    {
        get
        {
            return (int)GetValue(ProcessIdProperty);
        }
        set
        {
            SetValue(ProcessIdProperty, value);
        }
    }

    public static readonly DependencyProperty ProcessIdProperty =
      DependencyProperty.Register("ProcessId", typeof(int),
        typeof(PerformanceControl), new PropertyMetadata(0, OnDependencyPropertyChanged));

    public bool AggregateChildrenProcesses
    {
        get
        {
            return (bool)GetValue(AggregateChildrenProcessesProperty);
        }
        set
        {
            SetValue(AggregateChildrenProcessesProperty, value);
        }
    }

    public static readonly DependencyProperty AggregateChildrenProcessesProperty =
      DependencyProperty.Register("AggregateChildrenProcesses", typeof(bool),
        typeof(PerformanceControl), new PropertyMetadata(true, OnDependencyPropertyChanged));

    private readonly List<PerformanceCounter> _cpuCounters = new List<PerformanceCounter>();
    private readonly List<PerformanceCounter> _ramCounters = new List<PerformanceCounter>();
    private DispatcherTimer? _timer = null;

    private float _cpu = 0.0f;
    private float _maxCpu = 0.0f;
    private float _ram = 0.0f;
    private float _maxRam = 0.0f;

    public PerformanceControl()
    {
        InitializeComponent();
        appName.DataContext = this;
        ResetCounters();
    }

    private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PerformanceControl pc)
        {
            pc.ResetCounters();
        }
    }

    private void ResetCounters()
    {
        if (_timer is not null)
        {
            _timer.Tick -= timer_Tick;
            _timer.Stop();
        }

        _cpuCounters.Clear();
        _ramCounters.Clear();

        string title = "Unknown Process";

        var process = ProcessId > 0 ? Process.GetProcessById(ProcessId)
            : (!string.IsNullOrWhiteSpace(ProcessName) ? Process.GetProcessesByName(ProcessName).FirstOrDefault()
            : Process.GetCurrentProcess());

        if (process is not null)
        {
            title = $"{process.ProcessName}({process.Id})";

            var processes = new List<Process>() { process };

            if (AggregateChildrenProcesses)
            {
                processes.AddRange(process.GetChildProcesses(true));
            }

            foreach (var p in processes)
            {
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
        }

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += timer_Tick;

        _cpu = 0.0f;
        _maxCpu = 0.0f;
        _ram = 0.0f;
        _maxRam = 0.0f;

        UpdateText(title);
    }

    private void StartCounters()
    {
        _timer?.Start();
    }

    private void timer_Tick(object? sender, EventArgs e)
    {
        var cpu = 0.0f;
        foreach (var cpuPC in _cpuCounters)
        {
            cpu += cpuPC.NextValue() / Environment.ProcessorCount;
        }

        cpu = Math.Min(100.0f, cpu);

        var ram = 0.0f;
        foreach (var ramPC in _ramCounters)
        {
            ram += ramPC.NextValue() / 1024 / 1024;
        }

        _cpu = cpu;
        _maxCpu = Math.Max(_maxCpu, cpu);
        _ram = ram;
        _maxRam = Math.Max(_maxRam, ram);

        UpdateText();
    }

    private void UpdateText(string? title = null)
    {
        if (title is not null)
        {
            appName.Text = title;
        }

        appCpuMetric.Text = $"{_cpu:0.00}%";
        appPeakCpuMetric.Text = $"{_maxCpu:0.00}%";

        appRamMetric.Text = $"{_ram:0.0} MB";
        appPeakRamMetric.Text = $"{_maxRam:0.0} MB";
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        ResetCounters();
        StartCounters();
    }
}

