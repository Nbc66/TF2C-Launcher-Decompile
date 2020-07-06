// Decompiled with JetBrains decompiler
// Type: WindowsFormsApplication1.Properties.Resources
// Assembly: TF2ClassicLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E766BA07-5012-4950-84E7-413534B128DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\sourcemods\tf2classic\TF2ClassicLauncher.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace WindowsFormsApplication1.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (WindowsFormsApplication1.Properties.Resources.resourceMan == null)
          WindowsFormsApplication1.Properties.Resources.resourceMan = new ResourceManager("WindowsFormsApplication1.Properties.Resources", typeof (WindowsFormsApplication1.Properties.Resources).Assembly);
        return WindowsFormsApplication1.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return WindowsFormsApplication1.Properties.Resources.resourceCulture;
      }
      set
      {
        WindowsFormsApplication1.Properties.Resources.resourceCulture = value;
      }
    }

    internal static Bitmap play
    {
      get
      {
        return (Bitmap) WindowsFormsApplication1.Properties.Resources.ResourceManager.GetObject(nameof (play), WindowsFormsApplication1.Properties.Resources.resourceCulture);
      }
    }
  }
}
