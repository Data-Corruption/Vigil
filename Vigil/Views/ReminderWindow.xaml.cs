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
      this.Loaded += (s, e) => SetPos(configCopy.ReminderWindowPos);
    }

    public override void Update(object? sender, EventArgs e){}
  }
}