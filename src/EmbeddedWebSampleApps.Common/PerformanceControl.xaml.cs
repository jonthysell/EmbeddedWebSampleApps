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

    private ProcessMonitor? _processMonitor = null;
    private DispatcherTimer? _timer = null;

    public PerformanceControl()
    {
        InitializeComponent();
        appName.DataContext = this;

        _timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += timer_Tick;
        _timer?.Start();
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
        if (_processMonitor is not null)
        {
            _processMonitor?.Stop();
            _processMonitor = null;
        }

        string title = "Unknown Process";

        var process = ProcessId > 0 ? Process.GetProcessById(ProcessId)
            : (!string.IsNullOrWhiteSpace(ProcessName) ? Process.GetProcessesByName(ProcessName).FirstOrDefault()
            : Process.GetCurrentProcess());

        if (process is not null)
        {
            _processMonitor = new ProcessMonitor(process, AggregateChildrenProcesses);
            title = _processMonitor.ProcessTitle;
        }

        UpdateText(title);
    }

    private void StartCounters()
    {
        _processMonitor?.Start();
    }

    private void timer_Tick(object? sender, EventArgs e)
    {
        UpdateText();
    }

    private void UpdateText(string? title = null)
    {
        if (title is not null)
        {
            appName.Text = title;
        }

        appCpuMetric.Text = $"{_processMonitor?.CurrentCpuUsage ?? 0:0.00}%";
        appPeakCpuMetric.Text = $"{_processMonitor?.MaxCpuUsage ?? 0:0.00}%";

        appRamMetric.Text = $"{_processMonitor?.CurrentRamUsageMB ?? 0:0.0} MB";
        appPeakRamMetric.Text = $"{_processMonitor?.MaxRamUsageMB ?? 0:0.0} MB";
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        ResetCounters();
        StartCounters();
    }
}

