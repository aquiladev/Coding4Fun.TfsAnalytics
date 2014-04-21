using Coding4Fun.TfsAnalytics.Controllers;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace Coding4Fun.TfsAnalyticsPackage
{
	[Guid("f450fa5c-88c9-4cde-901d-588cbcf22d30")]
	public class WITimeWindow : ToolWindowPane
	{
		public WITimeWindow() :
			base(null)
		{
			Caption = Resources.ToolWindowTitle;
			BitmapResourceID = 301;
			BitmapIndex = 1;
			base.Content = new UsControl(new WiChartController());
		}
	}
}
