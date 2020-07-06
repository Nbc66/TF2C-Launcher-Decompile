// Decompiled with JetBrains decompiler
// Type: WindowsFormsApplication1.Patch
// Assembly: TF2ClassicLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E766BA07-5012-4950-84E7-413534B128DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\sourcemods\tf2classic\TF2ClassicLauncher.exe

using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;

namespace WindowsFormsApplication1
{
  public class Patch
  {
    private int prevVersion;
    private int version;
    private string name;
    private string dateString;
    private string hash;
    private Tree tree;

    public Patch(
      Tree tree,
      int prevVersion,
      int version,
      string name,
      string dateString,
      string hash)
    {
      this.tree = tree;
      this.prevVersion = prevVersion;
      this.version = version;
      this.name = name;
      this.dateString = dateString;
      this.hash = hash;
    }

    public int getPrevVersion()
    {
      return this.prevVersion;
    }

    public int getVersion()
    {
      return this.version;
    }

    public string getName()
    {
      return this.name;
    }

    public string getPublishDate()
    {
      return this.dateString;
    }

    public string getHash()
    {
      return this.hash;
    }

    public string getFilename()
    {
      return "patch" + this.getPrevVersion().ToString() + "_" + this.getVersion().ToString() + ".zip";
    }

    public override string ToString()
    {
      return "[Patch " + this.getVersion().ToString() + "] " + this.getName() + (this.getPublishDate() != "" ? " (" + this.getPublishDate() + ")" : "");
    }

    public string getInfo()
    {
      return this.getPrevVersion().ToString() + " to " + this.getVersion().ToString() + " " + this.getName() + " " + this.getPublishDate() + " " + this.getFilename() + " " + this.getHash();
    }

    public void download(Action<int> progress)
    {
      using (FileStream fileStream = System.IO.File.Create(this.getFilename(), 16384))
      {
        HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(this.tree.getRepo() + this.getFilename())) as HttpWebRequest;
        httpWebRequest.KeepAlive = false;
        using (WebResponse response = httpWebRequest.GetResponse())
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            long contentLength = response.ContentLength;
            long num1 = 0;
            byte[] buffer = new byte[16384];
            int count;
            do
            {
              count = responseStream.Read(buffer, 0, 16384);
              fileStream.Write(buffer, 0, count);
              num1 += (long) count;
              int num2 = (int) ((double) num1 / (double) contentLength * 100.0);
              progress(num2);
            }
            while (count > 0);
          }
        }
      }
    }

    private void removeFiles(string installDir)
    {
      string path1 = installDir + "\\.remove";
      if (!System.IO.File.Exists(path1))
        return;
      using (StreamReader streamReader = new StreamReader(path1))
      {
        string str;
        while ((str = streamReader.ReadLine()) != null)
        {
          string path2 = installDir + "/" + str;
          if (System.IO.File.Exists(path2))
            System.IO.File.Delete(path2);
          else if (Directory.Exists(path2))
          {
            Directory.Delete(path2);
            while (Directory.Exists(path2))
              Thread.Sleep(50);
          }
        }
      }
      System.IO.File.Delete(path1);
    }

    public bool install(Action<int> progress, string installDir)
    {
      this.download(progress);
      using (ZipArchive archive = ZipFile.OpenRead(this.getFilename()))
      {
        if (!archive.ExtractToDirectory(installDir, progress))
          return false;
      }
      System.IO.File.Delete(this.getFilename());
      this.removeFiles(installDir);
      return true;
    }
  }
}
