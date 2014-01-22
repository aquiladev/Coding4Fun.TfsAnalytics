using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Coding4Fun.TfsAnalyticsPackage
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(GuidList.guidCoding4Fun_TfsAnalyticsPkgString)]
	[ProvideAutoLoad("{e13eedef-b531-4afe-9725-28a69fa4f896}")]
	public sealed class Coding4FunTfsAnalyticsPackage : Package
	{
		public Coding4FunTfsAnalyticsPackage()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this));
		}

		#region Package Members

		protected override void Initialize()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
			base.Initialize();

			// Add our command handlers for menu (commands must exist in the .vsct file)
			var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs)
			{
				var menuAAAID = new CommandID(GuidList.guidCoding4Fun_TfsAnalyticsCmdSet, (int)PkgCmdIDList.cmdidAAAQuery);
				var menuAAAItem = new MenuCommand(MenuItemCallback, menuAAAID);
				mcs.AddCommand(menuAAAItem);
			}
		}

		#endregion

		private void MenuItemCallback(object sender, EventArgs e)
		{
			var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
			Guid clsid = Guid.Empty;
			int result;
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
					   0,
					   ref clsid,
					   "Coding4Fun.TfsAnalytics",
					   string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
					   string.Empty,
					   0,
					   OLEMSGBUTTON.OLEMSGBUTTON_OK,
					   OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
					   OLEMSGICON.OLEMSGICON_INFO,
					   0,        // false
					   out result));
		}

	}
}
