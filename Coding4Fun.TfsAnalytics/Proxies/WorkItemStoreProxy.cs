using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Coding4Fun.TfsAnalytics.Proxies
{
	public class WorkItemStoreProxy : IWorkItemStoreProxy
	{
		private readonly WorkItemStore _workItemStore;

		public WorkItemStoreProxy(WorkItemStore workItemStore)
		{
			_workItemStore = workItemStore;
		}

		public WorkItem GetWorkItem(int id)
		{
			return _workItemStore.GetWorkItem(id);
		}

		public WorkItemLinkTypeEnd GetHierarchyLinkType()
		{
			return _workItemStore.WorkItemLinkTypes.LinkTypeEnds["System.LinkTypes.Hierarchy-Forward"];
		}
	}
}
