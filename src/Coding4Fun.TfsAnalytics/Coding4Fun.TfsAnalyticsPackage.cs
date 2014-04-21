using EnvDTE;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

using Coding4Fun.TfsAnalytics.Proxies;

namespace Coding4Fun.TfsAnalyticsPackage
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideToolWindow(typeof(WITimeWindow))]
	[Guid(GuidList.guidCoding4Fun_TfsAnalyticsPkgString)]
	[ProvideAutoLoad("{e13eedef-b531-4afe-9725-28a69fa4f896}")]
	public sealed class Coding4FunTfsAnalyticsPackage : Package
	{
		private DocumentService _docService;
		public DocumentService DocService
		{
			get
			{
				if (_docService != null)
				{
					return _docService;
				}

				_docService = (DocumentService)GetService(typeof(DocumentService));
				return _docService;
			}
		}

		private DTE _dte;
		public DTE Dte
		{
			get
			{
				if (_dte != null)
				{
					return _dte;
				}

				_dte = GetService(typeof(DTE)) as DTE;
				return _dte;
			}
		}

		private WorkItemStore _workItemStore;
		public WorkItemStore WorkItemStore
		{
			get
			{
				if (_workItemStore != null)
				{
					return _workItemStore;
				}

				var ext = Dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
				var tfs = new TeamFoundationServer(ext.ActiveProjectContext.DomainUri);
				_workItemStore = tfs.TfsTeamProjectCollection.GetService<WorkItemStore>();
				return _workItemStore;
			}
		}

		public Coding4FunTfsAnalyticsPackage()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this));
		}

		#region Package Members

		protected override void Initialize()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
			base.Initialize();

			var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs)
			{
				var cmdidShowTimelineId = new CommandID(GuidList.guidCoding4Fun_TfsAnalyticsCmdSet, (int)PkgCmdIdList.CmdidShowTimeline);
				var cmdidShowTimelineItem = new MenuCommand(MenuItemCallback, cmdidShowTimelineId);
				mcs.AddCommand(cmdidShowTimelineItem);
			}
		}

		#endregion

		private void MenuItemCallback(object sender, EventArgs e)
		{
			var window = FindToolWindow(typeof(WITimeWindow), 0, true);
			if ((null == window) || (null == window.Frame))
			{
				throw new NotSupportedException(Resources.CanNotCreateWindow);
			}
			var control = window.Content as UsControl;
			if (control == null)
			{
				return;
			}

			var windowFrame = (IVsWindowFrame) window.Frame;
			windowFrame.Show();
			control.Init(GetResultsDocument(), new WorkItemStoreProxy(WorkItemStore));
			control.Render();
		}

		private readonly object _lockToken = new object();

		private IResultsDocument GetResultsDocument()
		{
			if (Dte.ActiveDocument == null)
				return null;

			var doc = DocService.FindDocument(Dte.ActiveDocument.FullName, _lockToken);
			if (doc != null)
			{
				doc.Release(_lockToken);
			}

			return (IResultsDocument)doc;
		}
	}
}
