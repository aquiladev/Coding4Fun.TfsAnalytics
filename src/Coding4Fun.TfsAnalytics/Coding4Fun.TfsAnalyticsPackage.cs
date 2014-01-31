using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

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
			var info = new Dictionary<WorkItem, Dictionary<WorkItem, TimeSpan>>();
			var selectedItems = GetSelectedWorkItems();
			foreach (var item in selectedItems)
			{
				var taskIds = RetrieveTasks(item);
				var pbiInfo = (from int id in taskIds select GetWorkItemDetails(id))
					.ToDictionary(taskInfo => taskInfo, GetElapsedTime);
				info.Add(item, pbiInfo);
			}

			ToolWindowPane window = FindToolWindow(typeof(WITimeWindow), 0, true);
			if ((null == window) || (null == window.Frame))
			{
				throw new NotSupportedException(Resources.CanNotCreateWindow);
			}
			var control = window.Content as UsControl;
			if (control != null)
			{
				var windowFrame = (IVsWindowFrame) window.Frame;
				//Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
				windowFrame.Show();
				control.ShowCharts(info);
			}
		}

		private IEnumerable<WorkItem> GetSelectedWorkItems()
		{
			var resultsDoc = GetActiveResultsDocument();
			WorkItem[] selectedItems = null;
			if (resultsDoc != null && resultsDoc.SelectedItemIds.Length > 0)
			{
				selectedItems = new WorkItem[resultsDoc.SelectedItemIds.Length];
				var resultsProvider = resultsDoc.ResultListDataProvider;
				int idx = 0;

				foreach (int workItemId in resultsDoc.SelectedItemIds)
				{
					selectedItems[idx++] = resultsProvider.GetItem(resultsProvider.GetItemIndex(workItemId));
				}
			}
			return selectedItems;
		}

		private readonly object _lockToken = new object();

		private IResultsDocument GetActiveResultsDocument()
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

		private WorkItem GetWorkItemDetails(int id)
		{
			return WorkItemStore.GetWorkItem(id);
		}

		public IEnumerable<int> RetrieveTasks(WorkItem item)
		{
			var linkType = WorkItemStore.WorkItemLinkTypes.LinkTypeEnds["System.LinkTypes.Hierarchy-Forward"];
			return item.WorkItemLinks.OfType<WorkItemLink>()
				.Where(x => x.LinkTypeEnd.Id == linkType.Id)
				.Select(x => x.TargetId);
		}

		private TimeSpan GetElapsedTime(WorkItem item)
		{
			const string inProgressState = "In Progress";
			bool isTaskInProgress = false;
			var startDate = new DateTime();
			var elapsedTime = new TimeSpan();
			foreach (Revision revision in item.Revisions)
			{
				var stateField = revision.Fields["State"];
				if (!stateField.OriginalValue.Equals(inProgressState)
					&& stateField.Value.Equals(inProgressState))
				{
					isTaskInProgress = true;
					startDate = (DateTime)revision.Fields["Changed Date"].Value;
					continue;
				}
				if (stateField.OriginalValue.Equals(inProgressState)
					&& !stateField.Value.Equals(inProgressState))
				{
					isTaskInProgress = false;
					var endDate = (DateTime)revision.Fields["Changed Date"].Value;
					elapsedTime = elapsedTime.Add(endDate.Subtract(startDate));
				}
			}

			if (isTaskInProgress)
			{
				elapsedTime = elapsedTime.Add(DateTime.Now.Subtract(startDate));
			}

			return elapsedTime;
		}
	}
}
