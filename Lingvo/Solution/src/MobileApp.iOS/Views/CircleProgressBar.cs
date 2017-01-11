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
		public CALayer backgroundLayer;
		public CAShapeLayer strokeLayer;
		protected float lineWidth = 10.0f;
		protected float margin = 0.0f;
		protected int nestingLevel = 0;
		//protected UIColor unfinishedCircleColor = new UIColor(234.0f / 255.0f, 234.0f / 255.0f, 234.0f / 255.0f, 1.0f); //grayish color
		protected UIColor unfinishedCircleColor = UIColor.Brown;
		protected int progress = 0;
		protected readonly string animationName = "drawCircleAnim";
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
				backgroundLayer.RemoveFromSuperLayer();
				strokeLayer.RemoveFromSuperLayer();
				backgroundLayer = drawCircle(unfinishedCircleColor, -100);
				strokeLayer = drawCircle(unfinishedCircleColor, -100);
				drawStroke(angle);
			}
		}
		public UIColor ProgressColor
		{
			get
			{
				return progressColor;
			}
			set
			{
				progressColor = value;
				strokeLayer.StrokeColor = value.CGColor;
				strokeLayer.SetNeedsDisplay();
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
				progress = value % maxProgress;
				drawStroke(angle);
			}
		}
		protected float angle
		{
			get
			{
				var modValue = progress % maxProgress;
				float percentage = (float)modValue / (float)maxProgress;
				return (float)(percentage * (2 * Math.PI));
			}
		}



		protected void runOnMainThread(Action action)
		{
			//updates on UI only work on the main thread
			DispatchQueue.MainQueue.DispatchAsync(action);
		}

		public void drawStroke(float endAngle)
		{
			var startAngle = correctAngle(0.0f);
			var end = correctAngle(endAngle);
			var circleFragment = new UIBezierPath();

			circleFragment.AddArc(Center, radius, startAngle, end, true);
			strokeLayer.StrokeColor = UIColor.Yellow.CGColor;
			strokeLayer.Path = circleFragment.CGPath;
			strokeLayer.SetNeedsDisplay();

		}

		public CircleProgressBar(CGRect frame) : base(frame)
		{
			backgroundLayer = drawCircle(unfinishedCircleColor, -100);
			strokeLayer = drawCircle(unfinishedCircleColor, -100);
		}

		public void renderBackground()
		{
			backgroundLayer?.RemoveFromSuperLayer();
			strokeLayer.RemoveFromSuperLayer();
			backgroundLayer = drawCircle(unfinishedCircleColor, -100);
			strokeLayer = drawCircle(unfinishedCircleColor, -100);
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
				ZPosition = zPosition
			};
			var path = new UIBezierPath();
			path.AddArc(Center, radius, adjustedStartAngle, adjustedEndAngle, true);
			circleLayer.Path = path.CGPath;
			Layer.AddSublayer(circleLayer);
			return circleLayer;
		}
		private float correctAngle(float angle)
		{
			return angle - (float)(Math.PI / 2.0);
		}
		public void animate()
		{
			Size = 100;
			Progress = 20;
			/*ProgressColor = UIColor.Green;
			MaxProgress = 200;*/
		}
		public void Animate(float strokeStart, nfloat strokeEnd, float duration)
		{
			/*strokeLayer.StrokeColor = UIColor.Yellow.CGColor;
			strokeLayer.StrokeStart = strokeStart;
			strokeLayer.StrokeEnd = strokeEnd;

			var animation = new CABasicAnimation()
			{
				KeyPath = "strokeEnd",
				Duration = duration,
				From = new NSNumber(0),
				To = new NSNumber(strokeEnd)
			};

			strokeLayer.AddAnimation(animation, animationName);
			animationStartTime = strokeLayer.ConvertTimeFromLayer(CAAnimation.CurrentMediaTime(), null);*/


		}
		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);
			if (layer == Layer)
			{
				backgroundLayer.Frame = Bounds;
				strokeLayer.Frame = Bounds;
			}

		}


	}

	/*public class CircleProgressBar : UIView
	{


		public CAShapeLayer strokeLayer;
		private float maxProgress = 100.0f;
		private float lineWidth = 10.0f;
		private float margin = 0.0f;
		private UIColor progressColor = UIColor.Blue;
		private UIColor unfinishedCircleColor = new UIColor(234.0f / 255.0f, 234.0f / 255.0f, 234.0f / 255.0f, 1.0f); //grayish color
		private UIView centerView;
		private float progress;
		private int nestingLevel = 0;
		public CALayer backgroundLayer;
		private bool muted;



		private readonly string animationName = "drawCircleAnim";
		private double animationStartTime;

		public bool Muted
		{
			get
			{
				return muted;
			}
			set
			{
				muted = value;
				redrawStroke();
				/*runOnMainThread(new Action(() => {
					strokeLayer.RemoveFromSuperLayer();
				}));

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
		public UIView CenterView
		{
			get
			{
				return centerView;
			}
			set
			{
				centerView = value;
				runOnMainThread(new Action(() =>
				{
					centerView.Center = this.Center;
					AddSubview(centerView);
					BringSubviewToFront(centerView);
					SetNeedsDisplay();
				}));
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
				if (value >= 0)
				{
					lineWidth = value;
					clear();
					redrawStroke();


				}

			}
		}

		public UIColor UnfinishedCircleColor
		{
			get
			{
				return unfinishedCircleColor;
			}
			set
			{
				unfinishedCircleColor = value;
				SetNeedsDisplay();
			}
		}
		public UIColor ProgressColor
		{
			get
			{
				return progressColor;
			}
			set
			{
				progressColor = value;
				redrawStroke();
			}
		}

		public float Progress
		{
			get
			{
				//if an animation is running, we calculate its progress
				var anim = strokeLayer?.AnimationForKey(animationName);
				if (anim != null)
				{
					var currentAnimationTime = strokeLayer.ConvertTimeFromLayer(CAAnimation.CurrentMediaTime(), null);
					var progressPercentage = (currentAnimationTime - animationStartTime) / anim.Duration;
					return (float)(progressPercentage * maxProgress);
				}
				//if not the progress might have been set programatically, so return our private variable
				return progress;
			}
			set
			{
				progress = value > 0 ? value : 0.0f;
				redrawStroke();
			}
		}
		private void redrawStroke()
		{
			if (progress < 0.0f)
			{
				return;
			}
			if (muted)
			{
				runOnMainThread(new Action(() => {
					strokeLayer?.RemoveFromSuperLayer();
					strokeLayer = null;
				}));

			}
			var percentage = progress / maxProgress;
			var endAngle = percentage * (float)(2 * Math.PI);

			//updates on UI only work on the main thread
			runOnMainThread(new Action(() =>
			{
				strokeLayer?.RemoveFromSuperLayer();
				if (!muted)
				{
					strokeLayer = drawArc(ProgressColor, 0.0f, endAngle, 2);
					Layer.AddSublayer(strokeLayer);
				}
			}));
		}


		public float Margin
		{
			get
			{
				return margin;
			}
			set
			{
				if (value >= 0)
				{
					margin = value;
					clear();
					redrawStroke();
				}
			}
		}

		public float MaxProgress
		{
			get
			{
				return maxProgress;
			}
			set
			{
				if (value > 0.0)
				{
					maxProgress = value;
					redrawStroke();
				}
			}
		}
		public float Size
		{
			get
			{
				return (float)(Math.Min(Frame.Width, Frame.Height));
			}
			set
			{
				Frame = new CGRect(Frame.X, Frame.Y, value, value);
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


		public CircleProgressBar()
		{
			//setupViews();
		}

		public CircleProgressBar(CGRect frame) : base(frame)
		{
			//setupViews();
		}
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			setupViews();
		}
		protected virtual void setupViews()
		{
			backgroundLayer = drawCircle(unfinishedCircleColor, -100);
		}

		private CALayer drawCircle(UIColor fillColor, int zPosition)
		{
			return drawArc(fillColor, 0, (float)(2.0 * Math.PI), zPosition);
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
				ZPosition = zPosition
			};
			var path = new UIBezierPath();
			path.AddArc(Center, radius, adjustedStartAngle, adjustedEndAngle, true);
			circleLayer.Path = path.CGPath;
			Layer.AddSublayer(circleLayer);
			return circleLayer;
		}
		public void Animate(float startAngle, float endAngle, float duration)
		{


			var adjustedStrokeStartAngle = correctAngle(startAngle);
			var adjustedStrokeEndAngle = correctAngle(endAngle);



			var fillArcLayer = new CAShapeLayer()
			{
				LineWidth = lineWidth,
				FillColor = UIColor.Clear.CGColor,
				StrokeColor = progressColor.CGColor
			};
			var path = new UIBezierPath();
			path.AddArc(Center, radius, adjustedStrokeStartAngle, adjustedStrokeEndAngle, true);
			fillArcLayer.Path = path.CGPath;

			var animation = new CABasicAnimation()
			{
				KeyPath = "strokeEnd",
				Duration = fillArcLayer.ConvertTimeFromLayer(duration, null),
				From = new NSNumber(0),
				To = new NSNumber(1),
				TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear)
			};

			fillArcLayer.AddAnimation(animation, animationName);
			Layer.AddSublayer(fillArcLayer);
			animationStartTime = fillArcLayer.ConvertTimeFromLayer(CAAnimation.CurrentMediaTime(), null);

			this.strokeLayer = fillArcLayer;

		}
		//drawing usually starts at 3 o'clock , so we have to substract 90 degrees to start at 12 o'clock
		private float correctAngle(float angle)
		{
			return angle - (float)(Math.PI / 2.0);
		}
		protected void runOnMainThread(Action action)
		{
			//updates on UI only work on the main thread
			DispatchQueue.MainQueue.DispatchAsync(action);
		}
		public void clear()
		{
			runOnMainThread(new Action(() =>
			{
				Console.WriteLine("background layer was nil = " + backgroundLayer);
				backgroundLayer?.RemoveFromSuperLayer();
				strokeLayer?.RemoveFromSuperLayer();

			}));

		}
		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);
			if (layer == Layer)
			{
				if (strokeLayer != null)
					strokeLayer.Frame = layer.Bounds;
				if (backgroundLayer != null)
					backgroundLayer.Frame = layer.Bounds;

				//backgroundLayer.Frame = layer.Bounds;
			}

		}
	}*/
}
