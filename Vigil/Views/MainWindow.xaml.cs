using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Vigil.Config;
using Vigil.Hardware;

namespace Vigil.Views
{
  public partial class MainWindow : VigilWindow
  {
    private bool _isPosOneSet = true;

    private ScrollingBitmap? _scrollingBitmap;

    public MainWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval) { }

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();
      _scrollingBitmap = new ScrollingBitmap(20, 20, 40, 96, 96);
      BitmapDisplay.Source = _scrollingBitmap.Bitmap;
      ConfigData configCopy = _services.ConfigManager.GetConfig();
      this.Loaded += (s, e) => SetPos(configCopy.MainWindowPosOne);
    }

    public override void Update(object? sender, EventArgs e)
    {
      if (!IsVisible) return;
      // OutputTextBox.Text = _hardwareMonitor.GetLatestData();
      _scrollingBitmap?.Push(0.5);
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
  }
}