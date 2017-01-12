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
		private string text = "100%";
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				label.Text = value;
			}
		}
		public override UIColor ProgressColor
		{
			get
			{
				return progressColor;
			}
			set
			{
				progressColor = value;
				strokeLayer.StrokeColor = value.CGColor;

				//strokeLayer.setNeedsDisplay()
				drawStroke(angle);
				drawBackground();
				renderBackground();
				label.TextColor = value;

			}
		}
	
		public override int Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
				Frame = new CGRect(Frame.X, Frame.Y, value, value);
				backgroundLayer.RemoveFromSuperLayer();
				strokeLayer.RemoveFromSuperLayer();
				label.RemoveFromSuperview();

				BackgroundColor = UIColor.LightGray;
				backgroundLayer = drawCircle(backgroundLayerColor, -100);
				strokeLayer = drawCircle(backgroundLayerColor, -100);

				Console.WriteLine(Frame);
				setupLabel();
				label.SetNeedsDisplay();
				drawStroke(angle);
			}
		}
		public SingleProgressView(CGRect frame) : base(frame)
		{
			setupLabel();
		}
		private void setupLabel()
		{
			label = new UILabel(new CGRect(0, 0, Frame.Width / 4, 40))
			{
				Text = text,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize(21)
			};
			label.TranslatesAutoresizingMaskIntoConstraints = false;
			AddSubview(label);

			label.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
			label.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
		}

	}
}
