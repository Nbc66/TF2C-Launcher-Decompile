// Decompiled with JetBrains decompiler
// Type: WindowsFormsApplication1.Form1
// Assembly: TF2ClassicLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E766BA07-5012-4950-84E7-413534B128DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\sourcemods\tf2classic\TF2ClassicLauncher.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
  public class Form1 : Form
  {
    private string executableFilename = Path.GetFileName(Assembly.GetEntryAssembly().Location);
    private string currentPatchFile = "";
    private int ItemMargin = 5;
    private int currentVersionNumber;
    private const string serverURL = "http://steevens.net/tf2c-public/";
    private Steam steam;
    private Tree tree;
    private IContainer components;
    private PictureBox pictureBox1;
    private OpenFileDialog fileBrowseDialog;
    private FolderBrowserDialog folderBrowseDialog;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel statusPanel;
    private ImageList imageList1;
    private TabPage configTab;
    private GroupBox groupBox1;
    private Button button1;
    private Label label2;
    private Label label3;
    private Button sdkBrowseButton;
    private RichTextBox sdkPathBox;
    private RichTextBox paramBox;
    private TabPage tabPage1;
    private Label label4;
    private ListBox listBox1;
    private ProgressBar updateProgressBar;
    private Button updateButton;
    private Button launchButton;
    private TabControl tabControl1;

    public Form1()
    {
      this.InitializeComponent();
      string url = "http://steevens.net/tf2c-public/header.png?random=" + new Random().Next().ToString();
      if (Form1.CheckForUrl(url))
        this.pictureBox1.ImageLocation = url;
      this.steam = new Steam();
      if (System.IO.File.Exists(this.executableFilename + ".old"))
        System.IO.File.Delete(this.executableFilename + ".old");
      string path = this.getGamePath() + "\\.remove";
      if (System.IO.File.Exists(path))
        System.IO.File.Delete(path);
      this.tabControl1.Selecting += new TabControlCancelEventHandler(this.tabControl1_Selecting);
      try
      {
        StreamReader streamReader = new StreamReader(this.getGamePath() + "\\config.txt");
        this.sdkPathBox.Text = streamReader.ReadLine();
        this.paramBox.Text = streamReader.ReadLine();
        streamReader.Close();
      }
      catch (Exception ex)
      {
        this.detectSDK(true);
      }
      this.downloadTreeFile();
    }

    public static bool CheckForUrl(string url)
    {
      try
      {
        using (WebClient webClient = new WebClient())
        {
          using (webClient.OpenRead(url))
            return true;
        }
      }
      catch
      {
        return false;
      }
    }

    public void downloadFile(WebClient client, string url, string filename, bool noCaching = false)
    {
      string uriString = url + filename;
      if (noCaching)
      {
        Random random = new Random();
        uriString = uriString + "?random=" + random.Next().ToString();
      }
      Uri address = new Uri(uriString);
      client.DownloadFileAsync(address, filename);
    }

    private void downloadTreeFile()
    {
      try
      {
        WebClient client = new WebClient();
        client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
        client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.downloadFileCompletedTree);
        this.downloadFile(client, "http://steevens.net/tf2c-public/", "tree.txt", true);
        client.Dispose();
      }
      catch (Exception ex)
      {
        this.statusPanel.Text = "problem getting tree";
      }
    }

    public void DownloadFile(Uri url, string outputFilePath)
    {
      using (FileStream fileStream = System.IO.File.Create(outputFilePath, 16384))
      {
        using (WebResponse response = WebRequest.Create(url).GetResponse())
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            byte[] buffer = new byte[16384];
            int count;
            do
            {
              count = responseStream.Read(buffer, 0, 16384);
              fileStream.Write(buffer, 0, count);
            }
            while (count > 0);
          }
        }
      }
    }

    private void downloadFileCompletedTree(object sender, AsyncCompletedEventArgs e)
    {
      this.tree = new Tree("http://steevens.net/tf2c-public/");
      if (this.fetchLocalVersionNumber() && this.checkForUpdates())
      {
        int num = this.getLatestTreeVersionString(1) == "" ? 1 : 0;
        this.statusPanel.Text = "A new patch is available for download!";
        this.updateButton.Enabled = true;
        this.updateButton.Text = "Update available";
      }
      else
        this.statusPanel.Text = "Your build appears to be up to date";
      this.updateListBox();
    }

    private bool checkForUpdates()
    {
      return this.getLatestTreeVersion() > this.getLocalVersionNumber();
    }

    private string getLatestTreeVersionString(int field = 0)
    {
      StreamReader streamReader = new StreamReader("tree.txt");
      string[] strArray = new string[1]{ "0" };
      string str;
      while ((str = streamReader.ReadLine()) != null)
        strArray = str.Split(';');
      streamReader.Close();
      return field < strArray.Length ? strArray[field] : "";
    }

    private int getLatestTreeVersion()
    {
      return int.Parse(this.getLatestTreeVersionString(0));
    }

    private int getNextTreeVersion()
    {
      List<int> intList = new List<int>();
      StreamReader streamReader = new StreamReader("tree.txt");
      string str;
      while ((str = streamReader.ReadLine()) != null)
        intList.Add(int.Parse(str.Split(';')[0]));
      int index = intList.FindIndex((Predicate<int>) (x => x == this.getLocalVersionNumber()));
      return intList[index + 1];
    }

    private void updateListBox2()
    {
      List<string> stringList = new List<string>();
      StreamReader streamReader = new StreamReader("tree.txt");
      string str1;
      while ((str1 = streamReader.ReadLine()) != null)
      {
        string str2 = str1.Split(';')[0];
        string str3 = str1.Split(';')[1];
        if (str3 != "")
          stringList.Add("[Patch #" + str2 + "] " + str3);
      }
      for (int index = stringList.Count - 1; index > 0; --index)
        this.listBox1.Items.Add((object) stringList[index]);
    }

    public List<Patch> getTree()
    {
      List<Patch> patchList = new List<Patch>();
      StreamReader streamReader = new StreamReader("tree.txt");
      int prevVersion = -1;
      string str;
      while ((str = streamReader.ReadLine()) != null)
      {
        string[] strArray = str.Split(';');
        int version = int.Parse(strArray[0]);
        patchList.Add(new Patch((Tree) null, prevVersion, version, strArray[1], strArray[2], strArray[3]));
        prevVersion = version;
      }
      return patchList;
    }

    public void updateListBox()
    {
      List<Patch> tree = this.getTree();
      this.listBox1.DrawMode = DrawMode.OwnerDrawVariable;
      this.listBox1.MeasureItem += new MeasureItemEventHandler(this.lstChoices_MeasureItem);
      this.listBox1.DrawItem += new DrawItemEventHandler(this.lstChoices_DrawItem);
      for (int index = tree.Count - 1; index > 0; --index)
      {
        string str = "Patch " + tree[index].getVersion().ToString() + ": " + tree[index].getName();
        if (tree[index].getPublishDate() != "")
          str = str + "\nReleased on " + tree[index].getPublishDate();
        this.listBox1.Items.Add((object) str);
      }
    }

    private void lstChoices_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      string text = (string) (sender as ListBox).Items[e.Index];
      SizeF sizeF = e.Graphics.MeasureString(text, this.Font);
      e.ItemHeight = (int) sizeF.Height + 2 * this.ItemMargin;
      e.ItemWidth = (int) sizeF.Width;
    }

    private void lstChoices_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (e.Index < 0)
        return;
      string str = (string) (sender as ListBox).Items[e.Index];
      using (SolidBrush solidBrush1 = new SolidBrush(Color.Black))
      {
        using (SolidBrush solidBrush2 = new SolidBrush(Color.White))
        {
          using (SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(130, 135, 144)))
          {
            Graphics graphics1 = e.Graphics;
            SolidBrush solidBrush4 = solidBrush2;
            Rectangle bounds = e.Bounds;
            double x1 = (double) bounds.X;
            bounds = e.Bounds;
            double y1 = (double) bounds.Y;
            PointF location1 = new PointF((float) x1, (float) y1);
            bounds = e.Bounds;
            double width = (double) bounds.Width;
            bounds = e.Bounds;
            double height1 = (double) bounds.Height;
            SizeF size1 = new SizeF((float) width, (float) height1);
            RectangleF rect1 = new RectangleF(location1, size1);
            graphics1.FillRectangle((Brush) solidBrush4, rect1);
            Graphics graphics2 = e.Graphics;
            string s = str;
            Font font = this.Font;
            SolidBrush solidBrush5 = solidBrush1;
            bounds = e.Bounds;
            double left = (double) bounds.Left;
            bounds = e.Bounds;
            double num1 = (double) (bounds.Top + this.ItemMargin);
            graphics2.DrawString(s, font, (Brush) solidBrush5, (float) left, (float) num1);
            Graphics graphics3 = e.Graphics;
            SolidBrush solidBrush6 = solidBrush3;
            bounds = e.Bounds;
            double x2 = (double) bounds.X;
            bounds = e.Bounds;
            int y2 = bounds.Y;
            bounds = e.Bounds;
            int height2 = bounds.Height;
            double num2 = (double) (y2 + height2 - 1);
            PointF location2 = new PointF((float) x2, (float) num2);
            bounds = e.Bounds;
            SizeF size2 = new SizeF((float) bounds.Width, 1f);
            RectangleF rect2 = new RectangleF(location2, size2);
            graphics3.FillRectangle((Brush) solidBrush6, rect2);
          }
        }
      }
    }

    public void progress(int percentage)
    {
      this.updateButton.Text = "Updating(" + percentage.ToString() + "%)";
      this.updateProgressBar.Value = percentage;
    }

    public void update()
    {
      this.updateButton.Enabled = false;
      this.launchButton.Enabled = false;
      Tree tree = new Tree("http://steevens.net/tf2c-public/");
      bool flag = true;
      while (this.currentVersionNumber < tree.getLatestVersionNumber())
      {
        Patch patchFromVersion = tree.getNextPatchFromVersion(this.currentVersionNumber);
        this.statusPanel.ForeColor = Color.Black;
        this.statusPanel.Text = "Installing patch " + patchFromVersion.getVersion().ToString() + " \"" + patchFromVersion.getName() + "\"";
        if (!patchFromVersion.install(new Action<int>(this.progress), this.getGamePath()))
        {
          flag = false;
          break;
        }
        ++this.currentVersionNumber;
        this.saveLocalVersionNumber();
      }
      if (flag)
      {
        this.statusPanel.ForeColor = Color.Black;
        this.statusPanel.Text = "Game updated successfully!";
      }
      else
      {
        this.statusPanel.ForeColor = Color.Red;
        this.statusPanel.Text = "A problem occurred while updating, cancelled update.";
      }
      this.updateButton.Enabled = !flag;
      this.updateButton.Text = "Update";
      this.launchButton.Enabled = true;
      this.updateProgressBar.Value = 0;
      this.applyLauncherUpdates();
    }

    private int getLocalVersionNumber()
    {
      return this.currentVersionNumber;
    }

    private bool fetchLocalVersionNumber()
    {
      try
      {
        StreamReader streamReader = new StreamReader(this.getGamePath() + "\\rev.txt");
        string s = streamReader.ReadLine();
        streamReader.Close();
        this.currentVersionNumber = int.Parse(s);
        if (this.currentVersionNumber < this.tree.getEarliestVersionNumber())
        {
          int num = (int) MessageBox.Show("Illegal version number \"" + this.currentVersionNumber.ToString() + "\", as a result you will not be able to update the game!", "An error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          this.updateButton.Enabled = false;
          return false;
        }
      }
      catch (IOException ex)
      {
        if (!System.IO.File.Exists(this.getGamePath() + "\\rev.txt"))
        {
          this.currentVersionNumber = this.tree.getEarliestVersionNumber();
          this.saveLocalVersionNumber();
        }
      }
      return true;
    }

    private void saveLocalVersionNumber()
    {
      StreamWriter streamWriter = new StreamWriter(this.getGamePath() + "\\rev.txt");
      streamWriter.WriteLine(this.currentVersionNumber);
      streamWriter.Close();
    }

    private void updateGameClient()
    {
      this.currentPatchFile = "patch" + this.getLocalVersionNumber().ToString() + "_" + this.getNextTreeVersion().ToString() + ".zip";
      this.updateButton.Enabled = false;
      this.launchButton.Enabled = false;
      try
      {
        using (WebClient client = new WebClient())
        {
          client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.downloadProgressChanged);
          client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.downloadFileCompletedPatch);
          Patch patchFromVersion = this.tree.getNextPatchFromVersion(this.getLocalVersionNumber());
          this.statusPanel.Text = "Downloading and installing [patch " + patchFromVersion.getVersion().ToString() + "] \"" + patchFromVersion.getName() + "\"";
          this.downloadFile(client, "http://steevens.net/tf2c-public/", this.currentPatchFile, true);
        }
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.ToString(), "An error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
    }

    private void downloadFileCompletedPatch(object sender, AsyncCompletedEventArgs e)
    {
      if (e.Error == null && !e.Cancelled)
      {
        this.extractFiles();
      }
      else
      {
        int num = (int) MessageBox.Show(e.ToString() + " " + e.Error?.ToString(), "An error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        this.updateProgressBar.Value = 0;
        this.updateButton.Enabled = true;
        this.updateButton.Text = "Update";
        this.launchButton.Enabled = true;
      }
    }

    private void updateButton_Click(object sender, EventArgs e)
    {
      this.statusPanel.ForeColor = Color.Black;
      this.statusPanel.Text = "Updating...";
      this.updateProgressBar.Maximum = 100;
      this.updateProgressBar.Value = 0;
      Task.Factory.StartNew((Action) (() => this.update()));
    }

    private void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      this.updateProgressBar.Maximum = 100;
      this.updateProgressBar.Value = e.ProgressPercentage;
      this.updateButton.Text = "Updating (" + e.ProgressPercentage.ToString() + "%)";
    }

    private void applyLauncherUpdates()
    {
      if (!System.IO.File.Exists(this.executableFilename + ".updated"))
        return;
      System.IO.File.Move(this.executableFilename, this.executableFilename + ".old");
      System.IO.File.Move(this.executableFilename + ".updated", this.executableFilename);
      while (!System.IO.File.Exists(this.executableFilename))
        Thread.Sleep(50);
      if (MessageBox.Show("An update to the launcher has been installed.\nWould you like to restart the launcher?", "An update to the launcher has been installed.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        return;
      Process.Start(this.executableFilename);
      Environment.Exit(0);
    }

    private void removeFiles()
    {
      string path1 = this.getGamePath() + "\\.remove";
      if (!System.IO.File.Exists(path1))
        return;
      using (StreamReader streamReader = new StreamReader(path1))
      {
        string str;
        while ((str = streamReader.ReadLine()) != null)
        {
          string path2 = this.getGamePath() + "/" + str;
          if (System.IO.File.Exists(path2))
            System.IO.File.Delete(path2);
          else if (Directory.Exists(path2))
          {
            Console.WriteLine("Removing dir" + path2);
            Directory.Delete(path2);
          }
          Console.WriteLine(path2);
        }
      }
      System.IO.File.Delete(path1);
    }

    private void extractFiles()
    {
      ZipArchive archive = ZipFile.OpenRead(this.currentPatchFile);
      int toDirectory = archive.ExtractToDirectory(this.getGamePath(), true, this.updateButton, this.updateProgressBar);
      archive.Dispose();
      System.IO.File.Delete(this.currentPatchFile);
      this.removeFiles();
      this.updateButton.Text = "Update";
      if (toDirectory == 0)
      {
        this.currentVersionNumber = this.getNextTreeVersion();
        if (this.getLatestTreeVersion() > this.getLocalVersionNumber())
        {
          this.updateGameClient();
          this.saveLocalVersionNumber();
        }
        else
        {
          this.statusPanel.ForeColor = Color.Black;
          this.statusPanel.Text = "Game successfully updated!";
          this.updateProgressBar.Value = 0;
          this.launchButton.Enabled = true;
          this.saveLocalVersionNumber();
          this.applyLauncherUpdates();
        }
      }
      else
      {
        this.statusPanel.ForeColor = Color.Red;
        this.statusPanel.Text = "A problem occurred while updating, cancelled update.";
        this.updateProgressBar.Value = 0;
        this.updateButton.Enabled = true;
        this.launchButton.Enabled = true;
      }
    }

    private void launchButton_Click(object sender, EventArgs e)
    {
      string str = "-steam -game \"" + this.getGamePath() + "\" " + this.paramBox.Text;
      try
      {
        if (this.steam.isSteamInstalled())
        {
          InstallationStatus appIdStatus = this.steam.getAppIdStatus(243750, this.steam.getLibraryFolders());
          if (!appIdStatus.isInstalled())
          {
            Process.Start("steam://install/243750");
            this.statusPanel.ForeColor = Color.Red;
            this.statusPanel.Text = "Source SDK Base 2013 Multiplayer must be installed to run the game.";
            return;
          }
          if (appIdStatus.isUpdating())
          {
            this.statusPanel.ForeColor = Color.Red;
            this.statusPanel.Text = "Source SDK Base 2013 Multiplayer is still updating or being installed.";
            return;
          }
        }
        if (!this.sdkPathBox.Text.EndsWith("hl2.exe") || !System.IO.File.Exists(this.sdkPathBox.Text))
        {
          this.statusPanel.ForeColor = Color.Red;
          this.statusPanel.Text = "The configured sdk path has to be set to run \"hl2.exe\"! You can press detect in the configuration tab.";
          if (MessageBox.Show("Your sdk path is configured incorrectly.\nWould you like to try automatically detect this location?", "SDK Path configuration error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
          this.detectSDK(true);
          this.saveConfiguration();
        }
        else if (!System.IO.File.Exists(this.getGamePath() + "\\gameinfo.txt"))
        {
          this.statusPanel.ForeColor = Color.Red;
          this.statusPanel.Text = "This application must be placed in the root directory of the mod in order to function correctly!";
        }
        else
        {
          if (!new Process()
          {
            StartInfo = {
              FileName = this.sdkPathBox.Text,
              Arguments = str
            }
          }.Start())
            return;
          this.Dispose();
          this.Close();
        }
      }
      catch (Exception ex)
      {
        this.statusPanel.ForeColor = Color.Red;
        this.statusPanel.Text = "A Problem occurred while launching the game!";
      }
    }

    private void sdkBrowseButton_Click(object sender, EventArgs e)
    {
      if (this.fileBrowseDialog.ShowDialog() != DialogResult.OK)
        return;
      this.sdkPathBox.Text = this.fileBrowseDialog.FileName;
    }

    private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
    {
      this.saveConfiguration();
    }

    public void saveConfiguration()
    {
      try
      {
        StreamWriter streamWriter = new StreamWriter(this.getGamePath() + "\\config.txt");
        streamWriter.WriteLine(this.sdkPathBox.Text);
        streamWriter.WriteLine(this.paramBox.Text);
        streamWriter.Close();
      }
      catch (Exception ex)
      {
        this.statusPanel.ForeColor = Color.Red;
        this.statusPanel.Text = "A Problem occurred while saving the configuration!";
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.detectSDK(true);
    }

    private void detectSDK(bool updatePathBox = true)
    {
      if (!this.steam.isSteamInstalled())
        return;
      InstallationStatus appIdStatus = this.steam.getAppIdStatus(243750, this.steam.getLibraryFolders());
      if (updatePathBox)
        this.sdkPathBox.Text = appIdStatus.getInstallationDirectory() + "\\hl2.exe";
      if (appIdStatus.isInstalled())
        return;
      Process.Start("steam://install/243750");
    }

    private string getGamePath()
    {
      return AppDomain.CurrentDomain.BaseDirectory.Remove(AppDomain.CurrentDomain.BaseDirectory.Length - 1);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
      this.pictureBox1 = new PictureBox();
      this.imageList1 = new ImageList(this.components);
      this.fileBrowseDialog = new OpenFileDialog();
      this.folderBrowseDialog = new FolderBrowserDialog();
      this.statusStrip1 = new StatusStrip();
      this.statusPanel = new ToolStripStatusLabel();
      this.configTab = new TabPage();
      this.groupBox1 = new GroupBox();
      this.button1 = new Button();
      this.label2 = new Label();
      this.label3 = new Label();
      this.sdkBrowseButton = new Button();
      this.sdkPathBox = new RichTextBox();
      this.paramBox = new RichTextBox();
      this.tabPage1 = new TabPage();
      this.label4 = new Label();
      this.listBox1 = new ListBox();
      this.updateProgressBar = new ProgressBar();
      this.updateButton = new Button();
      this.launchButton = new Button();
      this.tabControl1 = new TabControl();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.statusStrip1.SuspendLayout();
      this.configTab.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.SuspendLayout();
      this.pictureBox1.Image = (Image) componentResourceManager.GetObject("pictureBox1.Image");
      this.pictureBox1.Location = new Point(-1, -1);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(577, 138);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.imageList1.ImageStream = (ImageListStreamer) componentResourceManager.GetObject("imageList1.ImageStream");
      this.imageList1.TransparentColor = Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "play.png");
      this.statusStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.statusPanel
      });
      this.statusStrip1.Location = new Point(0, 475);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new Size(574, 22);
      this.statusStrip1.SizingGrip = false;
      this.statusStrip1.TabIndex = 2;
      this.statusStrip1.Text = "statusStrip1";
      this.statusPanel.Name = "statusPanel";
      this.statusPanel.Size = new Size(0, 17);
      this.configTab.Controls.Add((Control) this.groupBox1);
      this.configTab.Location = new Point(4, 22);
      this.configTab.Name = "configTab";
      this.configTab.Padding = new Padding(3);
      this.configTab.Size = new Size(569, 303);
      this.configTab.TabIndex = 2;
      this.configTab.Text = "Game configuration";
      this.configTab.UseVisualStyleBackColor = true;
      this.groupBox1.Controls.Add((Control) this.button1);
      this.groupBox1.Controls.Add((Control) this.label2);
      this.groupBox1.Controls.Add((Control) this.label3);
      this.groupBox1.Controls.Add((Control) this.sdkBrowseButton);
      this.groupBox1.Controls.Add((Control) this.sdkPathBox);
      this.groupBox1.Controls.Add((Control) this.paramBox);
      this.groupBox1.Location = new Point(3, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(563, 148);
      this.groupBox1.TabIndex = 20;
      this.groupBox1.TabStop = false;
      this.button1.Location = new Point(506, 32);
      this.button1.Name = "button1";
      this.button1.Size = new Size(50, 25);
      this.button1.TabIndex = 20;
      this.button1.Text = "Detect";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.label2.AutoSize = true;
      this.label2.Location = new Point(6, 16);
      this.label2.Name = "label2";
      this.label2.Size = new Size(200, 13);
      this.label2.TabIndex = 18;
      this.label2.Text = "Source SDK 2013 multiplayer executable";
      this.label3.AutoSize = true;
      this.label3.Location = new Point(6, 91);
      this.label3.Name = "label3";
      this.label3.Size = new Size(98, 13);
      this.label3.TabIndex = 19;
      this.label3.Text = "Launch parameters";
      this.sdkBrowseButton.Location = new Point(506, 63);
      this.sdkBrowseButton.Name = "sdkBrowseButton";
      this.sdkBrowseButton.Size = new Size(50, 25);
      this.sdkBrowseButton.TabIndex = 13;
      this.sdkBrowseButton.Text = "Browse";
      this.sdkBrowseButton.UseVisualStyleBackColor = true;
      this.sdkBrowseButton.Click += new EventHandler(this.sdkBrowseButton_Click);
      this.sdkPathBox.Location = new Point(6, 32);
      this.sdkPathBox.Name = "sdkPathBox";
      this.sdkPathBox.Size = new Size(494, 56);
      this.sdkPathBox.TabIndex = 12;
      this.sdkPathBox.Text = "";
      this.paramBox.Location = new Point(6, 107);
      this.paramBox.Name = "paramBox";
      this.paramBox.Size = new Size(551, 31);
      this.paramBox.TabIndex = 16;
      this.paramBox.Text = "";
      this.tabPage1.Controls.Add((Control) this.label4);
      this.tabPage1.Controls.Add((Control) this.listBox1);
      this.tabPage1.Controls.Add((Control) this.updateProgressBar);
      this.tabPage1.Controls.Add((Control) this.updateButton);
      this.tabPage1.Controls.Add((Control) this.launchButton);
      this.tabPage1.Location = new Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new Padding(3);
      this.tabPage1.Size = new Size(569, 303);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Launch game";
      this.tabPage1.UseVisualStyleBackColor = true;
      this.label4.AutoSize = true;
      this.label4.Location = new Point(3, 40);
      this.label4.Name = "label4";
      this.label4.Size = new Size(80, 13);
      this.label4.TabIndex = 24;
      this.label4.Text = "Latest updates:";
      this.listBox1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new Point(6, 56);
      this.listBox1.Name = "listBox1";
      this.listBox1.SelectionMode = SelectionMode.None;
      this.listBox1.Size = new Size(557, 238);
      this.listBox1.TabIndex = 23;
      this.updateProgressBar.Location = new Point(205, 6);
      this.updateProgressBar.MarqueeAnimationSpeed = 200;
      this.updateProgressBar.Name = "updateProgressBar";
      this.updateProgressBar.Size = new Size(358, 31);
      this.updateProgressBar.Style = ProgressBarStyle.Continuous;
      this.updateProgressBar.TabIndex = 22;
      this.updateButton.Enabled = false;
      this.updateButton.Location = new Point(103, 6);
      this.updateButton.Name = "updateButton";
      this.updateButton.Size = new Size(96, 31);
      this.updateButton.TabIndex = 21;
      this.updateButton.Text = "Update";
      this.updateButton.UseVisualStyleBackColor = true;
      this.updateButton.Click += new EventHandler(this.updateButton_Click);
      this.launchButton.Location = new Point(6, 6);
      this.launchButton.Name = "launchButton";
      this.launchButton.Size = new Size(94, 31);
      this.launchButton.TabIndex = 20;
      this.launchButton.Text = "Launch";
      this.launchButton.UseVisualStyleBackColor = true;
      this.launchButton.Click += new EventHandler(this.launchButton_Click);
      this.tabControl1.Controls.Add((Control) this.tabPage1);
      this.tabControl1.Controls.Add((Control) this.configTab);
      this.tabControl1.Location = new Point(-1, 143);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new Size(577, 329);
      this.tabControl1.TabIndex = 2;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(574, 497);
      this.Controls.Add((Control) this.statusStrip1);
      this.Controls.Add((Control) this.tabControl1);
      this.Controls.Add((Control) this.pictureBox1);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.Name = nameof (Form1);
      this.Text = "Team Fortress 2 Classic Launcher";
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.configTab.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      this.tabControl1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
