/*
 * Created by SharpDevelop.
 * User: Bernhard
 * Date: 24.05.2016
 * Time: 16:25
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.ApplicationServices;

namespace SheepItRunner
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class frmMain : Form
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetKeyboardState(byte [] lpKeyState);
		
		public static byte[] GetKeys() {
			var b = new byte[256];
			GetKeyboardState(b);
			return b;
		}

		// http://stackoverflow.com/a/9858981
		// Used to check if the screen saver is running
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SystemParametersInfo(uint uAction, uint uParam, ref bool lpvParam, int fWinIni);

		// Used to check if the workstation is locked
		[DllImport("user32", SetLastError = true)]
		private static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags, bool fInherit, uint dwDesiredAccess);

		[DllImport("user32", SetLastError = true)]
		private static extern IntPtr OpenInputDesktop(uint dwFlags, bool fInherit, uint dwDesiredAccess);

		[DllImport("user32", SetLastError = true)]
		private static extern IntPtr CloseDesktop(IntPtr hDesktop);

		[DllImport("user32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SwitchDesktop(IntPtr hDesktop);

		// Check if the workstation has been locked.
		private bool lastLocked;
		public static bool IsWorkstationLocked()
		{
			const int DESKTOP_SWITCHDESKTOP = 256;
			IntPtr hwnd = OpenInputDesktop(0, false, DESKTOP_SWITCHDESKTOP);

			if (hwnd == IntPtr.Zero)
			{
				// Could not get the input desktop, might be locked already?
				hwnd = OpenDesktop("Default", 0, false, DESKTOP_SWITCHDESKTOP);
			}

			// Can we switch the desktop?
			if (hwnd != IntPtr.Zero)
			{
				if (SwitchDesktop(hwnd))
				{
					// Workstation is NOT LOCKED.
					CloseDesktop(hwnd);
				}
				else
				{
					CloseDesktop(hwnd);
					// Workstation is LOCKED.
					return true;
				}
			}

			return false;
		}
		
		// Check if the screensaver is busy running.
		private bool lastScreen;
		public static bool IsScreensaverRunning()
		{
			const int SPI_GETSCREENSAVERRUNNING = 114;
			bool isRunning = false;

			if (!SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0))
			{
				// Could not detect screen saver status...
				return false;
			}

			if (isRunning || !PowerManager.IsMonitorOn)
			{
				// Screen saver is ON.
				return true;
			}

			// Screen saver is OFF.
			return false;
		}
		
		private Process client;
		private bool minimized;
		
		private string login;
		private string password;
		private bool hide = false;
		private BloodyRain2k.AppSettings settings = new BloodyRain2k.AppSettings();
		private string logFile;
		
		private bool closeAfterRender = false;
		private int credits;
		private int framesRendered;
		private int framesRemaining;
		private int lastCredits;
		private int lastRendered;
		private List<string> blacklist = new List<string>(new string[] { "starting", "requesting job", "extracting renderer", "extracting project",
		                                                  	"reusing cached renderer", "reusing cached project", "rendering" });
		
		void updateLogFile() {
			var now = DateTime.Now;
//			logFile = Application.ExecutablePath.Replace(".exe", "");
			logFile = (txtCache.Text + @"\").Replace(@"\\", @"\");
			logFile += string.Format("SheepItRunner_{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute);
		}
		
		void fileLog(string message) {
			var now = DateTime.Now;
			
			if (InvokeRequired) {
				if (((Control)this).IsDisposed) { return; }
				Invoke((MethodInvoker) delegate { fileLog(message); });
			}
			else {
				File.AppendAllText(logFile, now.Year + "." + now.Month + "." + now.Day + " " + now.ToShortTimeString() + " - " + message + "\r\n");
			}
		}
		
		void log(string message, bool timeStamp = true) {
			var now = DateTime.Now;
			if (timeStamp) {
				message = now.Year + "." + now.Month + "." + now.Day + " " + now.ToShortTimeString() + " - " + message;
			}
			if (InvokeRequired) {
				if (((Control)this).IsDisposed) { return; }
				Invoke((MethodInvoker) delegate { log(message, false); });
			}
			else {
				lstLog.Items.Add(message);
				lstLog.SelectedIndex = lstLog.Items.Count - 1;
			}
		}
		
		void setStatus(string status = "") {
			if (InvokeRequired) {
				if (((Control)this).IsDisposed) { return; }
//				if (((Control)lblStatus).IsDisposed) { return; }
				Invoke((MethodInvoker) delegate { lblStatus.Text = status; });
			}
			else {
				lblStatus.Text = status;
			}
		}
		
		void setFrames(string frames = "") {
			if (InvokeRequired) {
				if (((Control)this).IsDisposed) { return; }
//				if (((Control)lblFrames).IsDisposed) { return; }
				Invoke((MethodInvoker) delegate { lblFrames.Text = frames; });
			}
			else {
				lblFrames.Text = frames;
			}
		}
		
		bool ClientRunning() {
			try {
				if (!client.HasExited) { return true; }
			} catch (Exception ex) {}
			return false;
		}
		
		void client_Exited(object sender, EventArgs e)
		{
			log("client exited");
			closeAfterRender = false;
			
			try {
				if (InvokeRequired) {
					Invoke((MethodInvoker) delegate { Icon = trayIcon.Icon = Resources.ShpIt; });
				}
				else {
					Icon = trayIcon.Icon = Resources.ShpIt;
				}
			}
			catch (Exception ex) { }
		}
		
		void client_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			setStatus(e.Data);
			if (e.Data == null || e.Data == "") { return; }
			fileLog(e.Data);
			var str = e.Data.ToLower();
			
			if (str.StartsWith("frames remaining: ") || str.StartsWith("frames rendered: ") || str.StartsWith("credits earned: ")) {
				var splits = str.Split(' ');
				if (splits[1] == "earned:")    { credits = int.Parse(splits[2]); if (closeAfterRender) { StopClient(); } }
				if (splits[1] == "rendered:")  { framesRendered = int.Parse(splits[2]) - 1; }
				if (splits[1] == "remaining:") { framesRemaining = int.Parse(splits[2]); }
				// setFrames(string.Format("Rendered: {0} / {1} - Earned: {2}", framesRendered, framesRemaining, credits));
				setFrames(string.Format("Frames: {0}  -  Earned: {1}", framesRendered + lastRendered, credits + lastCredits));
				return;
			}
			
			if (blacklist.IndexOf(str) > -1 || str.StartsWith("rendering (remaining")
			    || (str.StartsWith("rendering ") && str.EndsWith("%"))
			    || (str.StartsWith("downloading project ") && str.EndsWith("%"))
			   ) {
				return;
			}
			
			log(e.Data);
		}
		
		void client_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null || e.Data == "") { return; }
			fileLog("ERROR: " + e.Data);
			log("ERROR: " + e.Data);
		}
		
		void StartClient() {
			var mode = new string[] { "CPU", "CPU_GPU", "GPU" };
			
			if (ClientRunning()) { return; }
			client = new Process();
			client.StartInfo.FileName = @"javaw";
			client.StartInfo.Arguments = string.Format("-jar SheepItClient.jar -ui text -login {0} -password {1} -cache-dir \"{2}\" -compute-method {3}",
			                                           login, password, txtCache.Text, mode[cmbMode.SelectedIndex] + (cmbMode.SelectedIndex == 0 ? "" : " -gpu CUDA_0"));
//			client.StartInfo.Arguments = string.Format("-jar SheepItClient.jar");
			client.StartInfo.CreateNoWindow = true;
			client.StartInfo.UseShellExecute = false;
			client.StartInfo.RedirectStandardInput = true;
			client.StartInfo.RedirectStandardError = true;
			client.StartInfo.RedirectStandardOutput = true;
			
			client.EnableRaisingEvents = true;
			client.ErrorDataReceived  += new DataReceivedEventHandler(client_ErrorDataReceived);
			client.OutputDataReceived += new DataReceivedEventHandler(client_OutputDataReceived);
			client.Exited += new EventHandler(client_Exited);
			
			try {
				client.Start();
				client.BeginErrorReadLine();
				client.BeginOutputReadLine();
				log("client started");
				Icon = trayIcon.Icon = Resources.ShpItOn;
				updateLogFile();
			} catch (Exception ex) {
				log("can't start client: " + ex.Message);
			}
			
			if (hide && WindowState != FormWindowState.Minimized) {
				WindowState = FormWindowState.Minimized;
			}
		}
		
		void StopClient() {
			try {
				tmrStart.Stop();
				if (client == null || client.HasExited) { return; }
				// client.CloseMainWindow();
				client.Kill();
				fileLog("Killed");
				log("client killed");
				lastCredits += credits; credits = 0;
				lastRendered += framesRendered; framesRendered = 0;
				var kill = new Process();
				kill.StartInfo.FileName = @"taskkill.exe";
				kill.StartInfo.Arguments = @"/f /im rend.exe";
				kill.StartInfo.CreateNoWindow = true;
				kill.StartInfo.UseShellExecute = false;
				kill.Start();
			} catch (Exception ex) {
				log(ex.Message);
			}
		}
		
		
		
		public frmMain(string[] args)
		{
			for (var i = 0; i < args.Length; i++) {
				switch (args[i].ToLower()) {
					case "-hide":
						hide = true; break;
					case "-login":
						if (i + 1 >= args.Length) { break; }
						login = args[i + 1]; break;
					case "-password":
						if (i + 1 >= args.Length) { break; }
						password = args[i + 1]; break;
				}
			}
			
			InitializeComponent();
			
			txtCache.Text = settings.String["cache"];
			cmbMode.SelectedIndex = settings.Int["mode"];
			Icon = trayIcon.Icon = Resources.ShpIt;
			setFrames("Frames: 0  -  Earned: 0");
			
			login = (string.IsNullOrEmpty(login) ? settings.String["login"] : login);
			if (string.IsNullOrEmpty(login)) { log("no login"); }
			
			password = (string.IsNullOrEmpty(password) ? settings.String["password"] : password);
			if (string.IsNullOrEmpty(password)) { log("no password"); }
			
			log("started");
			tmrCheck.Start();
		}

		void TmrCheckTick(object sender, EventArgs e)
		{
			if (lastLocked != IsWorkstationLocked()) {
				lastLocked = IsWorkstationLocked();
				log(lastLocked ? "locked" : "unlocked");
				if (lastLocked || lastScreen) { StartClient(); }
				else { StopClient(); }
			}
			
			if (lastScreen != IsScreensaverRunning()) {
				lastScreen = IsScreensaverRunning();
				if (lastLocked || lastScreen) { tmrStart.Start(); }
				else { StopClient(); }
			}
			
			if (minimized != (WindowState == FormWindowState.Minimized)) {
				minimized = (WindowState == FormWindowState.Minimized);
				trayIcon.Visible = minimized;
				Visible = ShowInTaskbar = !minimized;
			}
		}
		
		void FrmMainKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F2) { StartClient(); }
			else if (e.KeyCode == Keys.F3) { StopClient(); }
			else if (e.KeyCode == Keys.F4 && client != null && !client.HasExited) {
				closeAfterRender = true;
				log("exiting after render");
			}
		}
		
		void FrmMainFormClosing(object sender, FormClosingEventArgs e)
		{
			StopClient();
			if (txtCache.Text != "") { settings.Add("cache", txtCache.Text); }
			settings.Add("mode", cmbMode.SelectedIndex);
			if (!string.IsNullOrEmpty(settings.String["login"])) { settings.Add("login", login); } // only save if they were already in the settings at startup
			if (!string.IsNullOrEmpty(settings.String["password"])) { settings.Add("password", password); }
			settings.Save();
		}
		
		void TrayIconDoubleClick(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Normal;
			trayIcon.Visible = false;
			Visible = ShowInTaskbar = true;
		}
		
		void LstLogSelectedIndexChanged(object sender, EventArgs e)
		{
			tipTool.SetToolTip(lstLog, lstLog.SelectedItem.ToString());
		}
		
		void StartToolStripMenuItemClick(object sender, EventArgs e)
		{
			StartClient();
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void TmrStartTick(object sender, EventArgs e)
		{
			log("screensaver " + (lastScreen ? "on" : "off"));
			StartClient();
			tmrStart.Stop();
		}
	}
}
