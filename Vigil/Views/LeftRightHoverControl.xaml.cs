using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vigil.Views
{
  public partial class LeftRightHoverControl : System.Windows.Controls.UserControl
  {
    public LeftRightHoverControl()
    {
      InitializeComponent();
    }

    public event EventHandler<bool>? HoverStateChanged;

    public void RootControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      LeftColumn.Width = new GridLength(4, GridUnitType.Star);
      RightColumn.Width = new GridLength(0, GridUnitType.Star);
      HoverStateChanged?.Invoke(this, true);
    }

    public void RootControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      LeftColumn.Width = new GridLength(1, GridUnitType.Star);
      RightColumn.Width = new GridLength(3, GridUnitType.Star);
      HoverStateChanged?.Invoke(this, false);
    }
  }
}
