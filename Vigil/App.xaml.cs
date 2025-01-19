using System.Windows;
using System.IO;
using Vigil.Hardware;
using Vigil.Config;
using Vigil.Views;
using Vigil.Reminder;

namespace Vigil
{
  public partial class App : System.Windows.Application
  {
    private ConfigManager<ConfigData>? _configManager;
    private HardwareMonitor? _hardwareMonitor;
    private ReminderManager? _reminderManager;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string localConfigPath = Path.Combine(localAppDataPath, "Vigil", "config.json");
      _configManager = new ConfigManager<ConfigData>(localConfigPath, new ConfigData());
      _hardwareMonitor = new HardwareMonitor();
      _reminderManager = new ReminderManager(8.0);
      // Create and show the main window
      MainWindow mainWindow = new MainWindow(_configManager, _hardwareMonitor);
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
