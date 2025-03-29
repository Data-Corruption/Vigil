using System.Timers;

namespace Vigil.Reminder
{
  public class ReminderManager
  {
    private VigilServices? _services;
    private readonly object _lock = new object();
    private System.Timers.Timer _timer;
    private double _interval;
    private int _duration;
    private DateTime _nextRun;
    private bool _isPaused;
    private bool _isDebug;

    public ReminderManager(VigilServices services, TimeSpan interval, TimeSpan duration)
    {
      _services = services;
      _interval = interval.TotalMilliseconds;
      _duration = (int)duration.TotalMilliseconds;
      _timer = new System.Timers.Timer(_interval);
      _timer.Elapsed += OnTimedEvent;
      _timer.AutoReset = true;
      _timer.Enabled = true;
      _nextRun = DateTime.Now.AddMilliseconds(_interval);
      _isPaused = false;
      _isDebug = false;
    }

    private void OnTimedEvent(Object? source, ElapsedEventArgs e)
    {
      lock (_lock)
      {
        if (_services == null) { Console.WriteLine("Error: ReminderManager services are null"); return; }
        _nextRun = DateTime.Now.AddMilliseconds(_interval);
        var flashDuration = TimeSpan.FromMilliseconds(_duration);
        _services.MainWindow?.Dispatcher.BeginInvoke(new Action(() =>
        {
          _services.MainWindow.FlashBackground(flashDuration);
        }));
      }
    }

    // Returns string in format "20m", "5m", or "30s" if under a minute
    public string GetTimeUntilNextRun()
    {
      lock (_lock)
      {
        var timeUntilNextRun = _nextRun - DateTime.Now;
        if (timeUntilNextRun.TotalMinutes < 1)
        {
          return timeUntilNextRun.ToString(@"s\s");
        }
        return timeUntilNextRun.ToString(@"m\m");
      }
    }

    public void Update(TimeSpan interval, TimeSpan duration)
    {
      lock (_lock)
      {
        _interval = interval.TotalMilliseconds;
        _duration = (int)duration.TotalMilliseconds;
        _timer.Interval = _interval;
        _nextRun = DateTime.Now.AddMilliseconds(_interval); // Reset the next run time
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