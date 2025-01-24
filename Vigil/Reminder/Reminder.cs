using System.Timers;
using System.Windows;
using Vigil.Views;

namespace Vigil.Reminder
{
  public class ReminderManager
  {
    private VigilServices? _services;
    private readonly object _lock = new object();
    private System.Timers.Timer _timer;
    private double _interval;
    private DateTime _nextRun;
    private bool _isPaused;
    private bool _isDebug;

    public ReminderManager(VigilServices services, TimeSpan interval)
    {
      _services = services;
      _interval = interval.TotalMilliseconds;
      _timer = new System.Timers.Timer(_interval);
      _timer.Elapsed += OnTimedEvent;
      _timer.AutoReset = true;
      _timer.Enabled = true;
      _nextRun = DateTime.Now.AddMilliseconds(_interval);
      _isPaused = false;
      _isDebug = false;
    }

    private async void OnTimedEvent(Object? source, ElapsedEventArgs e)
    {
      lock (_lock)
      {
        if (_services == null) { Console.WriteLine("Error: ReminderManager services are null"); return; }
        _nextRun = DateTime.Now.AddMilliseconds(_interval);
        _services.ReminderWindow?.Dispatcher.Invoke(() => _services.ReminderWindow.Show());
      }
      await Task.Delay(5000);
      lock (_lock)
      {
        if (_services == null) { Console.WriteLine("Error: ReminderManager services are null"); return; }
        if (!_isDebug)
        {
          _services.ReminderWindow?.Dispatcher.Invoke(() => _services.ReminderWindow.Hide());
        }
      }
    }

    public TimeSpan GetTimeUntilNextRun()
    {
      lock (_lock)
      {
        return _nextRun - DateTime.Now;
      }
    }

    public void SetInterval(double intervalInSeconds)
    {
      lock (_lock)
      {
        _interval = intervalInSeconds * 1000;
        _timer.Interval = _interval;
        _nextRun = DateTime.Now.AddMilliseconds(_interval);
      }
    }

    public void DebugOn()
    {
      lock (_lock)
      {
        if (_services == null) { Console.WriteLine("Error: ReminderManager services are null"); return; }
        Pause();
        _isDebug = true;
        _services.ReminderDebugWindow?.Dispatcher.Invoke(() => {
          _services.ReminderDebugWindow.Show();
          });
      }
    }

    public void DebugOff()
    {
      lock (_lock)
      {
        if (_services == null) { Console.WriteLine("Error: ReminderManager services are null"); return; }
        _isDebug = false;
        _services.ReminderDebugWindow?.Dispatcher.Invoke(() => {
          _services.ReminderDebugWindow.Hide();
          });
        Resume();
      }
    }

    public void Pause()
    {
      lock (_lock)
      {
        if (!_isPaused)
        {
          _timer.Stop();
          _isPaused = true;
        }
      }
    }

    public void Resume()
    {
      lock (_lock)
      {
        if (_isPaused)
        {
          _timer.Start();
          _nextRun = DateTime.Now.AddMilliseconds(_interval);
          _isPaused = false;
        }
      }
    }
  }
}