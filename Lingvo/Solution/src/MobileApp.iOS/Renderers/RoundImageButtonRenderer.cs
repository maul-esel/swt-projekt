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

		/// <summary>
		/// Called whenever a bindable property of the view has changed
		/// </summary>
		/// <param name="e">LinvgoRoundImageButton object</param>
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
				//unregister events
				e.OldElement.PropertyChanged -= updateView;
				e.OldElement.SizeChanged -= controlSizeChanged;
				if (tapGestureRecognizer != null)
				{
					button.RemoveGestureRecognizer(tapGestureRecognizer);
				}

			}
			else if (e.NewElement != null)
			{
				//register events
				e.NewElement.PropertyChanged += updateView;

				//make view tappable
				tapGestureRecognizer = new UITapGestureRecognizer((UITapGestureRecognizer obj) =>
				{
					e.NewElement.OnButtonClicked(button, null);
				});
				button.AddGestureRecognizer(tapGestureRecognizer); //detect tap gestures on UIView
				e.NewElement.SizeChanged += controlSizeChanged;


			}
		}
		/// <summary>
		/// Called whenever the view's frame changes
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="eventArgs">Event arguments</param>
		private void controlSizeChanged(object sender, EventArgs eventArgs)
		{
			var roundedControl = sender as Lingvo.MobileApp.Forms.LingvoRoundImageButton;
			if (roundedControl != null)
			{
				button.Layer.CornerRadius = (nfloat)roundedControl.Width / 2.0f;
			}
		}
		/// <summary>
		/// Creates the view and its Autolayout in code
		/// </summary>
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


			//create text label that is centered in the view
			label = new UILabel()
			{
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.LightGray
			};
			label.TranslatesAutoresizingMaskIntoConstraints = false;
			button.AddSubview(label);

			//setup autolayout constraints for label
			label.LeftAnchor.ConstraintEqualTo(button.LeftAnchor).Active = true;
			label.RightAnchor.ConstraintEqualTo(button.RightAnchor).Active = true;
			label.TopAnchor.ConstraintEqualTo(button.TopAnchor).Active = true;
			label.BottomAnchor.ConstraintEqualTo(button.BottomAnchor).Active = true;
	
		}
		private void updateView(object sender, EventArgs e)
		{
			if (Control == null)
			{
				return;
			}
			Lingvo.MobileApp.Forms.LingvoRoundImageButton element = (Lingvo.MobileApp.Forms.LingvoRoundImageButton)sender;

			//render the image with the respective tint color
			var tintColor = element.IsEnabled ? element.Color.ToUIColor() : UIColor.LightGray;
			imageView.TintColor = tintColor;
			button.TintColor = tintColor;
			label.TextColor = tintColor;

			if (element.Image == "")
			{
				return;
			}

			string identifier = element.Image?.Substring(0, element.Image.LastIndexOf('.'));

			var img = new UIImage(identifier).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
			imageView.Image = img;

			if (element.Text.Length > 0)
			{
				label.Text = element.Text;
			}

			//setup circular border for this control
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
