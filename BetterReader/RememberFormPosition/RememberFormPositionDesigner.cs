using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MartinTools
{
  // Provides an example component designer.
  public class RememberFormPositionDesigner : System.ComponentModel.Design.ComponentDesigner
  {
    public RememberFormPositionDesigner()
    {
    }

    public override void InitializeNewComponent(System.Collections.IDictionary defaultValues)
    {
      base.InitializeNewComponent(defaultValues);
    }

    // This method provides an opportunity to perform processing when a designer is initialized.
    // The component parameter is the component that the designer is associated with.
    public override void Initialize(System.ComponentModel.IComponent component)
    {
      // Always call the base Initialize method in an override of this method.
      base.Initialize(component);
      Form f = this.ParentComponent as Form;

      if (f != null)
      {
        RememberFormPosition saver = this.Component as RememberFormPosition;
        if (saver != null)
        {
          saver.Form = f;
        }
      }
    }

    // This method is invoked when the associated component is double-clicked.
    public override void DoDefaultAction()
    {
    }

    // This method provides designer verbs.
    public override System.ComponentModel.Design.DesignerVerbCollection Verbs
    {
      get
      {
        //return new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("Example Designer Verb Command", new EventHandler(this.onVerb)) });
        return null;
      }
    }

    // Event handling method for the example designer verb
    private void onVerb(object sender, EventArgs e)
    {
    }
  }
}
