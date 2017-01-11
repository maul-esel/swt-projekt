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
	/*public class AudioProgressView : UIView
	{
		CircleProgressBar studentProgressBar;
		CircleProgressBar teacherProgressBar;
		private float lineWidth = 10.0f;
		private float margin = 0.0f;
		private bool studentMuted = false;
		private int progress = 0;


		public delegate void StudentTrackMutedEventHandler(bool muted);

		public event StudentTrackMutedEventHandler StudentTrackMuted;


	

		public int Progress
		{
			get
			{
				return progress;
			}
			set
			{
				if (value > 0)
				{
					progress = value;
					runOnMainThread(new Action(() =>
					{
						var correctedProgressValue = value % MaxProgress;
						teacherProgressBar.Progress = correctedProgressValue;
						if (!studentMuted)
						{
							studentProgressBar.Progress = correctedProgressValue;
						}

						string minutes = (value / 60 < 10 ? "0" : "") + value / 60;
						string seconds = (value % 60 < 10 ? "0" : "") + value % 60;

						timeLabel.Text = minutes + ":" + seconds;

					}));

				}
			}
		}

		public override CGRect Frame
		{
			get
			{
				return base.Frame;
			}
			set
			{
				base.Frame = value;

				runOnMainThread(new Action(() =>
				{
					teacherProgressBar?.RemoveFromSuperview();
					studentProgressBar?.RemoveFromSuperview();


					teacherProgressBar?.clear();
					studentProgressBar?.clear();



					teacherProgressBar = null;
					studentProgressBar = null;
					setupViews();

				}));


			}
		}
		public override CGRect Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				base.Bounds = value;
				runOnMainThread(new Action(() =>
				{
					teacherProgressBar?.RemoveFromSuperview();
					studentProgressBar?.RemoveFromSuperview();
					teacherProgressBar?.clear();
					studentProgressBar?.clear();
					setupViews();
				}));
			}
		}
		public int MaxProgress
		{
			get { return (int)studentProgressBar.MaxProgress; }
			set
			{
				studentProgressBar.MaxProgress = value;
				teacherProgressBar.MaxProgress = value;
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
					studentProgressBar.Margin = value;
					teacherProgressBar.Margin = value;
				}
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


				runOnMainThread(new Action(() =>
				{
					teacherProgressBar.LineWidth = value;
					studentProgressBar.LineWidth = value;

				}));
			}
		}

		public UIColor InnerProgressColor
		{
			get { return studentProgressBar?.ProgressColor; }
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
		//we need this because of the interface definition
		public int Size
		{
			get
			{
				return (int)Frame.Width;
			}
			set
			{
				Frame = new CGRect(Frame.X, Frame.Y, value, value);

			}
		}

		public bool InnerProgressEnabled
		{
			get { return !studentMuted; }
			set
			{
				studentMuted = !value;
				if (studentMuted)
				{
					/*studentProgressBar.removeStroke();
					studentProgressBar
					studentProgressBar.Muted = studentMuted;
				}
			}
		}

		public bool InnerMuteButtonVisible
		{
			get { return muteBtn.Hidden; }
			set
			{
				runOnMainThread(new Action(() => muteBtn.Hidden = value));

			}
		}
		float teacherRadius
		{
			get
			{
				var outerWidthRadius = Frame.Width / 2 - lineWidth / 2 - 2 * margin;
				var outerHeightRadius = Frame.Height / 2 - lineWidth / 2 - 2 * margin;

				return (float)Math.Min(outerWidthRadius, outerHeightRadius);
			}
		}
		float studentRadius
		{
			get
			{
				return teacherRadius - lineWidth;

			}
		}

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
		public override void AwakeFromNib()
		{
			setupViews();
			base.AwakeFromNib();
		}

		private void setupViews()
		{
			var teacherFrame = Frame;
			teacherProgressBar = new CircleProgressBar(new CGRect());
			teacherProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			teacherProgressBar.LineWidth = LineWidth;
			teacherProgressBar.Progress = progress;
			AddSubview(teacherProgressBar);




			var teacherFrameLineWidth = teacherProgressBar.LineWidth;
			var studentFrameInset = teacherProgressBar.LineWidth / 2 - margin;
			var studentFrame = new CGRect(studentFrameInset, studentFrameInset, teacherFrame.Width - 2 * teacherFrameLineWidth, teacherFrame.Height - 2 * teacherFrameLineWidth);


			studentProgressBar = new CircleProgressBar(new CGRect());
			studentProgressBar.LineWidth = LineWidth;
			studentProgressBar.Progress = progress;
			studentProgressBar.NestingLevel = 1; //this calculates the margin so that this bar is displayed as the inner circle
			studentProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			AddSubview(studentProgressBar);



			//teacherProgressBar.BackgroundColor = UIColor.Yellow;
			//studentProgressBar.BackgroundColor = new UIColor(0, 0, 1, (nfloat)0.2);
			//teacherProgressBar.BackgroundColor = new UIColor(0, (nfloat)1.0, (nfloat)0.0, (nfloat)1.0);
			//studentProgressBar.ProgressColor = UIColor.Red;
			//studentProgressBar.BackgroundColor = new UIColor(0, 0, (nfloat)1.0, (nfloat)1.0);
			//studentProgressBar.UnfinishedCircleColor = UIColor.Green;


			//Autolayout
			var viewNames = NSDictionary.FromObjectsAndKeys(new NSObject[] {
				teacherProgressBar,
				studentProgressBar
			}, new NSObject[] {
				new NSString("teacher_bar"),
				new NSString("student_bar")
			});
			var emptyDict = new NSDictionary();

			AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|-0-[teacher_bar]-0-|", 0, emptyDict, viewNames));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-0-[teacher_bar]-0-|", 0, emptyDict, viewNames));
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

		protected void runOnMainThread(Action action)
		{
			//updates on UI only work on the main thread
			DispatchQueue.MainQueue.DispatchAsync(action);
		}

	}*/
}
