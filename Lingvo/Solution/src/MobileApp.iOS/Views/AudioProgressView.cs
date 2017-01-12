using System;
using UIKit;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;
using ObjCRuntime;

namespace Lingvo.MobileApp.iOS
{
	public class AudioProgressView : UIView
	{
		public CircleProgressBar teacherProgressBar;
		public CircleProgressBar studentProgressBar;

		private static UIColor studentGreen = new UIColor(61.0f / 255.0f, 182.0f / 255.0f, 128.0f / 255.0f, 1.0f);
		private static UIColor teacherBlue = new UIColor(74.0f / 255.0f, 144.0f / 255.0f, 226.0f / 255.0f, 1.0f);

		private float lineWidth = 10.0f;
		private bool studentMuted = false;
		private int progress = 0;
		private int maxProgress = 100;


		public delegate void StudentTrackMutedEventHandler(bool muted);
		public event StudentTrackMutedEventHandler StudentTrackMuted;

		UIButton muteBtn = new Func<UIButton>(() =>
		{
			var btn = new UIButton(new CGRect(0, 0, 50, 50));
			btn.SetTitleColor(teacherBlue, UIControlState.Normal);
			btn.SetTitle("Mute", UIControlState.Normal);
			return btn;
		})();
		UILabel timeLabel = new UILabel()
		{
			Text = "00:00",
			TextColor = teacherBlue,
			Font = UIFont.SystemFontOfSize(28)
		};

		public AudioProgressView(CGRect frame) : base(frame)
		{
			setupViews();
		}


		public void setupViews()
		{
			teacherProgressBar = new CircleProgressBar(Frame);
			teacherProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			teacherProgressBar.ProgressColor = teacherBlue;
			AddSubview(teacherProgressBar);

			studentProgressBar = new CircleProgressBar(Frame);
			studentProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			studentProgressBar.ProgressColor = studentGreen;
			studentProgressBar.NestingLevel = 1;
			studentProgressBar.backgroundLayer.SetNeedsDisplay();
			AddSubview(studentProgressBar);

			//layout views programatically
			//create Autolayout constraints
			var viewNames = NSDictionary.FromObjectsAndKeys(new NSObject[] {
				teacherProgressBar,
				studentProgressBar
			}, new NSObject[] {
				new NSString("teacher_bar"),
				new NSString("student_bar")
			});
			var emptyDict = new NSDictionary();

			AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[teacher_bar]|", 0, emptyDict, viewNames));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[teacher_bar]|", 0, emptyDict, viewNames));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[student_bar]|", 0, emptyDict, viewNames));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[student_bar]|", 0, emptyDict, viewNames));
			studentProgressBar.CenterXAnchor.ConstraintEqualTo(teacherProgressBar.CenterXAnchor).Active = true;
			studentProgressBar.CenterYAnchor.ConstraintEqualTo(teacherProgressBar.CenterYAnchor).Active = true;


			//Wrap label in vertical stack view to avoid repositioning the label programatically
			var stackView = new UIStackView()
			{
				Axis = UILayoutConstraintAxis.Vertical,
				Alignment = UIStackViewAlignment.Center
			};
			AddSubview(stackView);
			stackView.AddArrangedSubview(timeLabel);
			stackView.AddArrangedSubview(muteBtn);

			//subscribe to click events of button in the middle
			muteBtn.AddTarget((object sender, EventArgs e) =>
			{
				studentMuted = !studentMuted;
				StudentTrackMuted?.Invoke(studentMuted);
				muteBtn.SetTitle("Unmute", UIControlState.Normal);

			}, UIControlEvent.TouchUpInside);

			//layout stack view using Autolayout
			stackView.TranslatesAutoresizingMaskIntoConstraints = false;
			stackView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
			stackView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
		}

		public int Progress
		{
			get
			{
				return progress;
			}
			set
			{

				var modValue = Math.Min(maxProgress, value);
				progress = modValue;
				teacherProgressBar.Progress = modValue;

				if (!studentMuted)
				{
					studentProgressBar.Progress = modValue;
					studentProgressBar.strokeLayer.SetNeedsDisplay();
					studentProgressBar.SetNeedsDisplay();

				}


				//update the progress time label text
				string minutes = (value / 60 < 10 ? "0" : "") + value / 60;
				string seconds = (value % 60 < 10 ? "0" : "") + value % 60;

				timeLabel.Text = minutes + ":" + seconds;
			}
		}
		protected void runOnMainThread(Action action)
		{
			//updates on UI only work on the main thread
			DispatchQueue.MainQueue.DispatchAsync(action);
		}

		public int MaxProgress
		{
			get
			{
				return maxProgress;
			}
			set
			{
				studentProgressBar.MaxProgress = value;
				teacherProgressBar.MaxProgress = value;
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


				teacherProgressBar.LineWidth = value;
				studentProgressBar.LineWidth = value;
			}
		}
		public bool InnerProgressEnabled
		{
			get
			{
				return !studentMuted;
			}
			set
			{
				studentMuted = !value;
				studentProgressBar.Muted = true;
			}
		}

		public int Size
		{
			get
			{
				return (int)Frame.Width;
			}
			set
			{
				Frame = new CGRect(Frame.X, Frame.Y, value, value);
				studentProgressBar.Size = value;
				studentProgressBar.Size = value;
			}
		}

		public UIColor InnerProgressColor
		{
			get
			{
				return studentProgressBar.ProgressColor;
			}
			set
			{
				studentProgressBar.ProgressColor = value;
				SetNeedsDisplay();
			}
		}
		public UIColor OuterProgressColor
		{
			get { return teacherProgressBar?.ProgressColor; }
			set
			{
				teacherProgressBar.ProgressColor = value;
				SetNeedsDisplay();
				timeLabel.TextColor = value;
				muteBtn.SetTitleColor(value, UIControlState.Normal);
			}
		}

		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);
			if (layer == Layer)
			{

				if (studentProgressBar != null)
				{
					studentProgressBar.Frame = Frame;
					studentProgressBar.render();
					studentProgressBar.SetNeedsDisplay();
				}

				if (teacherProgressBar != null)
				{
					teacherProgressBar.Frame = Frame;
					teacherProgressBar.render();

				}
			}
		}
		public void render()
		{
			teacherProgressBar?.RemoveFromSuperview();
			studentProgressBar?.RemoveFromSuperview();

			setupViews();
		}

	}
}
