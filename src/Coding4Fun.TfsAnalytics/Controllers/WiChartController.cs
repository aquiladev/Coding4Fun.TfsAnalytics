using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Coding4Fun.TfsAnalytics.Proxies;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

using Coding4Fun.TfsAnalytics.Models;

namespace Coding4Fun.TfsAnalytics.Controllers
{
	public class WiChartController : IChartController
	{
		private const string ChartApiUrl = "http://chart.apis.google.com";

		public List<ChartWorkItem> GetChartItems(IResultsDocument resDocument, IWorkItemStoreProxy storeProxy)
		{
			return (from item in GetSelectedItems(resDocument)
					let taskIds = GetTasks(item, storeProxy)
					let tasks = (from int id in taskIds select storeProxy.GetWorkItem(id))
						.ToDictionary(task => task, GetElapsedTime).OrderBy(x => x.Value.TotalMinutes)
					let size = new ChartSize(tasks.Count())
					select new ChartWorkItem
					{
						Caption = string.Format("{0}: {1}", item.Type.Name, item.Title),
						Size = size,
						ChartUrl = GenerateUrl(tasks, size)
					}).ToList();
		}

		private IEnumerable<WorkItem> GetSelectedItems(IResultsDocument resDocument)
		{
			WorkItem[] selectedItems = null;
			if (resDocument != null && resDocument.SelectedItemIds.Length > 0)
			{
				selectedItems = new WorkItem[resDocument.SelectedItemIds.Length];
				var resultsProvider = resDocument.ResultListDataProvider;
				var idx = 0;

				foreach (var workItemId in resDocument.SelectedItemIds)
				{
					selectedItems[idx++] = resultsProvider.GetItem(resultsProvider.GetItemIndex(workItemId));
				}
			}
			return selectedItems;
		}

		public IEnumerable<int> GetTasks(WorkItem item, IWorkItemStoreProxy storeProxy)
		{
			var linkType = storeProxy.GetHierarchyLinkType();
			return item.WorkItemLinks.OfType<WorkItemLink>()
				.Where(x => x.LinkTypeEnd.Id == linkType.Id)
				.Select(x => x.TargetId);
		}

		private TimeSpan GetElapsedTime(WorkItem item)
		{
			const string inProgressState = "In Progress";
			var isTaskInProgress = false;
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

		private string GenerateUrl(IOrderedEnumerable<KeyValuePair<WorkItem, TimeSpan>> tasks, ChartSize size)
		{
			if (!tasks.Any())
			{
				return string.Empty;
			}

			var culture = new CultureInfo("en-US");
			var sizes = string.Format("chs={0}x{1}", size.Width, size.Height);
			var range = string.Format("chds={0},{1}", 0, tasks.Max(x => x.Value.TotalMinutes).ToString(culture));
			var values = string.Format("chd=t:{0}", string.Join(",", tasks.Select(x => x.Value.TotalMinutes.ToString(culture))));
			var settings = string.Format("chm={0}",
				string.Join("|", tasks.Select((x, index) => string.Format("t{0},,0,{1},18,,ls",
					x.Value.ToString("g", culture) + " - " + GetShortTitle(x.Key.Title),
					index))));

			return string.Format("{0}/chart?cht=bhs&{1}&{2}&{3}&{4}", ChartApiUrl, sizes, range, values, settings);
		}

		private string GetShortTitle(string value)
		{
			const int maxLength = 30;
			return value.Length > maxLength
				? value.Substring(0, maxLength) + "..."
				: value;
		}
	}
}
