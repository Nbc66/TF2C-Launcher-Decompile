// Decompiled with JetBrains decompiler
// Type: WindowsFormsApplication1.InstallationStatus
// Assembly: TF2ClassicLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E766BA07-5012-4950-84E7-413534B128DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\sourcemods\tf2classic\TF2ClassicLauncher.exe

namespace WindowsFormsApplication1
{
  internal class InstallationStatus
  {
    private bool installed;
    private bool updating;
    private string installationDirectory;

    public InstallationStatus(bool installed, bool updating, string installationDirectory)
    {
      this.installed = installed;
      this.updating = installed && updating;
      this.installationDirectory = installationDirectory;
    }

    public bool isInstalled()
    {
      return this.installed;
    }

    public bool isUpdating()
    {
      return this.updating;
    }

    public string getInstallationDirectory()
    {
      return this.installationDirectory;
    }
  }
}
