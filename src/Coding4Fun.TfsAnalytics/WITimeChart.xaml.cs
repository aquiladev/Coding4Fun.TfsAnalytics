using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

using Coding4Fun.TfsAnalytics.Models;
using Coding4Fun.TfsAnalytics.Controllers;

namespace Coding4Fun.TfsAnalyticsPackage
{
	public partial class UsControl
	{
		private readonly ITimeController _controller;
		private IList<ChartWorkItem> _workItems;

		public UsControl(ITimeController controller)
		{
			_controller = controller;
			InitializeComponent();
		}

		public void Init(IResultsDocument resDocument, WorkItemStore workItemStore)
		{
			_workItems = _controller.GetChartItems(resDocument, workItemStore);
		}

		public void Render()
		{
			ChartsPanel.Children.Clear();
			foreach (var item in _workItems)
			{
				ChartsPanel.Children.Add(new Label { Content = item.Caption, FontSize = 20 });

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri(item.ChartUrl);
				bitmapImage.EndInit();

				ChartsPanel.Children.Add(new Image { Source = bitmapImage, Height = item.Size.Height });
			}
		}
	}
}
