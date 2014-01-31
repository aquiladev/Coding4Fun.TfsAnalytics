﻿using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace Coding4Fun.TfsAnalyticsPackage
{
	[Guid("f450fa5c-88c9-4cde-901d-588cbcf22d30")]
	public class WITimeWindow : ToolWindowPane
	{
		/// <summary>
		/// Standard constructor for the tool window.v
		/// </summary>
		public WITimeWindow() :
			base(null)
		{
			// Set the window title reading it from the resources.
			Caption = Resources.ToolWindowTitle;
			// Set the image that will appear on the tab of the window frame
			// when docked with an other window
			// The resource ID correspond to the one defined in the resx file
			// while the Index is the offset in the bitmap strip. Each image in
			// the strip being 16x16.
			BitmapResourceID = 301;
			BitmapIndex = 1;

			// This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
			// we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
			// the object returned by the Content property.
			base.Content = new UsControl();
		}
	}
}