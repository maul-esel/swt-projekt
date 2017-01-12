using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;
using UIKit;

[assembly: ExportRenderer(typeof(Lingvo.MobileApp.LingvoSingleProgressView), typeof(Lingvo.MobileApp.iOS.SingleProgressViewRenderer))]
namespace Lingvo.MobileApp.iOS
{
	class SingleProgressViewRenderer : ViewRenderer<LingvoSingleProgressView, UIView>
	{
		SingleProgressView progressView;
		LingvoSingleProgressView.LabelTypeValue labelType;

		protected override void OnElementChanged(ElementChangedEventArgs<LingvoSingleProgressView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				progressView = new SingleProgressView(Frame);
				SetNativeControl(progressView);
				labelType = e.NewElement.LabelType;
			}

			if (e.OldElement != null && e.NewElement == null)
			{
				e.OldElement.PropertyChanged -= updateView;
				e.OldElement.SizeChanged -= NewElementOnSizeChanged;

			}
			else if (e.NewElement != null)
			{
				e.NewElement.PropertyChanged += updateView;
				e.NewElement.SizeChanged += NewElementOnSizeChanged;

			}
		}

		private void NewElementOnSizeChanged(object sender, EventArgs eventArgs)
		{
			var audioProgressView = sender as LingvoAudioProgressView;

			if (audioProgressView != null)
			{
				var frame = new CGRect(audioProgressView.X, audioProgressView.X, audioProgressView.Width, audioProgressView.Height);
				Console.WriteLine("frame to be set = " + frame);
				progressView.Frame = frame;
				progressView.backgroundLayer.Frame = Bounds;
				progressView.backgroundLayer.SetNeedsDisplay();
			
			}
			else
			{
				Console.WriteLine("cast failed");
			}
		}

		private void updateView(object sender, EventArgs e)
		{
			LingvoSingleProgressView element = (LingvoSingleProgressView)sender;

			if (progressView.Size != element.Size)
				progressView.Size = element.Size;
			if (!progressView.ProgressColor.Equals(element.ProgressColor.ToUIColor()))
				progressView.ProgressColor = element.ProgressColor.ToUIColor();
			if (progressView.MaxProgress != element.MaxProgress)
				progressView.MaxProgress = element.MaxProgress;

			if (progressView.Progress != element.Progress || !labelType.Equals(element.LabelType))
			{
				if (progressView.Progress != element.Progress)
					progressView.Progress = element.Progress;

				Console.WriteLine("singleProgressView:X "+ progressView.Progress);

				switch (element.LabelType)
				{
					case LingvoSingleProgressView.LabelTypeValue.NOfM: progressView.Text = element.Progress + "/" + element.MaxProgress; break;
					case LingvoSingleProgressView.LabelTypeValue.Percentual: progressView.Text = (int)(100.0 * element.Progress / (double)element.MaxProgress) + " %"; break;
				}

				labelType = element.LabelType;
			}
		}
		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);
			//since we are using sublayers, we have to layout them manually
			progressView.render();
		}
	}
}
