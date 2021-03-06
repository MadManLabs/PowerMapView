﻿using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace PowerMapView.Views.Controls
{
	[TemplatePart(Name = "PART_CirclePathBox", Type = typeof(Path))]
	[TemplatePart(Name = "PART_CirclePathFigure", Type = typeof(PathFigure))]
	[TemplatePart(Name = "PART_CircleArcSegment", Type = typeof(ArcSegment))]
	public class CircularProgressBar : ProgressBar
	{
		#region Template Part

		private Path circlePathBox;
		private PathFigure circlePathFigure;
		private ArcSegment circleArcSegment;

		#endregion

		#region Property

		public double Radius // Outer radius
		{
			get { return (double)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}
		public static readonly DependencyProperty RadiusProperty =
			DependencyProperty.Register(
				"Radius",
				typeof(double),
				typeof(CircularProgressBar),
				new PropertyMetadata(
					25D,
					OnPropertyChanged));

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}
		public static readonly DependencyProperty StrokeThicknessProperty =
			DependencyProperty.Register(
				"StrokeThickness",
				typeof(double),
				typeof(CircularProgressBar),
				new PropertyMetadata(
					10D,
					OnPropertyChanged));

		public Brush ArcSegmentColor
		{
			get { return (Brush)GetValue(ArcSegmentColorProperty); }
			set { SetValue(ArcSegmentColorProperty, value); }
		}
		public static readonly DependencyProperty ArcSegmentColorProperty =
			DependencyProperty.Register(
				"ArcSegmentColor",
				typeof(Brush),
				typeof(CircularProgressBar),
				new PropertyMetadata(
					null, // TODO: Check later
					OnPropertyChanged));

		public Brush RingSegmentColor
		{
			get { return (Brush)GetValue(RingSegmentColorProperty); }
			set { SetValue(RingSegmentColorProperty, value); }
		}
		public static readonly DependencyProperty RingSegmentColorProperty =
			DependencyProperty.Register(
				"RingSegmentColor",
				typeof(Brush),
				typeof(CircularProgressBar),
				new PropertyMetadata(
					null, // TODO: Check later
					OnPropertyChanged));

		public double RingSegmentOpacity
		{
			get { return (double)GetValue(RingSegmentOpacityProperty); }
			set { SetValue(RingSegmentOpacityProperty, value); }
		}
		public static readonly DependencyProperty RingSegmentOpacityProperty =
			DependencyProperty.Register(
				"RingSegmentOpacity",
				typeof(double),
				typeof(CircularProgressBar),
				new PropertyMetadata(
					0D,
					OnPropertyChanged));

		public double Percentage
		{
			get { return (double)GetValue(PercentageProperty); }
			set { SetValue(PercentageProperty, value); }
		}
		public static readonly DependencyProperty PercentageProperty =
			DependencyProperty.Register(
				"Percentage",
				typeof(double),
				typeof(CircularProgressBar),
				new PropertyMetadata(
					60D, // Sample percentage
					OnPercentageChanged));

		public double Angle
		{
			get { return (double)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}
		public static readonly DependencyProperty AngleProperty =
			DependencyProperty.Register(
				"Angle",
				typeof(double),
				typeof(CircularProgressBar),
				new PropertyMetadata(
					216D, // Sample angle
					OnAngleChanged));

		#endregion

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			circlePathBox = this.GetTemplateChild("PART_CirclePathBox") as Path;
			circlePathFigure = this.GetTemplateChild("PART_CirclePathFigure") as PathFigure;
			circleArcSegment = this.GetTemplateChild("PART_CircleArcSegment") as ArcSegment;

			RenderArc();
		}

		private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var circle = (CircularProgressBar)d;

			// Alternate way for validating new value
			if (circle.Percentage < 0D)
				circle.Percentage = 0D;
			else if (100D < circle.Percentage)
				circle.Percentage = 360D;

			circle.Angle = (circle.Percentage * 360D) / 100D; // This will invoke OnPropertyChanged.
		}

		private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var circle = (CircularProgressBar)d;

			// Alternate way for validating new value
			if (circle.Angle < 0D)
				circle.Angle = 0D;
			else if (360D < circle.Angle)
				circle.Angle = 360D;

			circle.RenderArc();
		}

		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var circle = (CircularProgressBar)d;
			circle.RenderArc();
		}

		private void RenderArc()
		{
			if ((circlePathBox == null) || (circlePathFigure == null) || (circleArcSegment == null))
				return;

			circlePathBox.Width = Radius * 2;
			circlePathBox.Height = Radius * 2;

			var pathRadius = Radius - StrokeThickness / 2;

			var startPoint = new Point(pathRadius, 0D);

			var endPoint = GetCartesianCoordinate(Angle, pathRadius);
			endPoint.X += pathRadius;
			endPoint.Y += pathRadius;

			// Check if distance between start point and end point is very short (when angle is close to
			// 360) and if so, adjust end point because in such case arc segment will not be rendered.
			if ((Math.Abs(endPoint.X - startPoint.X) < 0.01) &&
				(Math.Abs(endPoint.Y - startPoint.Y) < 0.01))
				endPoint.X -= 0.01;

			circlePathFigure.StartPoint = startPoint;

			circleArcSegment.Point = endPoint;
			circleArcSegment.Size = new Size(pathRadius, pathRadius);
			circleArcSegment.IsLargeArc = (Angle > 180D);

			circlePathBox.RenderTransform = new TranslateTransform
			{
				X = StrokeThickness / 2,
				Y = StrokeThickness / 2
			};
		}

		private static Point GetCartesianCoordinate(double angle, double radius)
		{
			// Convert from degree to radian.
			var angleRadian = (Math.PI / 180D) * (angle - 90D);

			var x = radius * Math.Cos(angleRadian);
			var y = radius * Math.Sin(angleRadian);

			return new Point(x, y);
		}
	}
}