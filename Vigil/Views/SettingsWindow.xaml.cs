using System.Windows;
using System.Windows.Threading;
using Vigil.Config;
using Vigil.Hardware;

namespace Vigil.Views
{
  public partial class SettingsWindow : VigilWindow
  {
    public SettingsWindow(VigilServices services, TimeSpan updateInterval) : base(services, updateInterval){}

    public override void Init()
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      InitializeComponent();
      AllSensorData.Text = _services.HardwareMonitor.GetAllSensorData();
    }

    public override void Update(object? sender, EventArgs e){}

    private void SetPosOne_Click(object sender, RoutedEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      Console.WriteLine($"Setting position one to {_services.ConfigManager.GetConfig().MainWindowCurrentPos}");
      _services.ConfigManager.UpdateConfig(cfg => { cfg.MainWindowPosOne = cfg.MainWindowCurrentPos; });
    }

    private void SetPosTwo_Click(object sender, RoutedEventArgs e)
    {
      if (_services == null) { Console.WriteLine("Error: main window services are null"); return; }
      Console.WriteLine($"Setting position two to {_services.ConfigManager.GetConfig().MainWindowCurrentPos}");
      _services.ConfigManager.UpdateConfig(cfg => { cfg.MainWindowPosTwo = cfg.MainWindowCurrentPos; });
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
      System.Windows.Application.Current.Shutdown();
    }
  }
}
