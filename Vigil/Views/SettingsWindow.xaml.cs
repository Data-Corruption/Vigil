using System.Windows;
using System.Windows.Threading;
using Vigil.Config;
using Vigil.Hardware;

namespace Vigil.Views
{
  public partial class SettingsWindow : Window
  {
    private readonly ConfigManager<ConfigData> _configManager;
    private readonly HardwareMonitor _hardwareMonitor;
    private readonly DispatcherTimer _timer;

    public SettingsWindow(ConfigManager<ConfigData> configManager, HardwareMonitor hardwareMonitor)
    {
      InitializeComponent();
      _configManager = configManager;
      _hardwareMonitor = hardwareMonitor;
      // Initialize and start the timer to update UI every second
      _timer = new DispatcherTimer
      {
        Interval = TimeSpan.FromSeconds(1)
      };
      _timer.Tick += Draw;
      _timer.Start();
    }

    private void Draw(object? sender, EventArgs e)
    {
      if (!IsVisible)
      {
        return;
      }
      Console.WriteLine("Draw");
      OutputTextBox.Text = _hardwareMonitor.GetLatestData();
    }

    private void SetPosOne_Click(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("SetPosOne_Click");
      _configManager.UpdateConfig(cfg => { cfg.MainWindowPosOne = cfg.MainWindowCurrentPos; });
    }

    private void SetPosTwo_Click(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("SetPosTwo_Click");
      _configManager.UpdateConfig(cfg => { cfg.MainWindowPosTwo = cfg.MainWindowCurrentPos; });
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
      System.Windows.Application.Current.Shutdown();
    }
  }
}
