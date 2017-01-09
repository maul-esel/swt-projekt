using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
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
			}
			else if (e.NewElement != null)
			{
				e.NewElement.PropertyChanged += updateView;
				progressView.StudentTrackMuted += e.NewElement.OnStudentTrackMuted;
			}
		}

		private void updateView(object sender, EventArgs e)
		{
			LingvoAudioProgressView element = (LingvoAudioProgressView)sender;

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

			Console.WriteLine("frame = " + Frame);
			Console.WriteLine("size = " + progressView.Size);
		}
	}
}
