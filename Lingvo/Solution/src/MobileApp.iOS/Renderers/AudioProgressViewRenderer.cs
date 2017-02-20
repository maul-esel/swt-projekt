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

			if (e.OldElement != null)
			{
				//unregister event listeners
				e.OldElement.PropertyChanged -= updateView;
				progressView.StudentTrackMuted -= e.OldElement.OnStudentTrackMuted;
				e.OldElement.SizeChanged -= NewElementOnSizeChanged;
			}
			if (e.NewElement != null)
			{
				//add event listeners for property changes
				e.NewElement.PropertyChanged += updateView;
				progressView.StudentTrackMuted += e.NewElement.OnStudentTrackMuted;
				e.NewElement.SizeChanged += NewElementOnSizeChanged;
			}
		}

		private void updateView(object sender, EventArgs e)
		{
			if (Control == null)
			{
				return;
			}
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
			if (progressView.MuteEnabled != element.MuteEnabled)
				progressView.MuteEnabled = element.MuteEnabled;
			if (progressView.TextSize != element.TextSize)
				progressView.TextSize = element.TextSize;
	
			//calculate the label's time label string depending on the current progress
            switch (element.LabelType)
            {
                case LingvoAudioProgressView.LabelTypeValue.NOfM: progressView.Text = element.Progress + "/" + element.MaxProgress; break;
                case LingvoAudioProgressView.LabelTypeValue.Percentual:
					if (element.MaxProgress == 0)
					{
						progressView.Text = "±∞";
					}
					else
					{
						progressView.Text = (int)(100.0 * element.Progress / (double)element.MaxProgress) + " %";
					}
					break;
                case LingvoAudioProgressView.LabelTypeValue.Time:
                    {
						string minutes = ((element.Progress / 60000 < 10 ? "0" : "") + element.Progress / 60000).Substring(0,2);
						string seconds = (((element.Progress % 60000) / 1000 < 10 ? "0" : "") + (element.Progress % 60000) / 1000).Substring(0,2);

                        progressView.Text = minutes + ":" + seconds;

                        break;
                    }
                case LingvoAudioProgressView.LabelTypeValue.None: progressView.Text = ""; break;
            }

        }
		/// <summary>
		/// Gets fired when the view's frame has changed
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="eventArgs">Event arguments</param>
		private void NewElementOnSizeChanged(object sender, EventArgs eventArgs)
		{
			var audioProgressView = sender as LingvoAudioProgressView;

			if (audioProgressView != null)
			{
				var frame = new CGRect(audioProgressView.X, audioProgressView.X, audioProgressView.Width, audioProgressView.Height);
				var squareMeasure = Math.Min(audioProgressView.Width, audioProgressView.Height);
				var lineWidth = squareMeasure * 0.0625;
				progressView.LineWidth = (float)lineWidth;
				progressView.Frame = frame;
				progressView.render();

			}
		}
		/// <summary>
		/// Layout sublayers of the current view. This is needed because the progress view uses CAShapeLayer objects to render itself.
		/// The sublayers' frames must be set manually
		/// </summary>
		/// <param name="layer">Layer</param>
		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);

			progressView.Frame = layer.Bounds;
			progressView.teacherProgressBar.Frame = layer.Bounds;
			progressView.studentProgressBar.Frame = layer.Bounds;
			progressView.teacherProgressBar.render();
			progressView.studentProgressBar.render();

		}
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

		}
	}
}
