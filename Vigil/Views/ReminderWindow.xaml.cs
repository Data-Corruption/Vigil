using Vigil.Config;

namespace Vigil.Views
{
  public partial class ReminderWindow : VigilWindow
  {
    public ReminderWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval){}

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();
      ConfigData configCopy = _services.ConfigManager.GetConfig();
      this.Loaded += (s, e) => {
        Width = configCopy.ReminderWindowSize.Width;
        Height = configCopy.ReminderWindowSize.Height;
        SetPos(configCopy.ReminderWindowPos);
      };
    }

    public override void Update(object? sender, EventArgs e){}

    public System.Windows.Point GetCurrentPosition() { return new System.Windows.Point(Left, Top); }
    public System.Windows.Size GetCurrentSize() { return new System.Windows.Size(Width, Height); }
  }
}