using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Vigil.Hardware;

namespace Vigil.Views
{
    public partial class MainWindow : Window
    {
        private readonly HardwareMonitor _hardwareMonitor;
        private readonly DispatcherTimer _timer;
        private SettingsWindow? _settingsWindow;

        public MainWindow(HardwareMonitor hardwareMonitor)
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (_settingsWindow != null)
                {
                    return;
                }
                _settingsWindow = new SettingsWindow(_hardwareMonitor);
                _settingsWindow.Owner = this;
                _settingsWindow.Closed += (s, e) => _settingsWindow = null;
                _settingsWindow.Show();
            }
        }
    }
}
