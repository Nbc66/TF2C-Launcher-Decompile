// Decompiled with JetBrains decompiler
// Type: WindowsFormsApplication1.Tree
// Assembly: TF2ClassicLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E766BA07-5012-4950-84E7-413534B128DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\sourcemods\tf2classic\TF2ClassicLauncher.exe

using System;
using System.Collections.Generic;
using System.IO;

namespace WindowsFormsApplication1
{
  public class Tree
  {
    private List<Patch> tree;
    private string repo;

    public Tree(string repo)
    {
      this.repo = repo;
      this.refresh();
    }

    public void refresh()
    {
      this.tree = new List<Patch>();
      StreamReader streamReader = new StreamReader("tree.txt");
      int prevVersion = -1;
      string str;
      while ((str = streamReader.ReadLine()) != null)
      {
        string[] strArray = str.Split(';');
        int version = int.Parse(strArray[0]);
        this.tree.Add(new Patch(this, prevVersion, version, strArray[1], strArray[2], strArray[3]));
        prevVersion = version;
      }
    }

    public void printPatchInfo()
    {
      foreach (Patch patch in this.tree)
        Console.WriteLine(patch.getInfo());
    }

    public Patch getNextPatchFromVersion(int version)
    {
      int index = 0;
      while (index < this.tree.Count && this.tree[index].getPrevVersion() != version)
        ++index;
      return index < this.tree.Count ? this.tree[index] : (Patch) null;
    }

    public int getLatestVersionNumber()
    {
      Patch patch1 = this.tree[0];
      foreach (Patch patch2 in this.tree)
      {
        if (patch2.getVersion() > patch1.getVersion())
          patch1 = patch2;
      }
      return patch1.getVersion();
    }

    public int getEarliestVersionNumber()
    {
      Patch patch1 = this.tree[0];
      foreach (Patch patch2 in this.tree)
      {
        if (patch2.getVersion() < patch1.getVersion())
          patch1 = patch2;
      }
      return patch1.getVersion();
    }

    public List<Patch> getTree()
    {
      return this.tree;
    }

    public string getRepo()
    {
      return this.repo;
    }
  }
}
