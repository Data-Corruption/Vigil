using System.Windows;
using System.Windows.Threading;
using Vigil.Hardware;

namespace Vigil.Views
{
  public partial class SettingsWindow : Window
  {
    private readonly HardwareMonitor _hardwareMonitor;
    private readonly DispatcherTimer _timer;

    public SettingsWindow(HardwareMonitor hardwareMonitor)
    {
      InitializeComponent();
      _hardwareMonitor = hardwareMonitor;

      // Initialize and start the timer to update UI every second
      _timer = new DispatcherTimer
      {
        Interval = TimeSpan.FromSeconds(1)
      };
      _timer.Tick += Draw;
      _timer.Start();
    }

    private void Draw(object sender, EventArgs e)
    {
      OutputTextBox.Text = _hardwareMonitor.GetLatestData();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }
  }
}
