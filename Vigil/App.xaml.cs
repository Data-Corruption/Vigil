using System.Windows;
using Vigil.Hardware;
using Vigil.Config;
using Vigil.Views;
using Vigil.Reminder;

namespace Vigil
{
  public partial class App : Application
  {
    private ConfigManager<ConfigData>? _configManager;
    private HardwareMonitor? _hardwareMonitor;
    private ReminderManager? _reminderManager;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      _configManager = new ConfigManager<ConfigData>("config.json", new ConfigData());
      _hardwareMonitor = new HardwareMonitor();
      _reminderManager = new ReminderManager(8.0);
      // Create and show the main window
      MainWindow mainWindow = new MainWindow(_hardwareMonitor);
      mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      // Ensure monitoring is stopped when the application exits
      _hardwareMonitor?.Cleanup();
      _reminderManager?.Pause();
      base.OnExit(e);
    }
  }
}
