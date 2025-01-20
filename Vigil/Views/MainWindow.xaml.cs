using System.Windows.Input;
using Vigil.Config;
using Vigil.Hardware;

namespace Vigil.Views
{
  public partial class MainWindow : VigilWindow
  {
    private bool _isPosOneSet = true;

    public MainWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval){}

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();
      ConfigData configCopy = _services.ConfigManager.GetConfig();
      this.Loaded += (s, e) => SetPos(configCopy.MainWindowPosOne);
    }

    public override void Update(object? sender, EventArgs e)
    {
      if (!IsVisible) return;
      // OutputTextBox.Text = _hardwareMonitor.GetLatestData();
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
          _services.ConfigManager.UpdateConfig(cfg =>
          {
            cfg.MainWindowCurrentPos = new System.Windows.Point(Left, Top);
          });
        }
      }
    }
  }
}
