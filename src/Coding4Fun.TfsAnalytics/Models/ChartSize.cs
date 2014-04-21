using System;

namespace Coding4Fun.TfsAnalytics.Models
{
	public class ChartSize
	{
		private const int DefaultHeight = 330;
		private const int VisibleLines = 12;
		
		public int Width = 900;
		public int Height { get; private set; }

		public ChartSize(int taskCount)
		{
			Height = Math.Min((DefaultHeight / VisibleLines) * taskCount, DefaultHeight);
		}
	}
}
