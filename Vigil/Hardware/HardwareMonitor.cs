using System.Text;
using LibreHardwareMonitor.Hardware;

namespace Vigil.Hardware
{
  public class HardwareMonitor
  {
    private Computer? _computer;
    private readonly object _lock = new object();

    public HardwareMonitor() {
      // Initialize hardware interfaces, setup sensors, etc.
      Console.WriteLine("Hardware Monitor Initialized.");
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
    }

    public void Cleanup()
    {
      lock (_lock)
      {
        if (_computer == null)
        {
          throw new InvalidOperationException("Hardware monitor not initialized.");
        }
        _computer.Close();
        _computer = null;
      }
    }

    public string GetLatestData()
    {
      lock (_lock)
      {
        if (_computer == null)
        {
          throw new InvalidOperationException("Hardware monitor not initialized.");
        }
        // Update all hardware data
        _computer.Accept(new UpdateVisitor());
        // Get the latest data from the hardware
        StringBuilder output = new StringBuilder();
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
        return output.ToString();
      }
    }

    // TODO: Update this, give it a ref to desired hardware/sensors on construction.
    // Use that list to avoid traversing / calling Update on all hardware.
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
}
