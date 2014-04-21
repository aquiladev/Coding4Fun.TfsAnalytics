using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Coding4Fun.TfsAnalytics.Proxies
{
	public interface IWorkItemStoreProxy
	{
		WorkItem GetWorkItem(int id);
		WorkItemLinkTypeEnd GetHierarchyLinkType();
	}
}
