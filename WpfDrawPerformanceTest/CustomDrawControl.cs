using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfDrawPerformanceTest
{
	class CustomDrawControl : FrameworkElement
	{
		public DrawType DrawType
		{
			get => (DrawType)GetValue(DrawTypeProperty);
			set => SetValue(DrawTypeProperty, value);
		}
		public static readonly DependencyProperty DrawTypeProperty = DependencyProperty.Register("DrawType", typeof(DrawType), typeof(CustomDrawControl), new PropertyMetadata(WpfDrawPerformanceTest.DrawType.None));



		public double Fps
		{
			get => (double)GetValue(FpsProperty);
			set => SetValue(FpsProperty, value);
		}
		// Using a DependencyProperty as the backing store for Fps.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FpsProperty = DependencyProperty.Register("Fps", typeof(double), typeof(CustomDrawControl), new PropertyMetadata(0.0));



		public CustomDrawControl()
		{
			var list = new List<Particle>();
			for (var i = 0; i < ParticalCount; i++)
			{
				var p = new Particle();
				p.Init(_random, 1000, 1000);
				p.Age = _random.Next(MaxAge);
				list.Add(p);
			}
			_particles = list.ToArray();

			_frameRateCalculateTimer = new Timer(CalculateFrameRate);
			_frameRateStopwatch.Start();
			_frameRateCalculateTimer.Change(1000, 1000);
		}

		private uint _counter = 0;
		private uint _previousCounter = 0;
		const int ParticalCount = 10000;
		const int MaxAge = 100;
		private readonly Particle[] _particles;
		private readonly Random _random = new Random();
		private readonly Stopwatch _frameRateStopwatch = new Stopwatch();
		private readonly Timer _frameRateCalculateTimer;

		private void CalculateFrameRate(object state)
		{
			var frameCount = _counter - _previousCounter;
			_previousCounter = _counter;
			var fps = ((double)frameCount / _frameRateStopwatch.ElapsedMilliseconds) * 1000;
			System.Diagnostics.Debug.WriteLine($"FPS:{fps}");
			_frameRateStopwatch.Restart();
			Dispatcher.BeginInvoke((Action)(() => Fps = fps));
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			switch (DrawType)
			{
			case DrawType.NotFreeze:
				if (ParticalCount > 3000) break;    // It's very slow
				NotFreezeRender(drawingContext);
				break;
			case DrawType.Freeze:
				FreezeRender(drawingContext);
				break;
			case DrawType.Grouping:
				GroupingRender(drawingContext);
				break;
			case DrawType.BackingStore:
				this.BackingStoreRender(drawingContext);
				break;
			}


			_counter++;
			Task.Run(async () =>
			{
				await Task.Delay(1);
				await Dispatcher.BeginInvoke((Action)this.InvalidateVisual);
			});
		}


		#region DrawType.NotFreeze
		private void NotFreezeRender(DrawingContext drawingContext)
		{
			#region Create Palette
			var colorList = new List<Color>();
			for (var i = 0; i < 8; i++)
			{
				colorList.Add(GetColor((_counter + i * 8) % 360));
			}
			var penList = colorList.Select(c => new Pen(new SolidColorBrush(c), 2)).ToArray();
			#endregion

			var counter = 0;
			foreach (var p in _particles)
			{
				if (p.Age > MaxAge) p.Init(_random, this.ActualWidth, this.ActualHeight);
				p.X2 = p.X1 + (_random.NextDouble() - .5) * 2;
				p.Y2 = p.Y1 + (_random.NextDouble() - .5) * 2;
				p.Pallete = (counter++) % penList.Length;
				drawingContext.DrawLine(penList[p.Pallete], new Point(p.X1, p.Y1), new Point(p.X2, p.Y2));
				p.X1 = p.X2;
				p.Y1 = p.Y2;
				p.Age++;
			}
		}
		#endregion

		#region DrawType.Freeze
		private void FreezeRender(DrawingContext drawingContext)
		{
			#region Create Palette with Freeze
			var colorList = new List<Color>();
			for (var i = 0; i < 8; i++)
			{
				colorList.Add(GetColor((_counter + i * 8) % 360));
			}
			var penList = colorList.Select(c =>
			{
				var brush = new SolidColorBrush(c);
				if (brush.CanFreeze) brush.Freeze();
				var pen = new Pen(brush, 2);
				if (pen.CanFreeze) pen.Freeze();
				return pen;
			}).ToArray();
			#endregion

			var counter = 0;
			foreach (var p in _particles)
			{
				if (p.Age > MaxAge) p.Init(_random, this.ActualWidth, this.ActualHeight);
				p.X2 = p.X1 + (_random.NextDouble() - .5) * 2;
				p.Y2 = p.Y1 + (_random.NextDouble() - .5) * 2;
				p.Pallete = (counter++) % penList.Length;
				drawingContext.DrawLine(penList[p.Pallete], new Point(p.X1, p.Y1), new Point(p.X2, p.Y2));
				p.X1 = p.X2;
				p.Y1 = p.Y2;
				p.Age++;
			}
		}
		#endregion

		#region DrawType.Grouping
		private void GroupingRender(DrawingContext drawingContext)
		{
			#region Create Palette with Freeze
			var colorList = new List<Color>();
			for (var i = 0; i < 8; i++)
			{
				colorList.Add(GetColor((_counter + i * 8) % 360));
			}
			var penList = colorList.Select(c =>
			{
				var brush = new SolidColorBrush(c);
				if (brush.CanFreeze) brush.Freeze();
				var pen = new Pen(brush, 2);
				if (pen.CanFreeze) pen.Freeze();
				return pen;
			}).ToArray();
			#endregion

			var counter = 0;
			foreach (var p in _particles)
			{
				if (p.Age > MaxAge) p.Init(_random, this.ActualWidth, this.ActualHeight);
				p.X2 = p.X1 + (_random.NextDouble() - .5) * 2;
				p.Y2 = p.Y1 + (_random.NextDouble() - .5) * 2;
				p.Pallete = (counter++) % penList.Length;
				drawingContext.DrawLine(penList[p.Pallete], new Point(p.X1, p.Y1), new Point(p.X2, p.Y2));
				p.X1 = p.X2;
				p.Y1 = p.Y2;
				p.Age++;
			}

			foreach (var g in _particles.GroupBy(p => p.Pallete))
			{
				var geometry = new StreamGeometry();
				using (var context = geometry.Open())
				{
					foreach (var p in g)
					{
						context.BeginFigure(new Point(p.X1, p.Y1), false, false);
						context.LineTo(new Point(p.X2, p.Y2), true, false);
						p.X1 = p.X2;
						p.Y1 = p.Y2;
					}
				}
				drawingContext.DrawGeometry(null, penList[g.Key], geometry);
			}
		}
		#endregion

		#region DrawType.BackingStore
		readonly DrawingGroup _backingStore = new DrawingGroup();
		private void BackingStoreRender(DrawingContext drawingContext)
		{
			#region Create Palette with Freeze
			var colorList = new List<Color>();
			for (var i = 0; i < 8; i++)
			{
				colorList.Add(GetColor((_counter + i * 8) % 360));
			}
			var penList = colorList.Select(c =>
			{
				var brush = new SolidColorBrush(c);
				if (brush.CanFreeze) brush.Freeze();
				var pen = new Pen(brush, 2);
				if (pen.CanFreeze) pen.Freeze();
				return pen;
			}).ToArray();
			#endregion

			var context = _backingStore.Open();

			var counter = 0;
			foreach (var p in _particles)
			{
				if (p.Age > MaxAge) p.Init(_random, this.ActualWidth, this.ActualHeight);
				p.X2 = p.X1 + (_random.NextDouble() - .5) * 2;
				p.Y2 = p.Y1 + (_random.NextDouble() - .5) * 2;
				p.Pallete = (counter++) % penList.Length;
				context.DrawLine(penList[p.Pallete], new Point(p.X1, p.Y1), new Point(p.X2, p.Y2));
				p.X1 = p.X2;
				p.Y1 = p.Y2;
				p.Age++;
			}

			context.Close();
			drawingContext.DrawDrawing(_backingStore);
		}
		#endregion



		private static Color GetColor(long h)
		{
			var ht = (int)(h * 6);
			var d = ((h * 6) % 360);
			byte t1 = 0;
			var t2 = (byte)(255 * (1 - d / 360));
			var t3 = (byte)(255 * ((1 - (360 - d) / 360)));

			switch (ht / 360)
			{
			case 0:
				return Color.FromArgb(255, 255, t3, t1);
			case 1:
				return Color.FromArgb(255, t2, 255, t1);
			case 2:
				return Color.FromArgb(255, t1, 255, t3);
			case 3:
				return Color.FromArgb(255, t1, t2, 255);
			case 4:
				return Color.FromArgb(255, t3, t1, 255);
			default:
				return Color.FromArgb(255, 255, t1, t2);
			}
		}

		class Particle
		{
			public double X1 { get; set; }
			public double Y1 { get; set; }

			public double X2 { get; set; }
			public double Y2 { get; set; }

			public int Pallete { get; set; }
			public int Age { get; set; }

			public void Init(Random r, double width, double height)
			{
				X1 = r.NextDouble() * width;
				Y1 = r.NextDouble() * height;
				Age = 0;
			}
		}
	}
}