using System.Text;
using LibreHardwareMonitor.Hardware;

namespace Vigil.Hardware
{
  public class UsedSensor
  {
    public string? ID { get; set; }
    public ISensor? Sensor { get; set; } // reference to the sensor object
  }

  public class HardwareMonitor
  {
    public UsedSensor? CpuUsageSensor { get; private set; }
    public UsedSensor? CpuTempSensor { get; private set; }
    public UsedSensor? GpuUsageSensor { get; private set; }
    public UsedSensor? GpuTempSensor { get; private set; }
    public UsedSensor? VramTotalSensor { get; private set; }
    public UsedSensor? VramUsedSensor { get; private set; }
    public UsedSensor? RamUsageSensor { get; private set; }
    public UsedSensor? EthUsageSensor { get; private set; }

    private Computer? _computer;
    private VigilServices? _services;
    private HashSet<Identifier> _usedHardwareIDs = new HashSet<Identifier>();
    public readonly object Lock = new object();

    public HardwareMonitor(VigilServices services)
    {
      _services = services;
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
      UpdateIDs();
    }

    public void UpdateIDs()
    {
      lock (Lock)
      {
        if (_computer == null) { throw new InvalidOperationException("Hardware monitor not initialized."); }
        if (_services == null) { throw new InvalidOperationException("Services not initialized."); }

        _computer.Accept(new ScanVisitor());
        _usedHardwareIDs.Clear();

        var cfg = _services.ConfigManager.GetConfig();
        CpuUsageSensor = InitSensor(cfg.CpuUsageSensor);
        CpuTempSensor = InitSensor(cfg.CpuTempSensor);
        GpuUsageSensor = InitSensor(cfg.GpuUsageSensor);
        GpuTempSensor = InitSensor(cfg.GpuTempSensor);
        VramTotalSensor = InitSensor(cfg.GpuVramTotal);
        VramUsedSensor = InitSensor(cfg.GpuVramUsed);
        RamUsageSensor = InitSensor(cfg.RamUsageSensor);
        EthUsageSensor = InitSensor(cfg.EthUsageSensor);
      }
    }

    private UsedSensor InitSensor(string ID)
    {
      var usedSensor = new UsedSensor { ID = ID };
      foreach (IHardware hardware in _computer.Hardware)
      {
        foreach (ISensor sensor in hardware.Sensors)
        {
          if (sensor.Identifier.ToString() == usedSensor.ID)
          {
            _usedHardwareIDs.Add(hardware.Identifier);
            usedSensor.Sensor = sensor;
          }
        }
      }
      return usedSensor;
    }

    public void Update()
    {
      lock (Lock)
      {
        if (_computer == null) { throw new InvalidOperationException("Hardware monitor not initialized."); }
        if (_services == null) { throw new InvalidOperationException("Services not initialized."); }
        _computer.Accept(new UpdateVisitor(_usedHardwareIDs));
      }
    }

    public void Cleanup()
    {
      lock (Lock)
      {
        if (_computer == null)
        {
          throw new InvalidOperationException("Hardware monitor not initialized.");
        }
        _computer.Close();
        _computer = null;
      }
    }

    public (string, List<string>) GetAllSensorData()
    {
      lock (Lock)
      {
        if (_computer == null)
        {
          throw new InvalidOperationException("Hardware monitor not initialized.");
        }
        // Update all hardware data
        _computer.Accept(new ScanVisitor());
        // Get the latest data from the hardware
        StringBuilder output = new StringBuilder();
        var sensorData = new List<string>();
        foreach (IHardware hardware in _computer.Hardware)
        {
          output.AppendLine($"Hardware: {hardware.Name} ID: {hardware.Identifier}");
          Console.WriteLine($"Hardware: {hardware.Identifier}");
          foreach (ISensor sensor in hardware.Sensors)
          {
            output.AppendLine($"\tSensor: {sensor.Name}, value: {sensor.Value}");
            Console.WriteLine($"\tSensor: {sensor.Identifier}, value: {sensor.Value}");
            sensorData.Add(sensor.Identifier.ToString());
          }
        }
        return (output.ToString(), sensorData);
      }
    }

    public class UpdateVisitor : IVisitor
    {
      private HashSet<Identifier> _usedHardwareIDs;

      public UpdateVisitor(HashSet<Identifier> usedHardwareIDs)
      {
        _usedHardwareIDs = usedHardwareIDs;
      }

      public void VisitComputer(IComputer computer)
      {
        computer.Traverse(this);
      }
      public void VisitHardware(IHardware hardware)
      {
        if (_usedHardwareIDs.Contains(hardware.Identifier))
        {
          hardware.Update();
        }
      }
      public void VisitSensor(ISensor sensor) { }
      public void VisitParameter(IParameter parameter) { }
    }

    public class ScanVisitor : IVisitor
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
