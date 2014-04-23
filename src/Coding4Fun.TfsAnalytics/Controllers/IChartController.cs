using System.Collections.Generic;
using Coding4Fun.TfsAnalytics.Proxies;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

using Coding4Fun.TfsAnalytics.Models;

namespace Coding4Fun.TfsAnalytics.Controllers
{
	public interface IChartController
	{
		List<ChartWorkItem> GetChartItems(IResultsDocument resDocument, IWorkItemStoreProxy storeProxy);
	}
}
