using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;

namespace TTToolbar
{
  public partial class TTToolbarMainForm : Form
  {
    List<Shortcut> shortcuts = new List<Shortcut>();
    PictureBox iconBeingDragged = null;
    string shortcutIndexing = "N/A";
    int maxIconSize = 60;
    int minIconSize = 40;
    int currentIconSize = 60;
    Size defaultSize = new Size(460, 113);

    //-------------------------------------------------------------------------

    struct Shortcut
    {
      public string exeRelFilename;
      public string name;
      public string link;
      public Image originalImage;
      public PictureBox icon;
      public int iconSourceWidth;
      public int iconSourceHeight;
    }

    //-------------------------------------------------------------------------

    public TTToolbarMainForm()
    {
      InitializeComponent();

      PopulateListOfShortcutsFromConfigXml();
      ArrangeShortcutsFromRegistrySetting();

      ApplyRegistrySettings();

      ScaleImageIcon(GetScaledPercentage());
      PlaceShortcutsOnForm();

      this.TransparencyKey = this.BackColor;

      // Write to registry so app launches on startup (WIP).
      //string exeFullPath = Environment.CurrentDirectory.ToString() + "\\" + "TTToolbar.exe";
      //RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
      //key.SetValue("TTToolbar", exeFullPath);
    }

    //-------------------------------------------------------------------------

    private void PopulateListOfShortcutsFromConfigXml()
    {
      XmlDocument document = new XmlDocument();
      document.Load("toolbarConfig.xml");

      XmlNodeList list = document.GetElementsByTagName("ToolbarShortcut");
      int index = 0;

      foreach (XmlNode shortcutNode in list)
      {
        XmlElement shortcut = shortcutNode as XmlElement;

        AddNewShortcutToList(shortcut, index);
        index++;
      }
    }

    //-------------------------------------------------------------------------

    private void AddNewShortcutToList(XmlElement shortcut, int index)
    {
      // Get info
      Shortcut shortcutInfo = new Shortcut();
      shortcutInfo.name = shortcut.GetAttribute("name");

      // Create new icon.
      PictureBox icon = CreateNewIcon(shortcut.GetAttribute("iconFilename"), shortcutInfo.name);
      shortcutInfo.originalImage = icon.Image;
      shortcutInfo.icon = icon;
      shortcutInfo.iconSourceHeight = shortcutInfo.originalImage.Height;
      shortcutInfo.iconSourceWidth = shortcutInfo.originalImage.Width;

      // Get exe or link to run.
      shortcutInfo.link = "N/A";
      shortcutInfo.exeRelFilename = "N/A";

      if (shortcut.HasAttribute("exeRelFilename"))
      {
        shortcutInfo.exeRelFilename = shortcut.GetAttribute("exeRelFilename");
      }
      else if (shortcut.HasAttribute("link"))
      {
        shortcutInfo.link = shortcut.GetAttribute("link");
      }
      else
      {
        MessageBox.Show("Must specify exe or link.", "Error");
      }

      // Only add shortcut to list, if it isn't a duplicate - ignore duplicates.
      if (CheckIfNameExists(shortcutInfo.name))
      {
        MessageBox.Show("Duplicate name - shortcut not added.", "Error");
      }
      else
      {
        this.shortcuts.Add(shortcutInfo);
      }
    }

    //-------------------------------------------------------------------------

    private void ArrangeShortcutsFromRegistrySetting()
    {
      GetShortcutSortingFromRegistry();

      // If setting does not exist in registry, don't go any further.
      if (this.shortcutIndexing.Equals("N/A"))
      {
        return;
      }

      int currentIndex = 0;
      List<Shortcut> temporaryShortcutsList =  new List<Shortcut>();
      string newShortcutIndexing = "";

      foreach (char indexString in this.shortcutIndexing.ToCharArray())
      {
        int index = Convert.ToInt32(indexString.ToString());

        // Only add shortcuts with index that falls within the shortcut icon number range.
        if (index < this.shortcuts.Count())
        {
          temporaryShortcutsList.Add(this.shortcuts[index]);
          newShortcutIndexing = newShortcutIndexing + indexString;
        }

        currentIndex++;
      }

      this.shortcutIndexing = newShortcutIndexing;

      // If new icons have been added, since setting saved to registry - add these to the list.
      if (this.shortcutIndexing.Length < this.shortcuts.Count())
      {
        for (int i = this.shortcutIndexing.Length; i < this.shortcuts.Count(); i++)
        {
          temporaryShortcutsList.Add( this.shortcuts[i] );
          this.shortcutIndexing = this.shortcutIndexing + i;
        }
      }

      this.shortcuts = temporaryShortcutsList;
    }

