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
		private CAShapeLayer strokeLayer;
		private float maxProgress = 100.0f;
		private float lineWidth = 10.0f;
		private float margin = 0.0f;
		private UIColor progressColor = UIColor.Blue;
		private UIColor unfinishedCircleColor = new UIColor(234.0f / 255.0f, 234.0f / 255.0f, 234.0f / 255.0f, 1.0f); //grayish color
		private UIView centerView;
		private float progress;


		private readonly string animationName = "drawCircleAnim";
		private double animationStartTime;

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
				SetNeedsDisplay();
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
				if (value <= 0.0f)
				{
					runOnMainThread(new Action(() =>
					{
						strokeLayer?.RemoveFromSuperLayer();
					}));
					return;
				}

				progress = value;
				var percentage = value / maxProgress;
				var endAngle = percentage * (float)(2 * Math.PI);

				//updates on UI only work on the main thread
				runOnMainThread(new Action(() =>
				{
					strokeLayer?.RemoveFromSuperLayer();
					strokeLayer = drawArc(ProgressColor, 0.0f, endAngle, 2);
				}));
			}
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
					SetNeedsDisplay();
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
					SetNeedsDisplay();
				}
			}
		}
		public float Size
		{
			get
			{
				return (float)(Math.Min(Frame.Width, Frame.Height));
			}
		}

		private float radius
		{
			get
			{
				var outerWidthRadius = Frame.Width / 2 - lineWidth / 2 - 2 * margin;
				var outerHeightRadius = Frame.Height / 2 - lineWidth / 2 - 2 * margin;

				return (float)Math.Min(outerWidthRadius, outerHeightRadius);
			}
		}


		public CircleProgressBar()
		{
			setupViews();
		}

		public CircleProgressBar(CGRect frame) : base(frame)
		{
			setupViews();
		}

		protected virtual void setupViews()
		{
			drawCircle(unfinishedCircleColor, -100);
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
	}
}
