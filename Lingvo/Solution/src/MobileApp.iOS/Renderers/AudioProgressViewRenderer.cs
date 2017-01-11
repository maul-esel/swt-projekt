using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;
using UIKit;

[assembly: ExportRenderer(typeof(Lingvo.MobileApp.LingvoAudioProgressView), typeof(Lingvo.MobileApp.iOS.AudioProgressViewRenderer))]
namespace Lingvo.MobileApp.iOS
{
	class AudioProgressViewRenderer : ViewRenderer<LingvoAudioProgressView, UIView>
	{
		AudioProgressView progressView;

		protected override void OnElementChanged(ElementChangedEventArgs<LingvoAudioProgressView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				progressView = new AudioProgressView(Frame);
				SetNativeControl(progressView);
			}

			if (e.OldElement != null && e.NewElement == null)
			{
				e.OldElement.PropertyChanged -= updateView;
				progressView.StudentTrackMuted -= e.OldElement.OnStudentTrackMuted;
				e.OldElement.SizeChanged -= NewElementOnSizeChanged;
			}
			else if (e.NewElement != null)
			{

				e.NewElement.PropertyChanged += updateView;
				progressView.StudentTrackMuted += e.NewElement.OnStudentTrackMuted;
				e.NewElement.SizeChanged += NewElementOnSizeChanged;
			}
		}

		private void updateView(object sender, EventArgs e)
		{
			LingvoAudioProgressView element = (LingvoAudioProgressView)sender;
			if (element == null)
			{
				return;
			}

			Console.WriteLine("element x = " + element.X + "y = " + element.Y + "element.width = " + element.Width + "element.height = " + element.Height);

			if (progressView.InnerProgressEnabled != element.InnerProgressEnabled)
				progressView.InnerProgressEnabled = element.InnerProgressEnabled;
			if (progressView.Size != element.Size)
				progressView.Size = element.Size;
			if (!progressView.OuterProgressColor.Equals(element.OuterProgressColor.ToUIColor()))
				progressView.OuterProgressColor = element.OuterProgressColor.ToUIColor();
			if (!progressView.InnerProgressColor.Equals(element.InnerProgressColor.ToUIColor()))
				progressView.InnerProgressColor = element.InnerProgressColor.ToUIColor();
			if (progressView.MaxProgress != element.MaxProgress)
				progressView.MaxProgress = element.MaxProgress;
			if (progressView.Progress != element.Progress)
				progressView.Progress = element.Progress;
			if (progressView.InnerMuteButtonVisible != element.InnerProgressEnabled)
				progressView.InnerMuteButtonVisible = element.InnerProgressEnabled;

			Console.WriteLine("current frame = " + Frame);
		}
		private void NewElementOnSizeChanged(object sender, EventArgs eventArgs)
		{
			var audioProgressView = sender as LingvoAudioProgressView;

			if (audioProgressView != null)
			{
				var frame = new CGRect(audioProgressView.X, audioProgressView.X, audioProgressView.Width, audioProgressView.Height);
				Console.WriteLine("frame to be set = " + frame);
				progressView.Frame = frame;
			}
			else
			{
				Console.WriteLine("cast failed");
			}
		}
		protected void runOnMainThread(Action action)
		{
			//updates on UI only work on the main thread
			DispatchQueue.MainQueue.DispatchAsync(action);
		}
		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);
			progressView.Frame = layer.Bounds;
			Console.WriteLine("LayoutSublayersOfLayer measures = " + layer.Frame);

		}
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

		}
	}
}
