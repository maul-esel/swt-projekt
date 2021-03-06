﻿using System;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;


namespace Lingvo.MobileApp.iOS
{
	public class AudioProgressView : UIView
	{

		public CircleProgressBar teacherProgressBar;
		public CircleProgressBar studentProgressBar;

		private static UIColor studentColor = UIColor.Red;
		private static UIColor teacherColor = new UIColor(74.0f / 255.0f, 144.0f / 255.0f, 226.0f / 255.0f, 1.0f);

		//width of circle fragment
		private float lineWidth = 10.0f;
		private bool innerProgressEnabled = true;

		private int progress = 0;
		int maxProgress = 100;

		public delegate void StudentTrackMutedEventHandler(bool muted);
		public event StudentTrackMutedEventHandler StudentTrackMuted;

		UIButton muteBtn = new Func<UIButton>(() =>
		{
			var btn = new UIButton(new CGRect(0, 0, 50, 50));
			btn.TranslatesAutoresizingMaskIntoConstraints = false;
			btn.SetTitleColor(teacherColor, UIControlState.Normal);
			var btnImage = new UIImage("ic_volume_up");
			btn.SetImage(btnImage, UIControlState.Normal);
			return btn;
		})();
		UILabel timeLabel = new UILabel()
		{
			Text = "00:00",
			TextColor = teacherColor.ColorWithAlpha(0.8f),
			Font = UIFont.SystemFontOfSize(28, UIFontWeight.Thin)
		};

		//vertical container view that holds the time label and the mute button
		UIStackView stackView = new UIStackView()
		{
			Axis = UILayoutConstraintAxis.Vertical,
			Alignment = UIStackViewAlignment.Center
		};

		public AudioProgressView(CGRect frame) : base(frame)
		{
			setupViews();
		}

		/// <summary>
		/// creates the view layout programatically using Autolayout
		/// </summary>
		public void setupViews()
		{
			//setup outer circle bar
			teacherProgressBar = new CircleProgressBar(Frame);
			teacherProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			teacherProgressBar.ProgressColor = teacherColor;
			teacherProgressBar.MaxProgress = maxProgress;
			teacherProgressBar.Progress = progress;
			teacherProgressBar.LineWidth = lineWidth;
			AddSubview(teacherProgressBar);

			//setup inner circle bar
			studentProgressBar = new CircleProgressBar(Frame);
			studentProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			studentProgressBar.ProgressColor = studentColor;
			studentProgressBar.NestingLevel = 1;
			studentProgressBar.backgroundLayer.SetNeedsDisplay();
			studentProgressBar.MaxProgress = maxProgress;
			studentProgressBar.Progress = progress;
			studentProgressBar.LineWidth = lineWidth;
			studentProgressBar.Hidden = !innerProgressEnabled;
			AddSubview(studentProgressBar);

			//layout views using Autolayout so that they look good on different screen sizes
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
			AddSubview(stackView);
			stackView.AddArrangedSubview(timeLabel);
			stackView.AddArrangedSubview(muteBtn);

			//subscribe to click events of button in the middle
			muteBtn.AddTarget(OnMuteButtonClicked, UIControlEvent.TouchUpInside);

			//set the width and height of the muteButton
			muteBtn.WidthAnchor.ConstraintEqualTo(48).Active = true;
			muteBtn.HeightAnchor.ConstraintEqualTo(48).Active = true;

			updateMuteButtonImage();

			//layout stack view using Autolayout
			stackView.TranslatesAutoresizingMaskIntoConstraints = false;
			stackView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
			stackView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;

			//hide the inner bar if student is muted
			if (!innerProgressEnabled)
			{
				studentProgressBar.Muted = true;
			}
		}

		/// <summary>
		/// Handles click events of the mute button
		/// </summary>
		/// <param name="sender">the mute button</param>
		/// <param name="e">event args</param>
		private void OnMuteButtonClicked(object sender, EventArgs e)
		{
			innerProgressEnabled = !innerProgressEnabled;
			StudentTrackMuted?.Invoke(!innerProgressEnabled);

			updateMuteButtonImage();

			studentProgressBar.Muted = !innerProgressEnabled;
			studentProgressBar.render();

		}

		/// <summary>
		/// adjusts image of mute button (speaker + crossed out speaker)
		/// </summary>
		private void updateMuteButtonImage()
		{
			var imageName = innerProgressEnabled ? "ic_volume_up" : "ic_volume_off";
			var image = new UIImage(imageName);
			muteBtn.SetImage(image, UIControlState.Normal);
		}
		public bool MuteEnabled
		{
			get { 
				return !muteBtn.Hidden;
			}
			set 
			{ 
				muteBtn.Hidden = !value;
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
				//make sure we don't go over the max progress
				var minimum = Math.Min(MaxProgress, value);
				progress = minimum;
				teacherProgressBar.Progress = minimum;

				if (innerProgressEnabled)
				{
					studentProgressBar.Progress = minimum;
					studentProgressBar.strokeLayer.SetNeedsDisplay();
					studentProgressBar.SetNeedsDisplay();

				}
			}
		}

		public int TextSize
		{
			get
			{
				return (int)timeLabel.Font.PointSize;
			}
			set
			{
				timeLabel.Font = UIFont.SystemFontOfSize(value, UIFontWeight.Thin);
			}
		}

        public string Text
        {
            get { return timeLabel.Text; }
            set
            {
                timeLabel.Text = value;
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
				var readValue = innerProgressEnabled;
				return readValue;
			}
			set
			{
				innerProgressEnabled = value;
				render();
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
				studentColor = value;
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
				teacherColor = value;
				SetNeedsDisplay();
				timeLabel.TextColor = value;
				muteBtn.SetTitleColor(value, UIControlState.Normal);
			}
		}
		/// <summary>
		/// Layout the sublayers of the view's layer. Here the frame + bound property of the sublayers are set
		/// Otherwise Autolayout is unable to render the sublayers correctly
		/// </summary>
		/// <param name="layer">Layer.</param>
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
		/// <summary>
		/// Redraw the entire progress view
		/// </summary>
		public void render()
		{
			stackView.RemoveFromSuperview();
			stackView.RemoveArrangedSubview(muteBtn);
			stackView.RemoveArrangedSubview(timeLabel);
			muteBtn.RemoveTarget(OnMuteButtonClicked, UIControlEvent.TouchUpInside);
			teacherProgressBar?.RemoveFromSuperview();
			studentProgressBar?.RemoveFromSuperview();

			setupViews();
		}
	}
}
