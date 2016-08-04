namespace TTToolbar
{
  partial class TTToolbarMainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      this.OnClosing();
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TTToolbarMainForm));
      this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.startOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.shortcutIcons = new System.Windows.Forms.TableLayoutPanel();
      this.trayMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // trayIcon
      // 
      this.trayIcon.ContextMenuStrip = this.trayMenu;
      this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
      this.trayIcon.Text = "TT Toolbar";
      this.trayIcon.Visible = true;
      this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
      // 
      // trayMenu
      // 
      this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem,
            this.closeApplicationToolStripMenuItem,
            this.startOnStartupToolStripMenuItem});
      this.trayMenu.Name = "trayMenu";
      this.trayMenu.Size = new System.Drawing.Size(168, 70);
      // 
      // restoreToolStripMenuItem
      // 
      this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
      this.restoreToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.restoreToolStripMenuItem.Text = "Restore";
      this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
      // 
      // closeApplicationToolStripMenuItem
      // 
      this.closeApplicationToolStripMenuItem.Name = "closeApplicationToolStripMenuItem";
      this.closeApplicationToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.closeApplicationToolStripMenuItem.Text = "Close Application";
      this.closeApplicationToolStripMenuItem.Click += new System.EventHandler(this.closeApplicationToolStripMenuItem_Click);
      // 
      // startOnStartupToolStripMenuItem
      // 
      this.startOnStartupToolStripMenuItem.Checked = true;
      this.startOnStartupToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.startOnStartupToolStripMenuItem.Name = "startOnStartupToolStripMenuItem";
      this.startOnStartupToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.startOnStartupToolStripMenuItem.Text = "Start on startup";
      this.startOnStartupToolStripMenuItem.Click += new System.EventHandler(this.startOnStartupItem_Click);
      // 
      // shortcutIcons
      // 
      this.shortcutIcons.ColumnCount = 1;
      this.shortcutIcons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.shortcutIcons.Location = new System.Drawing.Point(12, 12);
      this.shortcutIcons.Name = "shortcutIcons";
      this.shortcutIcons.RowCount = 1;
      this.shortcutIcons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.shortcutIcons.Size = new System.Drawing.Size(60, 60);
      this.shortcutIcons.TabIndex = 1;
      // 
      // TTToolbarMainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(444, 107);
      this.Controls.Add(this.shortcutIcons);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "TTToolbarMainForm";
      this.Opacity = 0.5D;
      this.ShowInTaskbar = false;
      this.Text = "TT Toolbar";
      this.TopMost = true;
      this.ResizeEnd += new System.EventHandler(this.Form_ResizeEnd);
      this.DoubleClick += new System.EventHandler(this.TTToolbarMainForm_DoubleClick);
      this.Resize += new System.EventHandler(this.Form_Resize);
      this.trayMenu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.NotifyIcon trayIcon;
    private System.Windows.Forms.ContextMenuStrip trayMenu;
    private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeApplicationToolStripMenuItem;
    private System.Windows.Forms.TableLayoutPanel shortcutIcons;
    private System.Windows.Forms.ToolStripMenuItem startOnStartupToolStripMenuItem;
  }
}

