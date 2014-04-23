using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

using Coding4Fun.TfsAnalytics.Controllers;
using Coding4Fun.TfsAnalytics.Models;
using Coding4Fun.TfsAnalytics.Proxies;

namespace Coding4Fun.TfsAnalyticsPackage
{
	public partial class UsControl
	{
		private readonly IChartController _controller;
		private IList<ChartWorkItem> _workItems;

		public UsControl(IChartController controller)
		{
			_controller = controller;
			InitializeComponent();
		}

		public void Init(IResultsDocument resDocument, IWorkItemStoreProxy storeProxy)
		{
			_workItems = _controller.GetChartItems(resDocument, storeProxy);
		}

		public void Render()
		{
			ChartsPanel.Children.Clear();
			foreach (var item in _workItems)
			{
				ChartsPanel.Children.Add(new Label { Content = item.Caption, FontSize = 20 });

				if (string.IsNullOrEmpty(item.ChartUrl))
			{
					ChartsPanel.Children.Add(new Label
				{
						Content = TfsAnalyticsPackage.Resources.TasksDontFound,
						FontSize = 16
					});
					continue;
				}

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri(item.ChartUrl);
				bitmapImage.EndInit();

				ChartsPanel.Children.Add(new Image { Source = bitmapImage, Height = item.Size.Height });
			}
		}
	}
}
