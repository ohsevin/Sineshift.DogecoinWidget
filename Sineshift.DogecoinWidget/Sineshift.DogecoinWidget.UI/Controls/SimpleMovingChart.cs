﻿using Sineshift.DogecoinWidget.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sineshift.DogecoinWidget.UI
{
	public class SimpleMovingChart : ItemsControl
	{
		Canvas canvasPart;

		public SimpleMovingChart()
		{
			
		}

		#region DP DollarBrush
		public Brush DollarBrush
		{
			get { return (Brush)GetValue(DollarBrushProperty); }
			set { SetValue(DollarBrushProperty, value); }
		}

		public static readonly DependencyProperty DollarBrushProperty =
			DependencyProperty.Register("DollarBrush", typeof(Brush), typeof(SimpleMovingChart), new PropertyMetadata(default(Brush), (sender, e) => (sender as SimpleMovingChart).OnDollarBrushChanged((Brush)e.OldValue, (Brush)e.NewValue)));

		protected void OnDollarBrushChanged(Brush oldValue, Brush newValue)
		{

		}
		#endregion

		#region DP BitcoinBrush
		public Brush BitcoinBrush
		{
			get { return (Brush)GetValue(BitcoinBrushProperty); }
			set { SetValue(BitcoinBrushProperty, value); }
		}

		public static readonly DependencyProperty BitcoinBrushProperty =
			DependencyProperty.Register("BitcoinBrush", typeof(Brush), typeof(SimpleMovingChart), new PropertyMetadata(default(Brush), (sender, e) => (sender as SimpleMovingChart).OnBitcoinBrushChanged((Brush)e.OldValue, (Brush)e.NewValue)));

		protected void OnBitcoinBrushChanged(Brush oldValue, Brush newValue)
		{

		}
		#endregion

		#region DP MaxItemCount
		public int MaxItemCount
		{
			get { return (int)GetValue(MaxItemCountProperty); }
			set { SetValue(MaxItemCountProperty, value); }
		}

		public static readonly DependencyProperty MaxItemCountProperty =
			DependencyProperty.Register("MaxItemCount", typeof(int), typeof(SimpleMovingChart), new PropertyMetadata(default(int), (sender, e) => (sender as SimpleMovingChart).OnMaxItemCountChanged((int)e.OldValue, (int)e.NewValue)));

		protected void OnMaxItemCountChanged(int oldValue, int newValue)
		{
			RedrawChart();
		}
		#endregion



		#region DP LineStyle
		public Style LineStyle
		{
			get { return (Style)GetValue(LineStyleProperty); }
			set { SetValue(LineStyleProperty, value); }
		}

		public static readonly DependencyProperty LineStyleProperty =
			DependencyProperty.Register("LineStyle", typeof(Style), typeof(SimpleMovingChart), new PropertyMetadata(default(Style), (sender, e) => (sender as SimpleMovingChart).OnLineStyleChanged((Style)e.OldValue, (Style)e.NewValue)));

		protected void OnLineStyleChanged(Style oldValue, Style newValue)
		{

		}
		#endregion



		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			canvasPart = GetTemplateChild("PART_Canvas") as Canvas;
			RedrawChart();
		}

		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			base.OnItemsSourceChanged(oldValue, newValue);
			RedrawChart();
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
			RedrawChart();
		}

		private void RedrawChart()
		{
			if (canvasPart == null || ItemsSource == null)
			{
				return;
			}

			var pairs = Items.OfType<BitcoinDollarPair>().ToList();

			if (pairs.None())
			{
				return;
			}

			var btcPrices = pairs.Select(p => p.PriceBTC).ToList();
			var usdPrices = pairs.Select(p => p.PriceUSD).ToList();

			var btcLine = GetLineFromPrices(btcPrices);
			btcLine.Stroke = BitcoinBrush;

			var usdLine = GetLineFromPrices(usdPrices);
			usdLine.Stroke = DollarBrush;

			canvasPart.Children.Clear();
			canvasPart.Children.Add(btcLine);
			canvasPart.Children.Add(usdLine);
		}

		private Polyline GetLineFromPrices(List<double> prices)
		{
			var min = prices.Min();
			var max = prices.Max();
			var span = min.AlmostEquals(max) ? max : max - min;
			var widthPerPrice = canvasPart.ActualWidth / MaxItemCount;
			var startX = canvasPart.ActualWidth - prices.Count * widthPerPrice;

			var polyline = new Polyline()
			{
				Style = LineStyle
			};

			foreach(var price in prices)
			{
				var x = startX + prices.IndexOf(price) * widthPerPrice;
				var relativeY = (price - min) / span;
				var y = canvasPart.ActualHeight * relativeY;

				polyline.Points.Add(new Point(x, y));
			}

			return polyline;
		}
	}
}
