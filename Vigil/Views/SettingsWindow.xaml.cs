using System.Windows;
using System.Windows.Threading;
using Vigil.Config;
using Vigil.Hardware;

namespace Vigil.Views
{
  public partial class SettingsWindow : VigilWindow
  {
    public SettingsWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval){}

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();
      AllSensorData.Text = _services.HardwareMonitor.GetAllSensorData();
    }

    public override void Update(object? sender, EventArgs e){}

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

    private void SetReminderSizeAndPosition_Click(object sender, RoutedEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      var reminderWindowCurrentPos = _services.ReminderWindow.GetCurrentPosition();
      var reminderWindowCurrentSize = _services.ReminderWindow.GetCurrentSize();
      Console.WriteLine($"Setting reminder window size to {reminderWindowCurrentSize} and position to {reminderWindowCurrentPos}");
      _services.ConfigManager.UpdateConfig(cfg => {
        cfg.ReminderWindowSize = reminderWindowCurrentSize;
        cfg.ReminderWindowPos = reminderWindowCurrentPos;
      });
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
      System.Windows.Application.Current.Shutdown();
    }
  }
}
