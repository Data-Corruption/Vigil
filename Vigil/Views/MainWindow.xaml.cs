using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Vigil.Config;
using Vigil.Hardware;

namespace Vigil.Views
{
  public partial class MainWindow : Window
  {
    private readonly ConfigManager<ConfigData> _configManager;
    private readonly HardwareMonitor _hardwareMonitor;
    private readonly DispatcherTimer _timer;
    private SettingsWindow? _settingsWindow;

    public MainWindow(ConfigManager<ConfigData> configManager, HardwareMonitor hardwareMonitor)
    {
      InitializeComponent();
      _configManager = configManager;
      _hardwareMonitor = hardwareMonitor;

      // Set the window position
      SetWindowPosition();

      Console.WriteLine($"Window position: {this.Left}, {this.Top}");

      // Initialize and start the timer to update UI every second
      _timer = new DispatcherTimer
      {
        Interval = TimeSpan.FromSeconds(1)
      };
      _timer.Tick += Draw;
      _timer.Start();
    }

    private void SetWindowPosition()
    {
      ConfigData configCopy = _configManager.GetConfig();
      if (configCopy.MainWindowPosition.X != 0 || configCopy.MainWindowPosition.Y != 0)
      {
        // Check if the saved position is on any connected screen
        bool isOnScreen = Screen.AllScreens.Any(screen =>
            screen.WorkingArea.Contains(new System.Drawing.Point(
                (int)configCopy.MainWindowPosition.X,
                (int)configCopy.MainWindowPosition.Y
            ))
        );

        if (isOnScreen)
        {
          this.WindowStartupLocation = WindowStartupLocation.Manual;
          this.Left = configCopy.MainWindowPosition.X;
          this.Top = configCopy.MainWindowPosition.Y;
        }
        else
        {
          // Default to center screen if saved position is not on any screen
          this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
      }
      else
      {
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      }
    }

    private void Draw(object sender, EventArgs e)
    {
      // OutputTextBox.Text = _hardwareMonitor.GetLatestData();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
      {
        if (_settingsWindow != null)
        {
          return;
        }
        _settingsWindow = new SettingsWindow(_hardwareMonitor);
        _settingsWindow.Owner = this;
        _settingsWindow.Closed += (s, e) => _settingsWindow = null;
        _settingsWindow.Show();
      }
      else
      {
        if (e.ChangedButton == MouseButton.Left)
        {
          this.DragMove();
          _configManager.UpdateConfig(cfg => {
            cfg.MainWindowPosition = new System.Windows.Point(this.Left, this.Top);
            });
        }
      }
    }
  }
}
