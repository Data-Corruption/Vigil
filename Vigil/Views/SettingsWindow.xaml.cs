using System.Windows;
using System.Windows.Controls;

namespace Vigil.Views
{
  public partial class SettingsWindow : VigilWindow
  {
    public SettingsWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval) { }

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();
      var (scanOutput, sensorData) = _services.HardwareMonitor.GetAllSensorData();
      AllSensorData.Text = scanOutput;
      CpuUsageSensor.ItemsSource = sensorData;
      CpuTempSensor.ItemsSource = sensorData;
      GpuUsageSensor.ItemsSource = sensorData;
      GpuTempSensor.ItemsSource = sensorData;
      VramTotalSensor.ItemsSource = sensorData;
      VramUsedSensor.ItemsSource = sensorData;
      RamUsageSensor.ItemsSource = sensorData;
      EthUsageSensor.ItemsSource = sensorData;
    }

    public override void Update(object? sender, EventArgs e) { }

    private void SetPosOne_Click(object sender, RoutedEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      var mainWindowCurrentPos = _services.MainWindow.GetCurrentPosition();
      Console.WriteLine($"Setting position one to {mainWindowCurrentPos}");
      _services.ConfigManager.UpdateConfig(cfg => { cfg.MainWindowPosOne = mainWindowCurrentPos; });
    }

    private void SetPosTwo_Click(object sender, RoutedEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      var mainWindowCurrentPos = _services.MainWindow.GetCurrentPosition();
      Console.WriteLine($"Setting position two to {mainWindowCurrentPos}");
      _services.ConfigManager.UpdateConfig(cfg => { cfg.MainWindowPosTwo = mainWindowCurrentPos; });
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      if (sender is System.Windows.Controls.ComboBox cb)
      {
        var selectedSensor = cb.SelectedItem as string;
        if (selectedSensor == null) { return; }
        Console.WriteLine($"Selected sensor: {selectedSensor}");
        switch (cb.Name)
        {
          case "CpuUsageSensor":
            _services.ConfigManager.UpdateConfig(cfg => { cfg.CpuUsageSensor = selectedSensor; });
            break;
          case "CpuTempSensor":
            _services.ConfigManager.UpdateConfig(cfg => { cfg.CpuTempSensor = selectedSensor; });
            break;
          case "GpuUsageSensor":
            _services.ConfigManager.UpdateConfig(cfg => { cfg.GpuUsageSensor = selectedSensor; });
            break;
          case "GpuTempSensor":
            _services.ConfigManager.UpdateConfig(cfg => { cfg.GpuTempSensor = selectedSensor; });
            break;
          case "VramTotalSensor":
            _services.ConfigManager.UpdateConfig(cfg => { cfg.GpuVramTotal = selectedSensor; });
            break;
          case "VramUsedSensor":
            _services.ConfigManager.UpdateConfig(cfg => { cfg.GpuVramUsed = selectedSensor; });
            break;
          case "RamUsageSensor":
            _services.ConfigManager.UpdateConfig(cfg => { cfg.RamUsageSensor = selectedSensor; });
            break;
          case "EthUsageSensor":
            _services.ConfigManager.UpdateConfig(cfg => { cfg.EthUsageSensor = selectedSensor; });
            break;
        }
      }
      _services.HardwareMonitor.UpdateIDs();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
      System.Windows.Application.Current.Shutdown();
    }

    private void UpdateReminder_Click(object sender, RoutedEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }

      // update interval and duration
      if (!int.TryParse(ReminderInterval.Text, out int intervalValue) || !int.TryParse(ReminderDuration.Text, out int durationValue))
      {
        Console.WriteLine("Error: could not parse interval or duration");
        return;
      }
      _services.ReminderManager.Update(TimeSpan.FromMinutes(intervalValue), TimeSpan.FromSeconds(durationValue));

      // update config
      _services.ConfigManager.UpdateConfig(cfg =>
      {
        cfg.ReminderInterval = TimeSpan.FromMinutes(intervalValue);
        cfg.ReminderDuration = TimeSpan.FromSeconds(durationValue);
      });
    }
  }
}
