using System;
using UIKit;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;
using System.Linq;



namespace Lingvo.MobileApp.iOS
{
	//https://forums.xamarin.com/discussion/48628/how-to-create-a-custom-control-uiview-with-xib-so-that-its-available-in-xamarin-ios-designer
	public class ProgressView : UIView
	{
		//TODO: add CAAnimationDelegate
		nfloat lineWidth = 30.0f;
		double margin = 8.0;

		nfloat duration = 15.0f;
		nfloat rewindStep = 1.0f;

		NSTimer timer = null;
		double offset = 0.0;

		NSDate startDate;

		UIColor outerCircleStrokeColor = new UIColor(72.0f / 255.0f, 144.0f / 255.0f, 226.0f / 255.0f, 1.0f);
		UIColor innerCircleStrokeColor = new UIColor(61.0f / 255.0f, 182.0f / 255.0f, 100.0f / 255.0f, 1.0f);
		UIColor grayedOutColor = new UIColor(234.0f / 255.0f, 234.0f / 255.0f, 234.0f / 255.0f, 1.0f);

		UILabel timeLabel = new Func<UILabel>(() => {
			var label = new UILabel();
			label.Text = "00:00";
			label.Font = UIFont.SystemFontOfSize(28);
			return label;
		})();


		UIButton muteBtn = new Func<UIButton>(() =>
		{
			var btn = new UIButton();
			btn.SetTitleColor(UIColor.Black, UIControlState.Normal);
			btn.SetTitle("Mute", UIControlState.Normal); 
			return btn;
		})();

		CALayer innerStrokeLayer;
		CALayer outerStrokeLayer;

		nfloat radius
		{
			get
			{
				var outerWidthRadius = Frame.Width / 2 - lineWidth / 2 - 2 * margin;
				var outerHeightRadius = Frame.Height / 2 - lineWidth / 2 - 2 * margin;

				return (nfloat)Math.Min(outerWidthRadius, outerHeightRadius);

			}
		}

		double innerRadius
		{
			get
			{
				return radius - lineWidth;
			}
		}

		public ProgressView()
		{
			setupViews();
		}

		public ProgressView(CGRect frame) : base(frame)
		{
			setupViews();
		}

		private void setupViews()
		{
		}


		/*UIStackView stackView = new Func<UIStackView>(() =>
		{
			var sv = new UIStackView();
			sv.AddArrangedSubview(timeLabel);
			return sv;
		})();*/

		private void createCircle(CGPoint center, nfloat radius, UIColor fillColor, bool animated)
		{
			var circleLayer = new CAShapeLayer();
			circleLayer.LineWidth = lineWidth;
			circleLayer.FillColor = grayedOutColor.CGColor;
			circleLayer.StrokeColor = fillColor.CGColor;
			circleLayer.ZPosition = -100;

			var path = new UIBezierPath();
			path.AddArc(center, radius, 0, (nfloat)(2.0 * Math.PI), true);
			circleLayer.Path = path.CGPath;

			Layer.AddSublayer(circleLayer);
		}
		private CALayer startStroking(double duration, nfloat radius, UIColor color)
		{
			var strokeLayer = new CAShapeLayer()
			{
				LineWidth = lineWidth,
				FillColor = UIColor.Clear.CGColor,
				StrokeColor = color.CGColor
			};
			var path = new UIBezierPath();
			path.AddArc(Center, radius, (nfloat)Math.PI / 2, (nfloat)(1.5 * Math.PI), true);
			strokeLayer.Path = path.CGPath;

			var animation = new CABasicAnimation()
			{
				Duration = strokeLayer.ConvertTimeFromLayer(duration, null),
				From = new NSNumber(0),
				To = new NSNumber(1),
				TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear)
			};
			strokeLayer.AddAnimation(animation, "drawCircleAnim");
			Layer.AddSublayer(strokeLayer);
			return strokeLayer;
		}

		void animateOuterCircle(nfloat duration)
		{
			if (duration <= 0) {
				return;
			}
			this.duration = duration;
			startTimer();
			outerStrokeLayer = startStroking(duration, radius, innerCircleStrokeColor);


		}
		void animateInnerCircle(nfloat duration)
		{
			if (duration <= 0)
			{
				return;
			}
			this.duration = duration;
			startTimer();
			innerStrokeLayer = startStroking(duration, radius, innerCircleStrokeColor);
		}
		void animateInnerOuterCircle(nfloat duration)
		{
			if (duration <= 0)
			{
				return;
			}
			this.duration = duration;
			startTimer();
			animateOuterCircle(duration);
			animateInnerCircle(duration);
		}

		#region pause, rewind and resume layers
		private void pauseLayer(CALayer layer)
		{
			if (layer == null) {
				return;
			}
			var pausedTime = layer.ConvertTimeFromLayer(CAAnimation.CurrentMediaTime(), null);
			layer.TimeOffset = pausedTime;
			layer.Speed = 0.0f;

		}
		void pauseOuterCircleAnimation()
		{
			pauseLayer(outerStrokeLayer);
		}
		void pauseInnerCircleAnimation()
		{
			pauseLayer(innerStrokeLayer);
		}
		void resume()
		{
			resumeLayer(innerStrokeLayer);
			resumeLayer(outerStrokeLayer);
		}
		private void resumeLayer(CALayer layer)
		{
			if (layer == null) {
				return;
			}
			var pausedTime = layer.TimeOffset;
			layer.Speed = 1.0f;
			layer.TimeOffset = 0.0;
			layer.BeginTime = 0.0;
			var timeSincePause = layer.ConvertTimeFromLayer(CAAnimation.CurrentMediaTime(), null) - pausedTime;
			layer.BeginTime = timeSincePause;

		}
		void reset()
		{
			innerStrokeLayer?.RemoveFromSuperLayer();
			outerStrokeLayer?.RemoveFromSuperLayer();
		}
		private void rewind(CALayer layer)
		{
			if (layer == null) {
				return;
			}
			pauseLayer(layer);
			var runningAnimation = layer.AnimationForKey("drawCircleAnim");
			var pausedTime = layer.TimeOffset;
			if (runningAnimation != null) {
				if (pausedTime - rewindStep < runningAnimation.BeginTime)
				{
					var diff = pausedTime - runningAnimation.BeginTime;
					layer.BeginTime = -diff;
					offset -= diff;
				}
				else 
				{
					layer.BeginTime = -rewindStep;
					offset -= rewindStep;
				}
			}
			layer.Speed = 1.0f;
			layer.TimeOffset = 0.0;
			var timeSincePause = layer.ConvertTimeFromLayer(CAAnimation.CurrentMediaTime(), null) - pausedTime;
			layer.BeginTime = timeSincePause;
		}

		void rewind()
		{
			rewind(innerStrokeLayer);
			rewind(outerStrokeLayer);
		}
		#endregion
		private void startTimer()
		{
			timer = NSTimer.CreateScheduledTimer(0.1, this, new ObjCRuntime.Selector("updateTimer"), null, true);
			startDate = new NSDate();
		}
		void updateTimer(NSTimer timer)
		{
		}




	}
}
