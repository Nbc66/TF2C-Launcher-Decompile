// Decompiled with JetBrains decompiler
// Type: extractData
// Assembly: TF2ClassicLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E766BA07-5012-4950-84E7-413534B128DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\sourcemods\tf2classic\TF2ClassicLauncher.exe

using System.IO.Compression;

public struct extractData
{
  public ZipArchive archive;

  public extractData(ZipArchive archive)
  {
    this.archive = archive;
  }
}
