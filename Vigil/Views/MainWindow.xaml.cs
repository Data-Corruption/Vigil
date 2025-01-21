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
    private int _graphSize = 18;

    public MainWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval) { }

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();

      var config = _services.ConfigManager.GetConfig();
      this.Loaded += (s, e) => SetPos(config.MainWindowPosOne);
      SetGraphSize(config.GraphScale);

      cpuUsageBitmap = SetupGraph(cpuUsage, config.CpuColor);
      cpuTempBitmap = SetupGraph(cpuTemp, config.CpuColor);
      gpuUsageBitmap = SetupGraph(gpuUsage, config.GpuColor);
      gpuTempBitmap = SetupGraph(gpuTemp, config.GpuColor);
      gpuVramUsageBitmap = SetupGraph(gpuVramUsage, config.GpuColor);
      ramUsageBitmap = SetupGraph(ramUsage, config.RamColor);
      ethUsageBitmap = SetupGraph(ethUsage, config.EthColor);
    }

    private ScrollingBitmap SetupGraph(System.Windows.Controls.Image img, System.Windows.Media.Color color)
    {
      var bitmap = new ScrollingBitmap(20, 20, 40, 96, 96) { PixelColor = color };
      img.Source = bitmap.Bitmap;
      if (img.Parent is Border border) { border.BorderBrush = new SolidColorBrush(color); }
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

    public void SetGraphSize(int size)
    {
      if (_graphSize == size) return;
      if (_graphSize > size) {
        // shrinking
        
      }
      else {
        // expanding
      }
      _graphSize = size;
      // Get the style you want to update:
      var style = (Style)Resources["SquareBorderStyle"];
      // Find all Border controls in the visual tree that use that style.
      var borders = FindAll<Border>(this).Where(b => b.Style == (Style)Resources["Cell"]);
      // Apply your changes to each matching Border
      foreach (var border in borders)
      {
        // Example: change the border’s brush
        border.BorderBrush = Brushes.Red;
        // or reapply the style if you modified it:
        // border.Style = null;
        // border.Style = style;
      }
    }

    private IEnumerable<T> FindAll<T>(DependencyObject parent) where T : DependencyObject
    {
      // Breadth-first search
      var queue = new Queue<DependencyObject>();
      queue.Enqueue(parent);

      while (queue.Count > 0)
      {
        var current = queue.Dequeue();
        if (current is T matched)
          yield return matched;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
          queue.Enqueue(VisualTreeHelper.GetChild(current, i));
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

    /*
    private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      // Expand canvas width and align right
      SquareBorder.Width = 40;
      BitmapDisplay.Width = 40;
      BitmapDisplay.Source = _scrollingBitmap.LargeBitmap;
    }

    private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      // Shrink canvas width and keep clipped content
      BitmapDisplay.Source = _scrollingBitmap.Bitmap;
      SquareBorder.Width = 20;
      BitmapDisplay.Width = 20;
    }
    */

    private void SquareBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (sender is Border border && border.Child is System.Windows.Controls.Image img)
      {
        border.Width = 40;
        img.Width = 40;
        switch (img.Name)
        {
          case "cpuUsage":
            img.Source = cpuUsageBitmap?.LargeBitmap;
            break;
          case "cpuTemp":
            img.Source = cpuTempBitmap?.LargeBitmap;
            break;
          case "gpuUsage":
            img.Source = gpuUsageBitmap?.LargeBitmap;
            break;
          case "gpuTemp":
            img.Source = gpuTempBitmap?.LargeBitmap;
            break;
          case "gpuVramUsage":
            img.Source = gpuVramUsageBitmap?.LargeBitmap;
            break;
          case "ramUsage":
            img.Source = ramUsageBitmap?.LargeBitmap;
            break;
          case "ethUsage":
            img.Source = ethUsageBitmap?.LargeBitmap;
            break;
        }
      }
    }

    private void SquareBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (sender is Border border && border.Child is System.Windows.Controls.Image img)
      {
        switch (img.Name)
        {
          case "cpuUsage":
            img.Source = cpuUsageBitmap?.Bitmap;
            break;
          case "cpuTemp":
            img.Source = cpuTempBitmap?.Bitmap;
            break;
          case "gpuUsage":
            img.Source = gpuUsageBitmap?.Bitmap;
            break;
          case "gpuTemp":
            img.Source = gpuTempBitmap?.Bitmap;
            break;
          case "gpuVramUsage":
            img.Source = gpuVramUsageBitmap?.Bitmap;
            break;
          case "ramUsage":
            img.Source = ramUsageBitmap?.Bitmap;
            break;
          case "ethUsage":
            img.Source = ethUsageBitmap?.Bitmap;
            break;
        }
        border.Width = 20;
        img.Width = 20;
      }
    }
  }
}