    //-------------------------------------------------------------------------

    private void ApplyRegistrySettings()
    {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TTToolbar");

      if (key != null)
      {
        if (key.GetValue("iconSize") != null)
        {
          this.currentIconSize = Convert.ToInt32(key.GetValue("iconSize").ToString());
          this.Size = new Size(Convert.ToInt32(key.GetValue("windowWidth")), this.Size.Height);
        }

        key.Close();
      }

      // Set starting position to setting stored in registry (if it exists).
      this.StartPosition = FormStartPosition.Manual;
      this.Location = this.GetStoredPosition();
    }

    //-------------------------------------------------------------------------

    private void GetShortcutSortingFromRegistry()
    {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TTToolbar");

      if (key != null)
      {
        if (key.GetValue("indexing") != null)
        {
          this.shortcutIndexing = key.GetValue("indexing").ToString();
        }

        key.Close();
      }
    }

    //-------------------------------------------------------------------------

    private PictureBox CreateNewIcon(string filename, string name)
    {
      PictureBox icon = new PictureBox();
      Bitmap iconImage = new Bitmap(filename);
      icon.Image = iconImage;
      icon.Size = new Size(50, 50);
      icon.SizeMode = PictureBoxSizeMode.CenterImage;
      icon.Name = name;
      icon.MouseHover += new System.EventHandler(this.icon_MouseHover);
      icon.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
      icon.MouseDown += new MouseEventHandler(this.icon_MouseDown);
      icon.DragEnter += new DragEventHandler(this.icon_DragEnter);
      icon.DragDrop += new DragEventHandler(this.icon_DragDrop);
      icon.AllowDrop = true;
      icon.Anchor = AnchorStyles.Top;

      return icon;
    }

    //-------------------------------------------------------------------------

    private void PlaceShortcutsOnForm()
    {
      // Clear shortcuts.
      ClearShortcuts();

      // Add row
      this.shortcutIcons.RowCount = this.shortcutIcons.RowCount + 1;
      this.shortcutIcons.RowStyles.Add(new RowStyle(SizeType.Absolute, this.currentIconSize));
      this.shortcutIcons.Size = new Size(0, this.currentIconSize);

      int maxNumberOfIconsPerRow = (this.Size.Width - 40) / this.currentIconSize;
      int numberOfIconsAdded = 0;

      foreach (Shortcut shortcut in this.shortcuts)
      {
        if (numberOfIconsAdded != 0 && numberOfIconsAdded % maxNumberOfIconsPerRow == 0)
        {
          // Add row
          this.shortcutIcons.RowCount = this.shortcutIcons.RowCount + 1;
          this.shortcutIcons.RowStyles.Add(new RowStyle(SizeType.Absolute, this.currentIconSize));
          this.shortcutIcons.Size = new Size(this.shortcutIcons.Size.Width, this.shortcutIcons.Size.Height + this.currentIconSize);
        }
        else if (numberOfIconsAdded < maxNumberOfIconsPerRow)
        {
          // Add column
          this.shortcutIcons.ColumnCount = this.shortcutIcons.ColumnCount + 1;
          this.shortcutIcons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, this.currentIconSize));
          this.shortcutIcons.Size = new Size(this.shortcutIcons.Size.Width + this.currentIconSize, this.shortcutIcons.Size.Height);
        }

        numberOfIconsAdded++;
        this.shortcutIcons.Controls.Add(shortcut.icon);
      }

