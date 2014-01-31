﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Coding4Fun.TfsAnalyticsPackage
{
	public partial class UsControl : UserControl
	{
		public UsControl()
		{
			InitializeComponent();
		}

		public void ShowCharts(Dictionary<WorkItem, Dictionary<WorkItem, TimeSpan>> info)
		{
			ChartsPanel.Children.Clear();

			if (info.Count == 0)
			{
				return;
			}

			foreach (var item in info)
			{
				ChartsPanel.Children.Add(new Label { Content = string.Format("{0}: {1}", item.Key.Type.Name, item.Key.Title), FontSize = 20 });

				if (item.Value.Count == 0)
				{
					continue;
				}

				var sortedDic = item.Value.OrderBy(x => x.Value.TotalMinutes);
				var url = GenerateUrl(sortedDic);

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri(url);
				bitmapImage.EndInit();

				ChartsPanel.Children.Add(new Image { Source = bitmapImage, Height = 330 });
			}
		}

		private string GenerateUrl(IOrderedEnumerable<KeyValuePair<WorkItem, TimeSpan>> dic)
		{
			var url = "http://chart.apis.google.com/chart?cht=bhs&chs=900x330";
			url += "&chds=0," + dic.Max(x => x.Value.TotalMinutes);
			url += "&chd=t:" + string.Join(",", dic.Select(x => x.Value.TotalMinutes));
			url += "&chm=" + string.Join("|", dic.Select((x, index) => string.Format("t{0},,0,{1},18,,ls",
				x.Value.ToString("g") + " - " + x.Key.Title.Substring(0, Math.Min(x.Key.Title.Length, 30)) + "...",
				index)));

			return url;
		}
	}
}