// Decompiled with JetBrains decompiler
// Type: WindowsFormsApplication1.Steam
// Assembly: TF2ClassicLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E766BA07-5012-4950-84E7-413534B128DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\sourcemods\tf2classic\TF2ClassicLauncher.exe

using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication1
{
  internal class Steam
  {
    private string installationFolder;

    public Steam()
    {
      this.installationFolder = this.fetchInstallationFolder();
    }

    public bool isSteamInstalled()
    {
      return this.installationFolder != null;
    }

    public string getInstallationFolder()
    {
      return this.installationFolder;
    }

    private string fetchInstallationFolder()
    {
      return (Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Valve\\Steam\\") ?? Registry.LocalMachine.OpenSubKey("SOFTWARE\\Valve\\Steam\\"))?.GetValue("InstallPath").ToString();
    }

    public string getSourcemodsFolder()
    {
      return this.installationFolder + "\\SteamApps\\sourcemods";
    }

    public List<string> getLibraryFolders()
    {
      List<string> stringList = new List<string>();
      stringList.Add(this.installationFolder);
      using (StreamReader streamReader = new StreamReader(this.installationFolder + "\\SteamApps\\libraryfolders.vdf"))
      {
        string input;
        while ((input = streamReader.ReadLine().Trim()) != "}")
        {
          if (new Regex("^\"[0-9]*\"( *\t*)*\".*\"$").IsMatch(input))
          {
            string path = Regex.Replace(input, "^\"[0-9]*\"( *\t*)*", "").Replace("\"", "").Replace("\\\\", "\\");
            if (Directory.Exists(path))
              stringList.Add(path);
          }
        }
      }
      return stringList;
    }

    public InstallationStatus getAppIdStatus(int appid)
    {
      return this.getAppIdStatus(appid, this.getLibraryFolders());
    }

    public InstallationStatus getAppIdStatus(
      int appid,
      List<string> libraryFolders)
    {
      foreach (string libraryFolder in libraryFolders)
      {
        string str = libraryFolder + "/SteamApps/appmanifest_" + appid.ToString() + ".acf";
        if (File.Exists(str))
        {
          string keyValue = this.getKeyValue("StateFlags", str);
          string installationDirectory = libraryFolder + "\\SteamApps\\common\\" + this.getKeyValue("installdir", str);
          return keyValue == "4" ? new InstallationStatus(true, false, installationDirectory) : new InstallationStatus(true, true, installationDirectory);
        }
      }
      return new InstallationStatus(false, false, (string) null);
    }

    private string getKeyValue(string key, string filename)
    {
      using (StreamReader streamReader = new StreamReader(filename))
      {
        string str1;
        while ((str1 = streamReader.ReadLine()) != null)
        {
          string input = str1.Trim();
          string pattern = "^\"" + key + "\"( *\t*)*";
          string str2 = "\".*\"$";
          if (new Regex(pattern + str2).IsMatch(input))
            return Regex.Replace(input, pattern, "").Replace("\"", "").Replace("\\\\", "\\");
        }
      }
      return (string) null;
    }
  }
}