      // Auto size form height.
      this.Size = new Size(this.Size.Width, 50 + (this.currentIconSize * this.shortcutIcons.RowCount));
    }

    //-------------------------------------------------------------------------

    private void ClearShortcuts()
    {
      this.shortcutIcons.Controls.Clear();
      this.shortcutIcons.ColumnStyles.Clear();
      this.shortcutIcons.ColumnCount = 0;
      this.shortcutIcons.RowStyles.Clear();
      this.shortcutIcons.RowCount = 0;
      this.shortcutIcons.Size = new Size(0, 0);
    }

    //-------------------------------------------------------------------------

    private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Show();
      WindowState = FormWindowState.Normal;
    }

    //-------------------------------------------------------------------------

    private void closeApplicationToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Close();
    }

    //-------------------------------------------------------------------------

    private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      Show();
      WindowState = FormWindowState.Normal;
    }

    //-------------------------------------------------------------------------

    private void icon_MouseHover(object sender, EventArgs e)
    {
      PictureBox pictureBox = (PictureBox)sender;
      pictureBox.BackColor = Color.LightGray;
      this.Text = pictureBox.Name;
    }

    //-------------------------------------------------------------------------

    private void icon_MouseLeave(object sender, EventArgs e)
    {
      PictureBox pictureBox = (PictureBox)sender;
      pictureBox.BackColor = Color.Transparent;
      this.Text = "TT Toolbar";
    }

    //-------------------------------------------------------------------------

    private void icon_MouseDown(object sender, MouseEventArgs e)
    {
      this.iconBeingDragged = (PictureBox)sender;

      if (e.Button == MouseButtons.Left)
      {
        this.iconBeingDragged.DoDragDrop(this.iconBeingDragged.Image, DragDropEffects.Move);
      }
    }

    //-------------------------------------------------------------------------

    void icon_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.Bitmap))
      {
        e.Effect = DragDropEffects.Move;
      }
      else
      {
        e.Effect = DragDropEffects.None;
      }
    }

    //-------------------------------------------------------------------------

    void icon_DragDrop(object sender, DragEventArgs e)
    {
      PictureBox pictureBox = (PictureBox)sender;

      if ((e.Data.GetDataPresent(DataFormats.Bitmap)))
      {
        if (this.iconBeingDragged != pictureBox)
        {
          Shortcut shortcutToInsert = GetShortcutFromName(this.iconBeingDragged.Name);
          int indexOfShortcutToInsert = GetIndexOfShortcut(shortcutToInsert);
          int indexToInsertAt = GetIndexOfShortcut(GetShortcutFromName(pictureBox.Name));

          // Insert selected icon at insertion position and update indexing for registry.
          InsertShortcutAtIndex(shortcutToInsert, indexToInsertAt);
          UpdateIndexing(indexOfShortcutToInsert, indexToInsertAt);

          PlaceShortcutsOnForm();
        }
        else
        {
          RunShortcut(pictureBox);
        }
      }
    }

    //-------------------------------------------------------------------------

    void Form_Resize(object sender, EventArgs e)
    {
      if (WindowState == FormWindowState.Minimized)
      {
        this.Hide();
      }
    }

    //-------------------------------------------------------------------------

    void Form_ResizeEnd(object sender, EventArgs e)
    {
      this.currentIconSize = (this.Size.Width - 40) / this.shortcuts.Count;

      this.currentIconSize = Math.Max(this.currentIconSize, this.minIconSize);
      this.currentIconSize = Math.Min(this.currentIconSize, this.maxIconSize);

      ScaleImageIcon(GetScaledPercentage());

      PlaceShortcutsOnForm();
    }

    //-------------------------------------------------------------------------

    private double GetScaledPercentage()
    {
      return (double)(currentIconSize - this.minIconSize) / (double)(this.maxIconSize - this.minIconSize);
    }

    //-------------------------------------------------------------------------

    void ScaleImageIcon(double percentage)
    {
      // Don't scale below 50%.
      percentage = Math.Max(0.5, percentage);

      List<Bitmap> resizedImages = new List<Bitmap>();

      foreach (Shortcut shortcut in this.shortcuts)
      {
        int newWidth = (int)Math.Round(shortcut.iconSourceWidth * percentage, 0);
        int newHeight = (int)Math.Round(shortcut.iconSourceHeight * percentage, 0);

        newWidth = Math.Min(newWidth, shortcut.iconSourceWidth);
        newHeight = Math.Min(newHeight, shortcut.iconSourceHeight);

        resizedImages.Add(new Bitmap(shortcut.originalImage, new Size(newWidth, newHeight)));
      }

      int index = 0;
      foreach (Bitmap icon in resizedImages)
      {
        this.shortcuts[index].icon.Image = icon;

        index++;
      }
    }

    //-------------------------------------------------------------------------

    private void InsertShortcutAtIndex(Shortcut shortcutToInsert, int indexToInsertAt)
    {
      this.shortcuts.Remove(shortcutToInsert);
      this.shortcuts.Insert(indexToInsertAt, shortcutToInsert);
    }

    //-------------------------------------------------------------------------

    private void UpdateIndexing(int indexToInsertFrom, int indexToInsertAt)
    {
      PopulateShortcutIndexingString();

      string valueToInsert = this.shortcutIndexing[indexToInsertFrom].ToString();
      this.shortcutIndexing = this.shortcutIndexing.Remove(indexToInsertFrom, 1);
      this.shortcutIndexing = this.shortcutIndexing.Insert(indexToInsertAt, valueToInsert);
    }

    //-------------------------------------------------------------------------

    private void PopulateShortcutIndexingString()
    {
      // If registry settings haven't previously been stored, populate indexing string with indices in order.
      if (this.shortcutIndexing.Equals("N/A"))
      {
        this.shortcutIndexing = this.shortcutIndexing.Remove(0);

        for (int i = 0; i < this.shortcuts.Count(); i++)
        {
          this.shortcutIndexing = this.shortcutIndexing + i;
        }
      }
    }

    //-------------------------------------------------------------------------

    private void RunShortcut(PictureBox pictureBox)
    {
      Shortcut shortcutClicked = GetShortcutFromName(pictureBox.Name);

      // If exe file specified, try to run it.
      if (!shortcutClicked.exeRelFilename.Equals("N/A"))
      {
        try
        {
          string directory = Environment.CurrentDirectory.ToString();
          string exePath = directory + "\\" + shortcutClicked.exeRelFilename;
          string exeDirectory = System.IO.Path.GetDirectoryName(exePath);

          var startInfo = new System.Diagnostics.ProcessStartInfo();
          startInfo.WorkingDirectory = exeDirectory;
          startInfo.FileName = exePath;

          System.Diagnostics.Process.Start(startInfo);
        }
        catch
        {
          MessageBox.Show("Specified exe does not exist.", "Error");
        }
      }
      // If link specified, run it.
      else if (!shortcutClicked.link.Equals("N/A"))
      {
        System.Diagnostics.Process.Start(shortcutClicked.link);
      }
      // If nothing has been specified to run, show error message.
      else
      {
        MessageBox.Show("No operation associated with this shortcut icon.", "Error");
      }
    }

    //-------------------------------------------------------------------------

    private void OnClosing()
    {
      int x = this.Location.X;
      int y = this.Location.Y;

      RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TTToolbar");
      key.SetValue("x", x);
      key.SetValue("y", y);

      if (!this.shortcutIndexing.Equals("N/A"))
      {
        key.SetValue("indexing", this.shortcutIndexing);
      }

      key.SetValue("iconSize", this.currentIconSize);
      key.SetValue("windowWidth", this.Size.Width);
    }

    //-------------------------------------------------------------------------

    private Point GetStoredPosition()
    {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TTToolbar");
      Point storedPosition = new Point(100, 100);

      if (key != null)
      {
        int x = Convert.ToInt32(key.GetValue("x"));
        int y = Convert.ToInt32(key.GetValue("y"));

        storedPosition = new Point(x, y);
        key.Close();
      }

      return storedPosition;
    }

    //-------------------------------------------------------------------------

    private Shortcut GetShortcutFromName(string nameToCompare)
    {
      Shortcut shortcutToReturn = new Shortcut();

      foreach (Shortcut shortcut in this.shortcuts)
      {
        if (shortcut.name.Equals(nameToCompare))
        {
          shortcutToReturn = shortcut;
          break;
        }
      }

      return shortcutToReturn;
    }

    //-------------------------------------------------------------------------

    private int GetIndexOfShortcut(Shortcut s)
    {
      int index = 0;
      int indexToReturn = -1;

      foreach (Shortcut shortcut in this.shortcuts)
      {
        if (shortcut.name.Equals(s.name))
        {
          indexToReturn = index;
          break;
        }

        index++;
      }

      return indexToReturn;
    }

    //-------------------------------------------------------------------------

    private bool CheckIfNameExists(string name)
    {
      foreach (Shortcut shortcut in this.shortcuts)
      {
        if (shortcut.name.Equals(name))
        {
          return true;
        }
      }

      return false;
    }

    //-------------------------------------------------------------------------
  }
}
