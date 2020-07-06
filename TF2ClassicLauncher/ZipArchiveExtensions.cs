// Decompiled with JetBrains decompiler
// Type: ZipArchiveExtensions
// Assembly: TF2ClassicLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E766BA07-5012-4950-84E7-413534B128DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\sourcemods\tf2classic\TF2ClassicLauncher.exe

using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

public static class ZipArchiveExtensions
{
  private static int entryAmount;
  private static int entryExtractedAmount;
  private static double progressPercentage;

  public static int ExtractToDirectory(
    this ZipArchive archive,
    string destinationDirectoryName,
    bool overwrite,
    Button b,
    ProgressBar pBar)
  {
    if (!overwrite)
    {
      archive.ExtractToDirectory(destinationDirectoryName);
      return 0;
    }
    foreach (ZipArchiveEntry entry in archive.Entries)
      ++ZipArchiveExtensions.entryAmount;
    try
    {
      pBar.Maximum = ZipArchiveExtensions.entryAmount;
    }
    catch (Exception ex)
    {
    }
    foreach (ZipArchiveEntry entry in archive.Entries)
    {
      string str = Path.Combine(destinationDirectoryName, entry.FullName);
      if (entry.Name == "")
      {
        Directory.CreateDirectory(Path.GetDirectoryName(str));
        ++ZipArchiveExtensions.entryExtractedAmount;
        ZipArchiveExtensions.updateButtonAndBar(b, pBar);
      }
      else
      {
        if (!Directory.Exists(Path.GetDirectoryName(str)) && Path.GetDirectoryName(str) != "")
          Directory.CreateDirectory(Path.GetDirectoryName(str));
        try
        {
          entry.ExtractToFile(str, true);
          ++ZipArchiveExtensions.entryExtractedAmount;
        }
        catch (IOException ex)
        {
          int num = (int) MessageBox.Show("Another process is using \"" + str + "\".\nMake sure Team Fortress 2 Classic is closed while updating the game. There could possibly be other processes using the file.", "An error occurred while updating the game!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          return 1;
        }
        ZipArchiveExtensions.updateButtonAndBar(b, pBar);
      }
    }
    return 0;
  }

  private static void updateButtonAndBar(Button b, ProgressBar pBar)
  {
    b.Text = "Extracting (" + ZipArchiveExtensions.entryExtractedAmount.ToString() + "/" + ZipArchiveExtensions.entryAmount.ToString() + ")";
    pBar.Value = ZipArchiveExtensions.entryExtractedAmount;
  }

  public static bool ExtractToDirectory(
    this ZipArchive archive,
    string destinationDirectoryName,
    Action<int> progress)
  {
    foreach (ZipArchiveEntry entry in archive.Entries)
      ++ZipArchiveExtensions.entryAmount;
    foreach (ZipArchiveEntry entry in archive.Entries)
    {
      string str = Path.Combine(destinationDirectoryName, entry.FullName);
      if (entry.Name == "")
      {
        Directory.CreateDirectory(Path.GetDirectoryName(str));
        ++ZipArchiveExtensions.entryExtractedAmount;
        int num = (int) ((double) ZipArchiveExtensions.entryExtractedAmount / (double) ZipArchiveExtensions.entryAmount * 100.0);
        progress(num);
      }
      else
      {
        if (!Directory.Exists(Path.GetDirectoryName(str)) && Path.GetDirectoryName(str) != "")
          Directory.CreateDirectory(Path.GetDirectoryName(str));
        try
        {
          entry.ExtractToFile(str, true);
          ++ZipArchiveExtensions.entryExtractedAmount;
        }
        catch (IOException ex)
        {
          int num = (int) MessageBox.Show("Another process is using \"" + str + "\".\nMake sure Team Fortress 2 Classic is closed while updating the game. There could possibly be other processes using the file.", "An error occurred while updating the game!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          return false;
        }
        int num1 = (int) ((double) ZipArchiveExtensions.entryExtractedAmount / (double) ZipArchiveExtensions.entryAmount * 100.0);
        progress(num1);
      }
    }
    return true;
  }
}
