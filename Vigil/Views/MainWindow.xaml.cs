using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Vigil.Config;
using Vigil.Hardware;
using System.Runtime.InteropServices;
using System.Windows.Interop;

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

      // Set the window position to the saved position
      ConfigData configCopy = _configManager.GetConfig();
      this.Loaded += (s, e) => SetPos(configCopy.MainWindowPosOne);

      // Initialize and start the timer to update UI every second
      _timer = new DispatcherTimer
      {
        Interval = TimeSpan.FromSeconds(1)
      };
      _timer.Tick += Draw;
      _timer.Start();
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
    int X, int Y, int cx, int cy, uint uFlags);

    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOSIZE = 0x0001;

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

    [DllImport("Shcore.dll")]
    private static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
      public int X;
      public int Y;

      public POINT(int x, int y)
      {
        X = x;
        Y = y;
      }
    }

    private void SetPos(System.Windows.Point wpfPos)
    {
      var source = (HwndSource)PresentationSource.FromVisual(this);
      if (source == null) return;

      // Convert WPF position to a POINT structure
      var targetPoint = new POINT((int)wpfPos.X, (int)wpfPos.Y);

      // Get the monitor for the target position
      IntPtr monitor = MonitorFromPoint(targetPoint, MONITOR_DEFAULTTONEAREST);

      // Get DPI for the monitor
      GetDpiForMonitor(monitor, 0, out uint dpiX, out uint dpiY);

      // Calculate scaling factor
      double scalingFactorX = dpiX / 96.0; // Default DPI is 96
      double scalingFactorY = dpiY / 96.0;

      // Adjust position to device pixels
      int deviceX = (int)(wpfPos.X * scalingFactorX);
      int deviceY = (int)(wpfPos.Y * scalingFactorY);

      Console.WriteLine($"Setting window position to {deviceX}, {deviceY}");
      SetWindowPos(source.Handle, IntPtr.Zero, deviceX, deviceY, 0, 0, SWP_NOZORDER | SWP_NOSIZE);
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
            SetPos(configCopy.MainWindowPosTwo);
            _isPosOneSet = false;

          }
          else
          {
            SetPos(configCopy.MainWindowPosOne);
            _isPosOneSet = true;
          }
        }
        else
        {
          this.DragMove();
          Console.WriteLine($"Window position: {Left}, {Top}");
          _configManager.UpdateConfig(cfg =>
          {
            cfg.MainWindowCurrentPos = new System.Windows.Point(Left, Top);
          });
        }
      }
    }
  }
}
