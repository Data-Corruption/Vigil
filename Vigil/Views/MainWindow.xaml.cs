using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows;



namespace Vigil.Views
{
  public partial class MainWindow : VigilWindow
  {
    public ScrollingBitmap? cpuUsageBitmap;
    public ScrollingBitmap? cpuTempBitmap;
    public ScrollingBitmap? gpuUsageBitmap;
    public ScrollingBitmap? gpuTempBitmap;
    public ScrollingBitmap? gpuVramUsageBitmap;
    public ScrollingBitmap? ramUsageBitmap;
    public ScrollingBitmap? ethUsageBitmap;

    private bool _isPosOneSet = true;

    public MainWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval) { }

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();

      var config = _services.ConfigManager.GetConfig();
      this.Loaded += (s, e) => { SetPos(config.MainWindowPosOne); };

      // Setup graphs
      cpuUsageBitmap = SetupGraph(cpuUsage, config.CpuColor);
      cpuTempBitmap = SetupGraph(cpuTemp, config.CpuColor);
      gpuUsageBitmap = SetupGraph(gpuUsage, config.GpuColor);
      gpuTempBitmap = SetupGraph(gpuTemp, config.GpuColor);
      gpuVramUsageBitmap = SetupGraph(gpuVramUsage, config.VramColor);
      ramUsageBitmap = SetupGraph(ramUsage, config.RamColor);
      ethUsageBitmap = SetupGraph(ethUsage, config.EthColor);
    }

    private System.Windows.Controls.Image FindImageInControl(LeftRightHoverControl control)
    {
      return control.FindName("Graph") as System.Windows.Controls.Image;
    }

    private ScrollingBitmap SetupGraph(LeftRightHoverControl control, System.Windows.Media.Color color)
    {
      var bitmap = new ScrollingBitmap(15, 15, 60, 96, 96) { PixelColor = color };
      var img = FindImageInControl(control);
      if (img == null)
      {
        Console.WriteLine($"Failed to find Image control in {control.Name}");
        return bitmap;
      }

      control.HoverStateChanged += (s, isHovered) =>
      {
        img.Source = isHovered ? bitmap.WideBitmap : bitmap.Bitmap;
      };

      img.Source = bitmap.Bitmap;
      if (img.Parent is Border border)
      {
        border.BorderBrush = new SolidColorBrush(color);
      }
      return bitmap;
    }

    public override void Update(object? sender, EventArgs e)
    {
      if (!IsVisible) return;
      if (_services == null) { Console.WriteLine("Error: main window services are null"); System.Windows.Application.Current.Shutdown(); return; }

      // Update Reminder
      reminderCountdown.Text = _services.ReminderManager.GetTimeUntilNextRun();

      // Update Sensors
      _services.HardwareMonitor.Update();

      lock (_services.HardwareMonitor.Lock)
      {
        var cpuUsageValue = _services.HardwareMonitor.CpuUsageSensor?.Sensor?.Value ?? 0;
        cpuUsageBitmap?.Push(cpuUsageValue / 100);
        cpuUsage.GraphLabel.Text = $"  {(int)cpuUsageValue}%";
        var cpuTempValue = _services.HardwareMonitor.CpuTempSensor?.Sensor?.Value ?? 0;
        cpuTempBitmap?.Push(cpuTempValue / 100);
        cpuTemp.GraphLabel.Text = $"  {(int)cpuTempValue}°C";
        var gpuUsageValue = _services.HardwareMonitor.GpuUsageSensor?.Sensor?.Value ?? 0;
        gpuUsageBitmap?.Push(gpuUsageValue / 100);
        gpuUsage.GraphLabel.Text = $"  {(int)gpuUsageValue}%";
        var gpuTempValue = _services.HardwareMonitor.GpuTempSensor?.Sensor?.Value ?? 0;
        gpuTempBitmap?.Push(gpuTempValue / 100);
        gpuTemp.GraphLabel.Text = $"  {(int)gpuTempValue}°C";
        var ramUsageValue = _services.HardwareMonitor.RamUsageSensor?.Sensor?.Value ?? 0;
        ramUsageBitmap?.Push(ramUsageValue / 100);
        ramUsage.GraphLabel.Text = $"  {(int)ramUsageValue}%";
        var ethUsageValue = _services.HardwareMonitor.EthUsageSensor?.Sensor?.Value ?? 0;
        ethUsageBitmap?.Push(ethUsageValue / 100);
        ethUsage.GraphLabel.Text = $"  {(int)ethUsageValue}%";
        // GPU VRAM usage is a bit special, as it requires two sensors
        var vramUsageValue = (_services.HardwareMonitor.VramUsedSensor?.Sensor?.Value / _services.HardwareMonitor.VramTotalSensor?.Sensor?.Value) ?? 0;
        gpuVramUsageBitmap?.Push(vramUsageValue);
        gpuVramUsage.GraphLabel.Text = $"  {(int)(vramUsageValue * 100)}%";
      }

      if (_services.SettingsWindow == null)
      {
        if (_isPosOneSet)
        {
          SetPos(_services.ConfigManager.GetConfig().MainWindowPosOne);
        }
        else
        {
          SetPos(_services.ConfigManager.GetConfig().MainWindowPosTwo);
        }
      }
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      if (e.ChangedButton == MouseButton.Left)
      {
        // if shift or control is pressed, toggle the window position. Otherwise, drag the window
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
            Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
          var configCopy = _services.ConfigManager.GetConfig();
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
        }
      }
    }

    public System.Windows.Point GetCurrentPosition() { return new System.Windows.Point(Left, Top); }
    public System.Windows.Size GetCurrentSize() { return new System.Windows.Size(Width, Height); }

    private bool _isFlashing = false;
    public void FlashBackground(TimeSpan totalDuration)
    {
      if (_isFlashing)
      {
        Console.WriteLine("Already flashing background, ignoring request.");
        return;
      }
      _isFlashing = true;

      Console.WriteLine("Flashing background");

      var brush = new SolidColorBrush(Colors.Transparent);
      RootGrid.Background = brush;

      // Configure the animation to alternate between transparent and green.
      var animation = new ColorAnimation
      {
        From = Colors.Transparent,
        To = Colors.Green,
        AutoReverse = true,
        RepeatBehavior = RepeatBehavior.Forever,
        Duration = new Duration(TimeSpan.FromMilliseconds(500))
      };

      // Start the animation.
      brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

      // Use a DispatcherTimer to stop the animation after the specified total duration.
      var timer = new DispatcherTimer { Interval = totalDuration };
      timer.Tick += (sender, args) =>
      {
        timer.Stop();
        brush.BeginAnimation(SolidColorBrush.ColorProperty, null);
        brush.Color = Colors.Transparent;
        _isFlashing = false;
      };
      timer.Start();
    }
  }
}