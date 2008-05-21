using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;

namespace MartinTools
{
  internal static class RememberFormPositionUtils
  {
 
    public static void SaveFormPlacement(RegistryKey key, Form form, string name)
    {
      if (form.WindowState == FormWindowState.Normal)
      {
        key.SetRectangleValue(name, form.DesktopBounds);
      }
      else
      {
        Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
        Rectangle restoreBounds = form.RestoreBounds;
        restoreBounds.X -= workingArea.X;
        restoreBounds.Y -= workingArea.Y;
        key.SetRectangleValue(name, restoreBounds);
      }
      key.SetWindowStateValue(name, form.WindowState);

    }

    public static void RestoreFormPlacement(RegistryKey key, Form form, string name)
    {
      Rectangle? rect = key.GetRectangleValue(name);

      if (rect.HasValue)
      {
        Rectangle formBounds = rect.Value;

        if (!form.IsMdiChild)
        {
          formBounds = EnsureFitsInDesktop(formBounds);
        }

        form.WindowState = FormWindowState.Normal;
        form.SetDesktopBounds(formBounds.X, formBounds.Y, formBounds.Width, formBounds.Height);
      }

      FormWindowState? state = key.GetWindowStateValue(name);

      if (state.HasValue)
      {
        form.WindowState = state.Value;
      }
    }

    private static Rectangle EnsureFitsInDesktop(Rectangle formBounds)
    {
      // okay, now we have a rectangle, we have to make sure it fits on the screen if that is where our form resides.

      // Let's test that each point is inside one of the screens. If not, let's just move it to the default screen instead.
      bool tl = false;
      bool tr = false;
      bool bl = false;
      bool br = false;

      foreach (Screen screen in Screen.AllScreens)
      {
        if (screen.WorkingArea.Contains(formBounds.Location + new Size(0, 0))) tl = true;
        if (screen.WorkingArea.Contains(formBounds.Location + new Size(formBounds.Width, 0))) tr = true;
        if (screen.WorkingArea.Contains(formBounds.Location + new Size(0, formBounds.Height))) bl = true;
        if (screen.WorkingArea.Contains(formBounds.Location + new Size(formBounds.Width, formBounds.Height))) br = true;
      }

      // If they have all missed a screen then let's move it to the current one.
      // If there is at least part of it visible, then that'll be okay.

      if (!(tl || tr || bl || br))
      {
        Rectangle maxBounds = Screen.PrimaryScreen.WorkingArea;
        if (formBounds.Width >= maxBounds.Width)
        {
          formBounds.Width = maxBounds.Width - 10;
        }
        if (formBounds.Height >= maxBounds.Height)
        {
          formBounds.Height = maxBounds.Height - 10;
        }

        formBounds.X = maxBounds.Width / 2 - formBounds.Width / 2;
        formBounds.Y = maxBounds.Height / 2 - formBounds.Height / 2;
      }
      return formBounds;
    }
  
  }
}
