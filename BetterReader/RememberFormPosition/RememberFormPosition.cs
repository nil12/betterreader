using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Drawing;
using Microsoft.Win32;

namespace MartinTools
{

  [DesignerAttribute(typeof(RememberFormPositionDesigner), typeof(IDesigner))]
  public partial class RememberFormPosition : Component, ISupportInitialize
  {
    public RememberFormPosition()
    {
      _UseFormName = true;
      InitializeComponent();
    }

    public RememberFormPosition(IContainer container)
    {
      _UseFormName = true;
      container.Add(this);
      InitializeComponent();
    }

    [Description("The Form that we are retaining a position for")]
    [Category("Form Position")]
    [DefaultValue(null)]
    [Browsable(true)]
    public Form Form
    {
      get
      {
        return _form;
      }
      set
      {
        _form = value;
        if (_form != null)
        {
          _form.FormClosing += new FormClosingEventHandler(form_FormClosing);
        }

      }
    }


    [Description("If true we use the name of the form as the registry key. If false we use the provided custom name")]
    [Category("Form Position")]
    [DefaultValue(true)]
    [Browsable(true)]
    public bool UseFormName
    {
      get { return _UseFormName; }
      set { _UseFormName = value; }
    }

    [Description("The name to store the form information under if UseFormName is false")]
    [Category("Form Position")]
    [DefaultValue("")]
    [Browsable(true)]
    public string StorageName
    {
      get { return _StorageName; }
      set { _StorageName = value; }
    }

    void form_FormClosing(object sender, FormClosingEventArgs e)
    {
		SavePosition(sender);
    }

	public void SavePosition(object sender)
	{
		RememberFormPositionUtils.SaveFormPlacement(Application.UserAppDataRegistry,
													sender as Form,
													UseFormName ? (sender as Form).Name : StorageName);
	}


    #region ISupportInitialize Members

    public void BeginInit()
    {
    }

    public void EndInit()
    {
		RestorePosition();
    }

	private void RestorePosition()
	{
		_form.StartPosition = FormStartPosition.Manual;
		RememberFormPositionUtils.RestoreFormPlacement(Application.UserAppDataRegistry,
														  _form,
														  UseFormName ? _form.Name : StorageName);
	}

    #endregion

    #region Private Fields

    private Form _form;
    private bool _UseFormName;
    private string _StorageName;


    #endregion

  }

}
