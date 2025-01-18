using System.Text;
using System.Windows;
using System.Windows.Threading;
using LibreHardwareMonitor.Hardware;

namespace Vigil
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _updateTimer;
        private Computer _computer;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the computer
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            _computer.Open();

            // Set up the timer for periodic updates
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _updateTimer.Tick += UpdateSystemInfo;
            _updateTimer.Start();
        }

        private void UpdateSystemInfo(object sender, EventArgs e)
        {
            StringBuilder output = new StringBuilder();

            // Update all hardware data
            _computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in _computer.Hardware)
            {
                output.AppendLine($"Hardware: {hardware.Name}");

                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    output.AppendLine($"\tSubhardware: {subhardware.Name}");

                    foreach (ISensor sensor in subhardware.Sensors)
                    {
                        output.AppendLine($"\t\tSensor: {sensor.Name}, value: {sensor.Value}");
                    }
                }

                foreach (ISensor sensor in hardware.Sensors)
                {
                    output.AppendLine($"\tSensor: {sensor.Name}, value: {sensor.Value}");
                }
            }

            // Display the updated text in the TextBox
            OutputTextBox.Text = output.ToString();
        }

        // Clean up resources when the window is closed
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _updateTimer.Stop();
            _computer.Close();
            _computer = null;
        }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }

        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
}
