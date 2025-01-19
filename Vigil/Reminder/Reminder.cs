using System.Timers;
using Vigil.Views;

namespace Vigil.Reminder
{
  public class ReminderManager
  {
    private readonly object _lock = new object();
    private System.Timers.Timer _timer;
    private double _interval;
    private DateTime _nextRun;
    private bool _isPaused;
    private ReminderWindow? _reminderWindow;

    public ReminderManager(double intervalInSeconds)
    {
      _reminderWindow = new ReminderWindow();
      _interval = intervalInSeconds * 1000;
      _timer = new System.Timers.Timer(_interval);
      _timer.Elapsed += OnTimedEvent;
      _timer.AutoReset = true;
      _timer.Enabled = true;
      _nextRun = DateTime.Now.AddMilliseconds(_interval);
      _isPaused = false;
    }

    private async void OnTimedEvent(Object? source, ElapsedEventArgs e)
    {
      lock (_lock)
      {
        _nextRun = DateTime.Now.AddMilliseconds(_interval);
        _reminderWindow?.Dispatcher.Invoke(() => _reminderWindow.Show());
      }
      await Task.Delay(5000);
      lock (_lock)
      {
        _reminderWindow?.Dispatcher.Invoke(() => _reminderWindow.Hide());
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