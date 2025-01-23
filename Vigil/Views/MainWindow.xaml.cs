using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Vigil.Config;
using Vigil.Hardware;

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
      this.Loaded += (s, e) => SetPos(config.MainWindowPosOne);

      // Setup graphs
      cpuUsageBitmap = SetupGraph(cpuUsage, config.CpuColor);
      cpuTempBitmap = SetupGraph(cpuTemp, config.CpuColor);
      gpuUsageBitmap = SetupGraph(gpuUsage, config.GpuColor);
      gpuTempBitmap = SetupGraph(gpuTemp, config.GpuColor);
      gpuVramUsageBitmap = SetupGraph(gpuVramUsage, config.GpuColor);
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
      cpuUsageBitmap?.Push(0.5);
      cpuTempBitmap?.Push(0.5);
      gpuUsageBitmap?.Push(0.5);
      gpuTempBitmap?.Push(0.5);
      gpuVramUsageBitmap?.Push(0.5);
      ramUsageBitmap?.Push(0.5);
      ethUsageBitmap?.Push(0.5);
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
  }
}