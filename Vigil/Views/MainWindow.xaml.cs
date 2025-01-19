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
    private bool _isPosOneSet = true;

    public MainWindow(ConfigManager<ConfigData> configManager, HardwareMonitor hardwareMonitor)
    {
      InitializeComponent();
      _configManager = configManager;
      _hardwareMonitor = hardwareMonitor;

      // Set the window position
      ConfigData configCopy = _configManager.GetConfig();
      SetWindowPos(configCopy.MainWindowPosOne);
      Console.WriteLine($"Window position: {this.Left}, {this.Top}");

      // Initialize and start the timer to update UI every second
      _timer = new DispatcherTimer
      {
        Interval = TimeSpan.FromSeconds(1)
      };
      _timer.Tick += Draw;
      _timer.Start();
    }

    private void SetWindowPos(System.Windows.Point point)
    {
      if (point.X != 0 || point.Y != 0)
      {
        // Check if the saved position is on any connected screen
        bool isOnScreen = Screen.AllScreens.Any(screen =>
            screen.WorkingArea.Contains(new System.Drawing.Point(
                (int)point.X,
                (int)point.Y
            ))
        );

        if (isOnScreen)
        {
          this.WindowStartupLocation = WindowStartupLocation.Manual;
          this.Left = point.X;
          this.Top = point.Y;
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

    private void Draw(object? sender, EventArgs e)
    {
      // OutputTextBox.Text = _hardwareMonitor.GetLatestData();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        // if shift or control is pressed, toggle the window position. Otherwise, drag the window
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
            Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
          var configCopy = _configManager.GetConfig();
          if (_isPosOneSet)
          {
            SetWindowPos(configCopy.MainWindowPosTwo);
            _isPosOneSet = false;

          }
          else
          {
            SetWindowPos(configCopy.MainWindowPosOne);
            _isPosOneSet = true;
          }
        }
        else
        {
          this.DragMove();
          _configManager.UpdateConfig(cfg =>
          {
            cfg.MainWindowCurrentPos = new System.Windows.Point(this.Left, this.Top);
          });
        }
      }
    }
  }
}
