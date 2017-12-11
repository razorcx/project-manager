using System;
using System.Diagnostics;
using System.Windows;
using RazorCXProjectManager.Common;

namespace RazorCXProjectManager
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			try
			{
				if (!ModelHelper.GetConnectionStatus()) return;

				InitializeComponent();

				DataGridMainBeams.CanUserAddRows = false;
				DataGridSecondaryBeams.CanUserAddRows = false;
				DataGridComponentBeams.CanUserAddRows = false;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message + ex.InnerException + ex.StackTrace);
			}
		}
	}
}
