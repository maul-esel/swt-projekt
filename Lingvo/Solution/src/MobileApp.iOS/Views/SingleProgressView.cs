using System;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;

namespace Lingvo.MobileApp.iOS
{
	public class SingleProgressView : CircleProgressBar
	{

		UILabel label;
		private string labelText = "100%";
		public string Text
		{
			get
			{
				return labelText;
			}
			set
			{
				labelText = value;
				label.Text = labelText;
			}
		}


		public SingleProgressView(CGRect frame) : base(frame)
		{
			setupViews();
		}



		protected override void setupViews()
		{
			base.setupViews();

			label = new UILabel(new CGRect(0, 0, Frame.Width / 4, 40))
			{
				Text = "100%",
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize(28)
			};

			runOnMainThread(new Action(() =>
			{
				CenterView = label;
			}));


		}
	}
}
