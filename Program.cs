/*
 * Created by SharpDevelop.
 * User: Bernhard
 * Date: 24.05.2016
 * Time: 16:25
 */
using System;
using System.Windows.Forms;

namespace SheepItRunner
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain(args));
		}
		
	}
}
