namespace Vigil.Config
{
  public class ConfigData
  {
    public System.Windows.Point MainWindowPosition { get; set; } = new System.Windows.Point(0, 0);
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