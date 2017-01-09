using System;
using UIKit;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;

namespace Lingvo.MobileApp.iOS
{
	public class AudioProgressView : UIView
	{
		CircleProgressBar studentProgressBar;
		CircleProgressBar teacherProgressBar;
		private float lineWidth = 10.0f;
		private float margin = 0.0f;

		private bool studentMuted = false;

		public delegate void StudentTrackMutedEventHandler(bool muted);

		public event StudentTrackMutedEventHandler StudentTrackMuted;

		public int Progress
		{
			get
			{
				return (int)teacherProgressBar.Progress;
			}
			set
			{
				if (value > 0)
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
				}
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
				if (lineWidth > 0.0f)
				{
					lineWidth = value;
				}
			}
		}

		public UIColor InnerProgressColor
		{
			get { return studentProgressBar.ProgressColor; }
			set
			{
				studentProgressBar.ProgressColor = value;
			}
		}

		public UIColor OuterProgressColor
		{
			get { return teacherProgressBar.ProgressColor; }
			set
			{
				teacherProgressBar.ProgressColor = value;
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
				runOnMainThread(new Action(() =>
				{
					Frame = new CGRect(0, 0, value, value);
					SetNeedsDisplay();
				}));
			}
		}

		public bool InnerProgressEnabled
		{
			get { return !studentMuted; }
			set
			{
				studentMuted = !value;
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

		private void setupViews()
		{
			var teacherFrame = Frame;
			teacherProgressBar = new CircleProgressBar(teacherFrame);
			AddSubview(teacherProgressBar);

			var teacherFrameLineWidth = teacherProgressBar.LineWidth;
			var studentFrameInset = teacherProgressBar.LineWidth / 2 - margin;


			var studentFrame = new CGRect(studentFrameInset, studentFrameInset, teacherFrame.Width - 2 * teacherFrameLineWidth, teacherFrame.Height - 2 * teacherFrameLineWidth);
			studentProgressBar = new CircleProgressBar(studentFrame);

			AddSubview(studentProgressBar);
			studentProgressBar.ProgressColor = UIColor.Red;
			studentProgressBar.UnfinishedCircleColor = UIColor.Green;
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

	}
}
