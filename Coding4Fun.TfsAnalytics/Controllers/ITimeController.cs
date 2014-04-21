using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

using Coding4Fun.TfsAnalytics.Models;

namespace Coding4Fun.TfsAnalytics.Controllers
{
	public interface ITimeController
	{
		List<ChartWorkItem> GetChartItems(IResultsDocument resDocument, WorkItemStore workItemStore);
	}
}
