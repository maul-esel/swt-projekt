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
					progress = 0;
					drawStroke(0);
				}
				else
				{
					drawStroke(angle);
				}
			}
		}
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

		//the angle starting from 12 o'clock to the current progress value
		//used for arc rendering
		protected float angle
		{
			get
			{
				if (progress == maxProgress)
				{
					return (float)(2 * Math.PI);
				}
				var modValue = progress % maxProgress;
				float percentage = (float)modValue / (float)maxProgress;
				return (float)(percentage * (2 * Math.PI));
			}
		}

		//render arc fragments
		public void drawStroke(float endAngle)
		{
			var startAngle = correctAngle(0.0f);
			var end = correctAngle(endAngle);
			var circleFragment = new UIBezierPath();

			circleFragment.AddArc(Center, radius, startAngle, end, true);
			strokeLayer.StrokeColor = progressColor.CGColor;
			strokeLayer.Path = circleFragment.CGPath;
			strokeLayer.SetNeedsDisplay();

		}

		public CircleProgressBar(CGRect frame) : base(frame)
		{
			renderBackgroundLayer();
			strokeLayer = drawCircle(backgroundLayerColor, -100);
		}

		public void render()
		{
			BackgroundColor = UIColor.Clear;
			strokeLayer.RemoveFromSuperLayer();

			renderBackgroundLayer();
			strokeLayer = drawCircle(backgroundLayerColor, -100);
			drawStroke(angle);
		}

		protected CAShapeLayer drawCircle(UIColor fillColor, int zPosition)
		{
			return drawArc(fillColor, 0.0f, (float)(2.0 * Math.PI), zPosition);
		}
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
		//CoreGraphics starts its rendering at 3 o'clock.
		//In order to start at 12 o'clock we always have to subtract PI/2
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

		private void renderBackgroundLayer()
		{
			if (!muted) {
				backgroundLayer?.RemoveFromSuperLayer();
				backgroundLayer = drawCircle(backgroundLayerColor, -100);
			}
		}
	}
}