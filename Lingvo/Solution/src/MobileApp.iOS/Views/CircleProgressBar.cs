using System;
using UIKit;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;

namespace Lingvo.MobileApp.iOS
{

	public class CircleProgressBar : UIView
	{
		public CAShapeLayer backgroundLayer;
		public CAShapeLayer strokeLayer;
		protected float lineWidth = 10.0f;
		protected float margin = 0.0f;
		protected int nestingLevel = 0;
		protected int progress = 0;
		protected int maxProgress = 100;
		protected double animationStartTime;
		protected UIColor progressColor = UIColor.Blue;
		protected bool muted = false;
		protected int size;


		public virtual int Size
		{
			get
			{
				return size;
			}
			set
			{
				Frame = new CGRect(Frame.X, Frame.Y, value, value);
				strokeLayer.RemoveFromSuperLayer();
				renderBackgroundLayer();
				strokeLayer = drawCircle(backgroundLayerColor, -100);
				strokeLayer.Hidden = muted;
				drawStroke(angle);
			}
		}
		protected UIColor backgroundLayerColor
		{
			get
			{
				nfloat red = 0.0f;
				nfloat green = 0.0f;
				nfloat blue = 0.0f;
				nfloat backgroundAlpha = 0.0f;
				progressColor.GetRGBA(out red, out green, out blue, out backgroundAlpha);
				return new UIColor(red, green, blue, 0.25f);
			}
		}
		public float LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
				strokeLayer.RemoveFromSuperLayer();
				renderBackgroundLayer();
				strokeLayer = drawCircle(backgroundLayerColor, -100);
				strokeLayer.Hidden = muted;
				drawStroke(angle);
			}
		}
		public virtual UIColor ProgressColor
		{
			get
			{
				return progressColor;
			}
			set
			{
				progressColor = value;
				strokeLayer.StrokeColor = value.CGColor;
		
				drawStroke(angle);
				render();
			}
		}
		/// <summary>
		/// Indicates if this ProgressBar is muted. If muted, the path stroke will not be visible
		/// </summary>
		/// <value>if progress bar is muted</value>
		public bool Muted
		{
			get
			{
				return muted;
			}
			set
			{
				muted = value;
				if (muted)
				{
					backgroundLayer.Hidden = true;
					strokeLayer.Hidden = true;
				}
				else
				{
					drawStroke(angle);
				}
			}
		}
		/// <summary>
		/// Level of nesting. The outer circle bar has a nesting of zero, the next progress bar has a nesting level of 1 and so on
		/// </summary>
		/// <value>The nesting level</value>
		public int NestingLevel
		{
			get
			{
				return nestingLevel;
			}
			set
			{
				nestingLevel = value >= 0 ? value : 0;
			}
		}

		private float radius
		{
			get
			{
				var outerWidthRadius = Frame.Width / 2 - lineWidth / 2 - 2 * (margin + nestingLevel * lineWidth / 2);
				var outerHeightRadius = Frame.Height / 2 - lineWidth / 2 - 2 * (margin + nestingLevel * lineWidth / 2);

				return (float)Math.Min(outerWidthRadius, outerHeightRadius);
			}
		}

		public int MaxProgress
		{
			get
			{
				return maxProgress;
			}
			set
			{
				maxProgress = value;
				drawStroke(angle);
			}
		}

		public int Progress
		{
			get
			{

				return progress;
			}
			set
			{
				progress = Math.Min(value, maxProgress);
				drawStroke(angle);
			
			}
		}

		/// <summary>
		/// The end angle of the circel fragment depending on the current progress
		/// </summary>
		/// <value>The end angle of the stroke layer path from 0 to 2*Math.PI</value>
		protected float angle
		{
			get
			{
				if (progress == maxProgress)
				{
					return (float)(2 * Math.PI);
				}
				float percentage = (float)progress / (float)maxProgress;
				return (float)(percentage * (2 * Math.PI));
			}
		}

		/// <summary>
		/// Draws the circle fragments
		/// </summary>
		/// <param name="endAngle">End angle.</param>
		public void drawStroke(float endAngle)
		{

			var startAngle = correctAngle(0.0f);
			var end = correctAngle(endAngle);
			var circleFragment = new UIBezierPath();

			circleFragment.AddArc(Center, radius, startAngle, end, true);
			strokeLayer.StrokeColor = progressColor.CGColor;
			strokeLayer.Path = circleFragment.CGPath;
			strokeLayer.SetNeedsDisplay();

			if (muted)
			{
				strokeLayer.Hidden = true;
				return;
			}

		}

		public CircleProgressBar(CGRect frame) : base(frame)
		{
			renderBackgroundLayer();
			strokeLayer = drawCircle(backgroundLayerColor, -100);
			strokeLayer.Hidden = muted;
		}
		/// <summary>
		/// Renders the entire view
		/// </summary>
		public void render()
		{
			BackgroundColor = UIColor.Clear;
			strokeLayer.RemoveFromSuperLayer();

			renderBackgroundLayer();
			strokeLayer = drawCircle(backgroundLayerColor, -100);
			strokeLayer.Hidden = muted;
			drawStroke(angle);
		}
		/// <summary>
		/// Renders a circle in a CAShapeLayer
		/// </summary>
		/// <returns>The CAShapeLayer object</returns>
		/// <param name="fillColor">The color of the circle</param>
		/// <param name="zPosition">z position of the circle</param>
		protected CAShapeLayer drawCircle(UIColor fillColor, int zPosition)
		{
			var circleLayer =  drawArc(fillColor, 0.0f, (float)(2.0 * Math.PI), zPosition);

			return circleLayer;
		}

		/// <summary>
		/// Drawa a circle fragment from a start angle to an end angle
		/// </summary>
		/// <returns>A CAShapeLayer with a circular path</returns>
		/// <param name="fillColor">The arc's fill color</param>
		/// <param name="startAngle">The arc's start angle</param>
		/// <param name="endAngle">The arc's end angle</param>
		/// <param name="zPosition">The arc's z position</param>
		private CAShapeLayer drawArc(UIColor fillColor, float startAngle, float endAngle, int zPosition)
		{
			var adjustedStartAngle = correctAngle(startAngle);
			var adjustedEndAngle = correctAngle(endAngle);

			var circleLayer = new CAShapeLayer()
			{
				LineWidth = (nfloat)lineWidth,
				FillColor = UIColor.Clear.CGColor,
				StrokeColor = fillColor.CGColor,
				ZPosition = zPosition,
				Frame = Bounds
			};
			var path = new UIBezierPath();
			path.AddArc(Center, radius, adjustedStartAngle, adjustedEndAngle, true);
			circleLayer.Path = path.CGPath;
			Layer.AddSublayer(circleLayer);
			return circleLayer;
		}


		/// <summary>
		/// Adds offset to a circle so that drawing can be started at 12 o' clock
		/// CoreGraphics starts its rendering at 3 o'clock.
		/// </summary>
		/// <returns>The angle with the offset</returns>
		/// <param name="oldAngle">The angle to be corrected</param>
		private float correctAngle(float oldAngle)
		{
			return oldAngle - (float)(Math.PI / 2.0);
		}
		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);
			if (layer == Layer)
			{
				backgroundLayer.Frame = Bounds;
				strokeLayer.Frame = Bounds;
				foreach (var l in layer.Sublayers)
				{
					l.Frame = Bounds;
				}
			}
		}
		/// <summary>
		/// Render a background layer with a low alpha value
		/// </summary>
		private void renderBackgroundLayer()
		{
			backgroundLayer?.RemoveFromSuperLayer();
			backgroundLayer = drawCircle(backgroundLayerColor, -100);
			backgroundLayer.Hidden = muted;
		}
	}
}