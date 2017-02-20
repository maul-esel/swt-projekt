using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;
using UIKit;
using Lingvo.MobileApp;
using ObjCRuntime;

[assembly: ExportRenderer(typeof(Lingvo.MobileApp.Forms.LingvoRoundImageButton), typeof(Lingvo.MobileApp.iOS.RoundImageButtonRenderer))]
namespace Lingvo.MobileApp.iOS
{
	public class RoundImageButtonRenderer : ViewRenderer<Lingvo.MobileApp.Forms.LingvoRoundImageButton, UIView>
	{
		UIView button;
		UIImageView imageView;
		UILabel label;
		UITapGestureRecognizer tapGestureRecognizer;


		protected override void OnElementChanged(ElementChangedEventArgs<Lingvo.MobileApp.Forms.LingvoRoundImageButton> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				setupViews();
				SetNativeControl(button);
			}

			if (e.OldElement != null && e.NewElement == null)
			{
				e.OldElement.PropertyChanged -= updateView;
				e.OldElement.SizeChanged -= controlSizeChanged;
				if (tapGestureRecognizer != null)
				{
					button.RemoveGestureRecognizer(tapGestureRecognizer);
				}

			}
			else if (e.NewElement != null)
			{

				e.NewElement.PropertyChanged += updateView;
				tapGestureRecognizer = new UITapGestureRecognizer((UITapGestureRecognizer obj) =>
				{
					e.NewElement.OnButtonClicked(button, null);
				});
				button.AddGestureRecognizer(tapGestureRecognizer); //detect tap gestures on UIView
				e.NewElement.SizeChanged += controlSizeChanged;


			}
		}

		private void controlSizeChanged(object sender, EventArgs eventArgs)
		{
			var roundedControl = sender as Lingvo.MobileApp.Forms.LingvoRoundImageButton;
			if (roundedControl != null)
			{
				//var frame = new CGRect(roundedControl.X, roundedControl.Y, roundedControl.Width, roundedControl.Height);
				//button.Frame = frame;
				button.Layer.CornerRadius = (nfloat)roundedControl.Width / 2.0f;
			}
		}
		private void setupViews()
		{
			button = new UIView()
			{
				UserInteractionEnabled = true,
				BackgroundColor = UIColor.Clear
			};


			//setup autolayout for imageView
			imageView = new UIImageView()
			{
				ContentMode = UIViewContentMode.ScaleAspectFit,
				BackgroundColor = UIColor.Clear
			};
			imageView.TranslatesAutoresizingMaskIntoConstraints = false;
			button.AddSubview(imageView);

			//image view autolayout constraints
			imageView.LeftAnchor.ConstraintEqualTo(button.LeftAnchor).Active = true;
			imageView.RightAnchor.ConstraintEqualTo(button.RightAnchor).Active = true;
			imageView.TopAnchor.ConstraintEqualTo(button.TopAnchor).Active = true;
			imageView.BottomAnchor.ConstraintEqualTo(button.BottomAnchor).Active = true;


			//create label
			label = new UILabel()
			{
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.LightGray
			};
			label.TranslatesAutoresizingMaskIntoConstraints = false;
			button.AddSubview(label);

			//setup autolayout for label
			label.LeftAnchor.ConstraintEqualTo(button.LeftAnchor).Active = true;
			label.RightAnchor.ConstraintEqualTo(button.RightAnchor).Active = true;
			label.TopAnchor.ConstraintEqualTo(button.TopAnchor).Active = true;
			label.BottomAnchor.ConstraintEqualTo(button.BottomAnchor).Active = true;
			/*label.CenterXAnchor.ConstraintEqualTo(button.CenterXAnchor).Active = true;
			label.CenterYAnchor.ConstraintLessThanOrEqualTo(button.CenterYAnchor).Active = true;*/





		}
		private void buttonClicked()
		{
			Console.WriteLine("button has been clicked");
		}
		private void updateView(object sender, EventArgs e)
		{
			if (Control == null)
			{
				return;
			}
			Lingvo.MobileApp.Forms.LingvoRoundImageButton element = (Lingvo.MobileApp.Forms.LingvoRoundImageButton)sender;
			//set image here)

			var tintColor = element.IsEnabled ? element.Color.ToUIColor() : UIColor.LightGray;
			imageView.TintColor = tintColor;
			button.TintColor = tintColor;
			label.TextColor = tintColor;

			if (element.Image == "")
			{
				return;
			}

			string identifier = element.Image?.Substring(0, element.Image.LastIndexOf('.'));
			Console.WriteLine("imageName = " + identifier);


			var img = new UIImage(identifier).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
			imageView.Image = img;

			if (element.Text.Length > 0)
			{
				label.Text = element.Text;
			}

			//setup border
			if (element.Border)
			{
				button.Layer.Frame = button.Bounds;
				button.Layer.CornerRadius = button.Bounds.Width / 2;
				button.ClipsToBounds = true;
				button.Layer.BorderColor = tintColor.CGColor;
				button.Layer.BorderWidth = 1;
				button.Layer.SetNeedsDisplay();
			}

			label.Font = UIFont.SystemFontOfSize((int)(0.25 * element.HeightRequest), UIFontWeight.Regular);

		}


	}
}
