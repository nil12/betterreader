using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;

namespace MartinTools
{
  internal static class RegistryUtils
  {
    public static Rectangle? GetRectangleValue(this RegistryKey key, string name)
    {
      try
      {
        object top, left, bottom, right;
        Rectangle result = new Rectangle();
        left = key.GetValue(string.Format("{0}_Left", name));
        top = key.GetValue(string.Format("{0}_Top", name));
        right = key.GetValue(string.Format("{0}_Right", name));
        bottom = key.GetValue(string.Format("{0}_Bottom", name));
        if (left != null && right != null && top != null && bottom != null)
        {
          result.X = Convert.ToInt32(left);
          result.Y = Convert.ToInt32(top);
          result.Width = Convert.ToInt32(right) - result.X;
          result.Height = Convert.ToInt32(bottom) - result.Y;
          return result;
        }
        else
        {
          return null;
        }
      }
      catch (FormatException)
      {
        return null;
      }
    }
    public static void SetRectangleValue(this RegistryKey key, string name, Rectangle rect)
    {
      key.SetValue(string.Format("{0}_Left", name), rect.Left.ToString());
      key.SetValue(string.Format("{0}_Top", name), rect.Top.ToString());
      key.SetValue(string.Format("{0}_Right", name), rect.Right.ToString());
      key.SetValue(string.Format("{0}_Bottom", name), rect.Bottom.ToString());
    }

    public static void SetWindowStateValue(this RegistryKey key, string name, FormWindowState state)
    {
      key.SetValue(string.Format("{0}_State", name), state.ToString());
    }

    public static FormWindowState? GetWindowStateValue(this RegistryKey key, string name)
    {
      object state = key.GetValue(string.Format("{0}_State", name));

      if (state != null)
      {
        try
        {
          return (FormWindowState)Enum.Parse(typeof(FormWindowState), state.ToString());
        }
        catch (ArgumentException)
        {
          return null;
        }
      }
      else
      {
        return null;
      }
    }
  }

}
