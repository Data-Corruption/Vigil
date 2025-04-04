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
    private VigilServices? _vigilServices;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string localConfigPath = Path.Combine(localAppDataPath, "Vigil", "config.json");
      _vigilServices = new VigilServices(localConfigPath);
      // Create and show the main window
      _vigilServices?.MainWindow?.Show();
      CreateSystemTrayItems();
    }

    private void CreateSystemTrayItems()
    {
      if (_vigilServices == null) return;

      // Create notification icon
      _vigilServices.NotifyIcon.Text = "Vigil";
      // Load embedded icon
      Stream? iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Vigil.Resources.vigil.ico");
      if (iconStream != null)
      {
        using (iconStream)
        {
          _vigilServices.NotifyIcon.Icon = new Icon(iconStream);
        }
      }
      else
      {
        Console.WriteLine("Failed to load icon from embedded resources.");
        return;
      }
      // Create context menu
      _vigilServices.ContextMenu.Items.Add("Settings", null, (sender, e) =>
      {
        if (_vigilServices.SettingsWindow == null)
        {
          _vigilServices.SettingsWindow = new SettingsWindow(_vigilServices, TimeSpan.FromSeconds(1));
          _vigilServices.SettingsWindow.Closed += (sender, e) =>
          {
            _vigilServices.SettingsWindow = null;
          };
          _vigilServices.SettingsWindow.Show();
        }
        else
        {
          _vigilServices.SettingsWindow.Activate();
        }
      });
      _vigilServices.ContextMenu.Items.Add("Pause Reminder", null, (sender, e) =>
      {
        _vigilServices.ReminderManager.Pause();
      });
      _vigilServices.ContextMenu.Items.Add("Resume Reminder", null, (sender, e) =>
      {
        _vigilServices.ReminderManager.Resume();
      });
      _vigilServices.ContextMenu.Items.Add("Reset Windows", null, (sender, e) =>
      {
        // Update main window position to zero
        System.Windows.Point zp = new System.Windows.Point(0, 0);
        _vigilServices.ConfigManager.UpdateConfig(cfg =>
        {
          cfg.MainWindowPosOne = zp;
          cfg.MainWindowPosTwo = zp;
        });
        if (_vigilServices.MainWindow != null) { _vigilServices.MainWindow.SetPos(zp); }
      });
      _vigilServices.ContextMenu.Items.Add("Test Reminder", null, (sender, e) =>
      {
        _vigilServices.MainWindow?.FlashBackground(TimeSpan.FromSeconds(5));
      });
      _vigilServices.ContextMenu.Items.Add("Exit", null, (sender, e) =>
      {
        Current.Shutdown();
      });
      _vigilServices.NotifyIcon.ContextMenuStrip = _vigilServices.ContextMenu;
      _vigilServices.NotifyIcon.Visible = true;
    }

    protected override void OnExit(ExitEventArgs e)
    {
      if (_vigilServices != null)
      {
        // Ensure monitoring is stopped when the application exits
        _vigilServices.HardwareMonitor.Cleanup();
        _vigilServices.ReminderManager.Pause();
        _vigilServices.NotifyIcon.Dispose();
      }
      base.OnExit(e);
    }
  }

  public class VigilServices
  {
    public ConfigManager<ConfigData> ConfigManager { get; }
    public ReminderManager ReminderManager { get; }
    public HardwareMonitor HardwareMonitor { get; }
    public ContextMenuStrip ContextMenu { get; }
    public NotifyIcon NotifyIcon { get; }

    public MainWindow? MainWindow { get; }
    public SettingsWindow? SettingsWindow { get; set; }

    public VigilServices(string configPath)
    {
      ConfigManager = new ConfigManager<ConfigData>(configPath, new ConfigData());
      MainWindow = new MainWindow(this, TimeSpan.FromSeconds(1));
      HardwareMonitor = new HardwareMonitor(this);
      ReminderManager = new ReminderManager(this, ConfigManager.GetConfig().ReminderInterval, ConfigManager.GetConfig().ReminderDuration);
      ContextMenu = new ContextMenuStrip();
      NotifyIcon = new NotifyIcon();
    }
  }
}
