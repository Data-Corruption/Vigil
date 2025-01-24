using System.Windows.Media;

namespace Vigil.Config
{
  public class ConfigData
  {
    public System.Windows.Point MainWindowPosOne { get; set; } = new System.Windows.Point(0, 0);
    public System.Windows.Point MainWindowPosTwo { get; set; } = new System.Windows.Point(0, 0);
    public System.Windows.Point ReminderWindowPos { get; set; } = new System.Windows.Point(0, 0);
    public System.Windows.Size ReminderWindowSize { get; set; } = new System.Windows.Size(200, 200);
    public TimeSpan ReminderInterval { get; set; } = new TimeSpan(0, 0, 10, 0, 0); // 10 minute default
    public TimeSpan ReminderDuration { get; set; } = new TimeSpan(0, 0, 0, 6, 0); // 6 second default
    public System.Windows.Media.Color CpuColor { get; set; } = Colors.Blue;
    public System.Windows.Media.Color GpuColor { get; set; } = Colors.Cyan;
    public System.Windows.Media.Color RamColor { get; set; } = Colors.Green;
    public System.Windows.Media.Color EthColor { get; set; } = Colors.Salmon;
    public string[] CpuUsageSensorPath { get; set; } = [];
    public string[] CpuTempSensorPath { get; set; } = [];
    public string[] GpuUsageSensorPath { get; set; } = [];
    public string[] GpuTempSensorPath { get; set; } = [];
    public string[] GpuVramTotalPath { get; set; } = [];
    public string[] GpuVramUsedPath { get; set; } = [];
    public string[] RamUsageSensorPath { get; set; } = [];
    public string[] EthUsageSensorPath { get; set; } = [];
  }
}