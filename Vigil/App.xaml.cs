using System.Windows;
using Vigil.Hardware;
using Vigil.Views;

namespace Vigil
{
  public partial class App : Application
    {
        private HardwareMonitor? _hardwareMonitor;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize the hardware monitor
            _hardwareMonitor = new HardwareMonitor();
            _hardwareMonitor.Initialize();
            // Create and show the main window
            MainWindow mainWindow = new MainWindow(_hardwareMonitor);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Ensure monitoring is stopped when the application exits
            _hardwareMonitor?.Cleanup();
            base.OnExit(e);
        }
    }
}
