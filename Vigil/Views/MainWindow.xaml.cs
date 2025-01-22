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

      monitorUniformGrid.Height = config.GraphHeight;
      monitorUniformGrid.Width = double.NaN; // auto width

      // Setup cells
      var cells = FindElementsByTag(RootGrid, "Cell");
      foreach (var cell in cells)
      {
        // get the left and right items
        var left = FindElementsByTag(cell, "Left");
        var right = FindElementsByTag(cell, "Right");
        if (!left.Any() || !right.Any()) { Console.WriteLine("Error: left or right item is null"); continue; }
        // get the left and right columns
        var leftCol = left as ColumnDefinition;
        var rightCol = right as ColumnDefinition;
        if (leftCol == null || rightCol == null) { Console.WriteLine("Error: left or right column is null"); continue; }
        // set the width of the left and right columns
        leftCol.Width = new GridLength(1, GridUnitType.Star);
        rightCol.Width = new GridLength(3, GridUnitType.Star);
      }

      // Setup graphs
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
      var bitmap = new ScrollingBitmap(20, 20, 96, 96) { PixelColor = color };
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

    private IEnumerable<DependencyObject> FindElementsByTag(DependencyObject parent, object tag)
    {
      if (parent == null) yield break;
      int childCount = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i < childCount; i++)
      {
        var child = VisualTreeHelper.GetChild(parent, i);
        if (child is FrameworkElement fe && fe.Tag != null && fe.Tag.Equals(tag))
        {
          yield return child;
        }
        foreach (var descendant in FindElementsByTag(child, tag))
        {
          yield return descendant;
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

    private void Graph_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      // get right item as col and set width to 0*
    }

    private void Graph_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      // get right item as col and set width to 3*
    }
  }
}