namespace Vigil.Config
{
  public class ConfigData
  {
    public System.Windows.Point MainWindowPosOne { get; set; } = new System.Windows.Point(0, 0);
    public System.Windows.Point MainWindowPosTwo { get; set; } = new System.Windows.Point(0, 0);
    public TimeSpan ReminderInterval { get; set; } = new TimeSpan(0, 0, 10, 0, 0); // 10 minute default
    public TimeSpan ReminderDuration { get; set; } = new TimeSpan(0, 0, 0, 6, 0); // 6 second default
    public System.Windows.Media.Color CpuColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 31, 144, 209);
    public System.Windows.Media.Color GpuColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 17, 187, 181);
    public System.Windows.Media.Color VramColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 164, 51, 217);
    public System.Windows.Media.Color RamColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 34, 177, 76);
    public System.Windows.Media.Color EthColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 212, 38, 116);
    public string CpuUsageSensor { get; set; } = "";
    public string CpuTempSensor { get; set; } = "";
    public string GpuUsageSensor { get; set; } = "";
    public string GpuTempSensor { get; set; } = "";
    public string GpuVramTotal { get; set; } = "";
    public string GpuVramUsed { get; set; } = "";
    public string RamUsageSensor { get; set; } = "";
    public string EthUsageSensor { get; set; } = "";
  }
}