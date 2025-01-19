using System.Windows;
using System.Reflection;
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
    private ContextMenuStrip? _contextMenu;
    private NotifyIcon? _notifyIcon;

    private MainWindow? _mainWindow;
    private SettingsWindow? _settingsWindow;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string localConfigPath = Path.Combine(localAppDataPath, "Vigil", "config.json");
      _configManager = new ConfigManager<ConfigData>(localConfigPath, new ConfigData());
      _hardwareMonitor = new HardwareMonitor();
      _reminderManager = new ReminderManager(8.0);
      // Create and show the main window
      _mainWindow = new MainWindow(_configManager, _hardwareMonitor);
      _mainWindow.Show();
      CreateSystemTrayItems();
    }

    private void CreateSystemTrayItems()
    {
      // Create notification icon
      _notifyIcon = new NotifyIcon();
      _notifyIcon.Text = "Vigil";
      // Load embedded icon
      Stream? iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Vigil.Resources.vigil.ico");
      if (iconStream != null)
      {
        using (iconStream)
        {
          _notifyIcon.Icon = new Icon(iconStream);
        }
      }
      else
      {
        Console.WriteLine("Failed to load icon from embedded resources.");
        return;
      }
      // Create context menu
      _contextMenu = new ContextMenuStrip();
      _contextMenu.Items.Add("Settings", null, (sender, e) =>
      {
        if (_settingsWindow == null)
        {
          _settingsWindow = new SettingsWindow(_configManager, _hardwareMonitor);
          _settingsWindow.Closed += (sender, e) => _settingsWindow = null;
          _settingsWindow.Show();
        }
        else
        {
          _settingsWindow.Activate();
        }
      });
      _contextMenu.Items.Add("Pause Reminder", null, (sender, e) =>
      {
        _reminderManager?.Pause();
      });
      _contextMenu.Items.Add("Resume Reminder", null, (sender, e) =>
      {
        _reminderManager?.Resume();
      });
      _contextMenu.Items.Add("Exit", null, (sender, e) =>
      {
        Current.Shutdown();
      });
      _notifyIcon.ContextMenuStrip = _contextMenu;
      _notifyIcon.Visible = true;
    }

    protected override void OnExit(ExitEventArgs e)
    {
      // Ensure monitoring is stopped when the application exits
      _hardwareMonitor?.Cleanup();
      _reminderManager?.Pause();
      _notifyIcon?.Dispose();
      base.OnExit(e);
    }
  }
}
