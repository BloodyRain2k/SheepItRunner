/*
 * Created by SharpDevelop.
 * User: Bernhard
 * Date: 24.05.2016
 * Time: 16:25
 */
namespace SheepItRunner
{
	partial class frmMain
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.ctxTray = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tmrCheck = new System.Windows.Forms.Timer(this.components);
			this.lstLog = new System.Windows.Forms.ListBox();
			this.txtCache = new System.Windows.Forms.TextBox();
			this.lblCache = new System.Windows.Forms.Label();
			this.cmbMode = new System.Windows.Forms.ComboBox();
			this.tipTool = new System.Windows.Forms.ToolTip(this.components);
			this.lblStatus = new System.Windows.Forms.Label();
			this.lblFrames = new System.Windows.Forms.Label();
			this.tmrStart = new System.Windows.Forms.Timer(this.components);
			this.ctxTray.SuspendLayout();
			this.SuspendLayout();
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenuStrip = this.ctxTray;
			this.trayIcon.Text = "SheepIt Runner";
			this.trayIcon.DoubleClick += new System.EventHandler(this.TrayIconDoubleClick);
			// 
			// ctxTray
			// 
			this.ctxTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.startToolStripMenuItem,
									this.exitToolStripMenuItem});
			this.ctxTray.Name = "ctxmnuTray";
			this.ctxTray.Size = new System.Drawing.Size(99, 48);
			// 
			// startToolStripMenuItem
			// 
			this.startToolStripMenuItem.Name = "startToolStripMenuItem";
			this.startToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.startToolStripMenuItem.Text = "&Start";
			this.startToolStripMenuItem.Click += new System.EventHandler(this.StartToolStripMenuItemClick);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
			// 
			// tmrCheck
			// 
			this.tmrCheck.Interval = 20;
			this.tmrCheck.Tick += new System.EventHandler(this.TmrCheckTick);
			// 
			// lstLog
			// 
			this.lstLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lstLog.FormattingEnabled = true;
			this.lstLog.IntegralHeight = false;
			this.lstLog.Location = new System.Drawing.Point(4, 30);
			this.lstLog.Name = "lstLog";
			this.lstLog.ScrollAlwaysVisible = true;
			this.lstLog.Size = new System.Drawing.Size(425, 242);
			this.lstLog.TabIndex = 0;
			this.lstLog.SelectedIndexChanged += new System.EventHandler(this.LstLogSelectedIndexChanged);
			// 
			// txtCache
			// 
			this.txtCache.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtCache.Location = new System.Drawing.Point(49, 4);
			this.txtCache.Name = "txtCache";
			this.txtCache.Size = new System.Drawing.Size(289, 20);
			this.txtCache.TabIndex = 1;
			// 
			// lblCache
			// 
			this.lblCache.Location = new System.Drawing.Point(4, 7);
			this.lblCache.Name = "lblCache";
			this.lblCache.Size = new System.Drawing.Size(39, 14);
			this.lblCache.TabIndex = 2;
			this.lblCache.Text = "Cache";
			// 
			// cmbMode
			// 
			this.cmbMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbMode.FormattingEnabled = true;
			this.cmbMode.Items.AddRange(new object[] {
									"CPU",
									"CPU & GPU",
									"GPU"});
			this.cmbMode.Location = new System.Drawing.Point(344, 3);
			this.cmbMode.Name = "cmbMode";
			this.cmbMode.Size = new System.Drawing.Size(85, 21);
			this.cmbMode.TabIndex = 3;
			// 
			// tipTool
			// 
			this.tipTool.Active = false;
			// 
			// lblStatus
			// 
			this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Location = new System.Drawing.Point(4, 278);
			this.lblStatus.Margin = new System.Windows.Forms.Padding(3);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(245, 16);
			this.lblStatus.TabIndex = 4;
			// 
			// lblFrames
			// 
			this.lblFrames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblFrames.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblFrames.Location = new System.Drawing.Point(255, 278);
			this.lblFrames.Margin = new System.Windows.Forms.Padding(3);
			this.lblFrames.Name = "lblFrames";
			this.lblFrames.Size = new System.Drawing.Size(174, 16);
			this.lblFrames.TabIndex = 5;
			// 
			// tmrStart
			// 
			this.tmrStart.Interval = 10000;
			this.tmrStart.Tick += new System.EventHandler(this.TmrStartTick);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(433, 298);
			this.Controls.Add(this.lblFrames);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.cmbMode);
			this.Controls.Add(this.lblCache);
			this.Controls.Add(this.txtCache);
			this.Controls.Add(this.lstLog);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SheepIt Runner";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMainFormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMainKeyDown);
			this.ctxTray.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Timer tmrStart;
		private System.Windows.Forms.Label lblFrames;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
		private System.Windows.Forms.ToolTip tipTool;
		private System.Windows.Forms.ComboBox cmbMode;
		private System.Windows.Forms.Label lblCache;
		private System.Windows.Forms.TextBox txtCache;
		private System.Windows.Forms.ContextMenuStrip ctxTray;
		private System.Windows.Forms.ListBox lstLog;
		private System.Windows.Forms.Timer tmrCheck;
		private System.Windows.Forms.NotifyIcon trayIcon;
	}
}
