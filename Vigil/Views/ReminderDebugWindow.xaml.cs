using System.Windows.Input;
using System.Windows;
using Vigil.Config;

namespace Vigil.Views
{
  public partial class ReminderDebugWindow : VigilWindow
  {
    public ReminderDebugWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval) { }

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();
      ConfigData configCopy = _services.ConfigManager.GetConfig();
      this.Loaded += (s, e) =>
      {
        Width = configCopy.ReminderWindowSize.Width;
        Height = configCopy.ReminderWindowSize.Height;
        SetPos(configCopy.ReminderWindowPos);
      };
    }

    public override void Update(object? sender, EventArgs e) { }

    public System.Windows.Point GetCurrentPosition() { return new System.Windows.Point(Left, Top); }
    public System.Windows.Size GetCurrentSize() { return new System.Windows.Size(Width, Height); }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      if (e.ChangedButton == MouseButton.Left)
      {
        this.DragMove();
        Console.WriteLine($"Window position: {Left}, {Top}");
      }
    }

    private void Update_Click(object sender, RoutedEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }

      // update position and size
      var currentPos = GetCurrentPosition();
      var currentSize = GetCurrentSize();
      Console.WriteLine($"Setting reminder window size to {currentSize} and position to {currentPos}");

      // update interval and duration
      if (!int.TryParse(ReminderInterval.Text, out int intervalValue) || !int.TryParse(ReminderDuration.Text, out int durationValue))
      {
        Console.WriteLine("Error: could not parse interval or duration");
        return;
      }
      _services.ReminderManager.Update(TimeSpan.FromMinutes(intervalValue), TimeSpan.FromSeconds(durationValue));

      // update real window
      _services.ReminderWindow?.SetSize(currentSize);
      _services.ReminderWindow?.SetPos(currentPos);

      // update config
      _services.ConfigManager.UpdateConfig(cfg => {
        cfg.ReminderWindowSize = currentSize;
        cfg.ReminderWindowPos = currentPos;
        cfg.ReminderInterval = TimeSpan.FromMinutes(intervalValue);
        cfg.ReminderDuration = TimeSpan.FromSeconds(durationValue);
      });
    }
  }
}