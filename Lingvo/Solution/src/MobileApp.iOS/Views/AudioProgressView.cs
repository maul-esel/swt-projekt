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

		private float lineWidth = 10.0f;
		private bool studentMuted = false;
		private int progress = 0;
		private int maxProgress = 100;


		public delegate void StudentTrackMutedEventHandler(bool muted);
		public event StudentTrackMutedEventHandler StudentTrackMuted;

		UIButton muteBtn = new Func<UIButton>(() =>
		{
			var btn = new UIButton(new CGRect(0, 0, 50, 50));
			btn.SetTitleColor(UIColor.Black, UIControlState.Normal);
			btn.SetTitle("Mute", UIControlState.Normal);
			return btn;
		})();
		UILabel timeLabel = new UILabel()
		{
			Text = "00:00",
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
			AddSubview(teacherProgressBar);

			studentProgressBar = new CircleProgressBar(Frame);
			studentProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			studentProgressBar.NestingLevel = 1;
			studentProgressBar.backgroundLayer.SetNeedsDisplay();
			AddSubview(studentProgressBar);

			//Autolayout
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


			var stackView = new UIStackView()
			{
				Axis = UILayoutConstraintAxis.Vertical,
				Alignment = UIStackViewAlignment.Center
			};
			AddSubview(stackView);
			stackView.AddArrangedSubview(timeLabel);
			stackView.AddArrangedSubview(muteBtn);


			muteBtn.AddTarget((object sender, EventArgs e) =>
			{
				studentMuted = !studentMuted;
				StudentTrackMuted?.Invoke(studentMuted);
				muteBtn.SetTitle("Unmute", UIControlState.Normal);

			}, UIControlEvent.TouchUpInside);

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

				var modValue = value % maxProgress;
				progress = modValue;
				teacherProgressBar.Progress = modValue;

				if (!studentMuted)
				{
					studentProgressBar.Progress = modValue;
					studentProgressBar.strokeLayer.SetNeedsDisplay();
					studentProgressBar.SetNeedsDisplay();

				}



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
			}
		}
		public UIColor OuterProgressColor
		{
			get { return teacherProgressBar?.ProgressColor; }
			set
			{
				teacherProgressBar.ProgressColor = value;
			}
		}
		public void animate()
		{
			Size = 100;
			Progress = 40;
			Progress = 50;
			InnerProgressColor = UIColor.Blue;
			/*LineWidth = 20;
			Progress = 20;*/
			/*ProgressColor = UIColor.Green;
			MaxProgress = 200;*/
		}

		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);
			if (layer == Layer)
			{

				if (studentProgressBar != null)
				{
					studentProgressBar.Frame = Frame;
					studentProgressBar.renderBackground();
					studentProgressBar.SetNeedsDisplay();
				}

				if (teacherProgressBar != null)
				{
					teacherProgressBar.Frame = Frame;
					teacherProgressBar.renderBackground();
				}


				//backgroundLayer.Frame = layer.Bounds;
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
