using System.Windows;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Vigil.Config;
using Vigil.Hardware;

namespace Vigil.Views
{
  public abstract partial class VigilWindow : Window
  {
    protected readonly VigilServices? _services;
    protected readonly DispatcherTimer _timer;

    public VigilWindow(VigilServices services, TimeSpan updateInterval)
    {
      _services = services;
      Init();
      _timer = new DispatcherTimer { Interval = updateInterval };
      _timer.Tick += Update;
      _timer.Start();
    }

    public abstract void Init();
    public abstract void Update(object? sender, EventArgs e);

    // SetPos interop nonsense

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
    int X, int Y, int cx, int cy, uint uFlags);

    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOSIZE = 0x0001;

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

    [DllImport("Shcore.dll")]
    private static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
      public int X;
      public int Y;

      public POINT(int x, int y)
      {
        X = x;
        Y = y;
      }
    }

    /// <summary>
    /// Input is in WPF units
    /// </summary>
    /// <param name="wpfPos"></param>
    protected void SetPos(System.Windows.Point wpfPos)
    {
      var source = (HwndSource)PresentationSource.FromVisual(this);
      if (source == null) return;

      // Convert WPF position to a POINT structure
      var targetPoint = new POINT((int)wpfPos.X, (int)wpfPos.Y);

      // Get the monitor for the target position
      IntPtr monitor = MonitorFromPoint(targetPoint, MONITOR_DEFAULTTONEAREST);

      // Get DPI for the monitor
      GetDpiForMonitor(monitor, 0, out uint dpiX, out uint dpiY);

      // Calculate scaling factor
      double scalingFactorX = dpiX / 96.0; // Default DPI is 96
      double scalingFactorY = dpiY / 96.0;

      // Adjust position to device pixels
      int deviceX = (int)(wpfPos.X * scalingFactorX);
      int deviceY = (int)(wpfPos.Y * scalingFactorY);

      Console.WriteLine($"Setting window position to {deviceX}, {deviceY}");
      SetWindowPos(source.Handle, IntPtr.Zero, deviceX, deviceY, 0, 0, SWP_NOZORDER | SWP_NOSIZE);
    }
  }
}