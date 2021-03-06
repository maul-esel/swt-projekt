using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Lingvo.MobileApp.Droid.Views;
using Android.Util;

[assembly: ExportRenderer(typeof(Lingvo.MobileApp.LingvoAudioProgressView), typeof(Lingvo.MobileApp.Droid.Renderers.LingvoAudioProgressViewRenderer))]
namespace Lingvo.MobileApp.Droid.Renderers
{
    /// <summary>
    /// The Android custom renderer for the <see cref="LingvoAudioProgressView"/>.
    /// </summary>
    class LingvoAudioProgressViewRenderer : ViewRenderer<LingvoAudioProgressView, Android.Views.View>
    {
        AndroidLingvoAudioProgressView progressView;

        /// <summary>
        /// Called on each instatiation of the <c>Xamarin.Forms.View</c>.
        /// Registers or unregisters event listeners and <see cref="updateView(object, EventArgs)"/>.
        /// </summary>
        /// <param name="e">The <c>ElementChangedEventArgs</c> containing the <see cref="LingvoAudioProgressView"/>.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<LingvoAudioProgressView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                progressView = new AndroidLingvoAudioProgressView(Context);
                SetNativeControl(progressView);
            }

            if (e.OldElement != null)
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

        /// <summary>
        /// Updates the <see cref="AndroidLingvoAudioProgressView"/> with information from <see cref="LingvoAudioProgressView"/>.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The <c>EventArgs</c>.</param>
        private void updateView(object sender, EventArgs e)
        {
            if (Control == null)
            {
                return;
            }
            LingvoAudioProgressView element = (LingvoAudioProgressView)sender;

            progressView.ContentDescription = element.AutomationId;

            //Set each attribute if it has changed
            if (progressView.InnerProgressEnabled != element.InnerProgressEnabled)
                progressView.InnerProgressEnabled = element.InnerProgressEnabled;
            if (progressView.InnerMuteButtonVisible != element.MuteEnabled)
                progressView.InnerMuteButtonVisible = element.MuteEnabled;
            if (progressView.Size != element.Size)
                progressView.Size = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, element.Size, Resources.DisplayMetrics);
            if (!progressView.OuterProgressColor.Equals(element.OuterProgressColor.ToAndroid()))
                progressView.OuterProgressColor = element.OuterProgressColor.ToAndroid();
            if (!progressView.InnerProgressColor.Equals(element.InnerProgressColor.ToAndroid()))
                progressView.InnerProgressColor = element.InnerProgressColor.ToAndroid();
            if (progressView.Max != element.MaxProgress)
                progressView.Max = element.MaxProgress;
            if (progressView.Progress != element.Progress)
                progressView.Progress = element.Progress;

            //Set the text of the progress view according to the label type
            switch (element.LabelType)
            {
                case LingvoAudioProgressView.LabelTypeValue.NOfM: progressView.Text = element.Progress + "/" + element.MaxProgress; break;
                case LingvoAudioProgressView.LabelTypeValue.Percentual:
                    if (element.MaxProgress == 0)
                    {
                        progressView.Text = "0 %";
                    }
                    else
                    {
                        progressView.Text = (int)(100.0 * element.Progress / (double)element.MaxProgress) + " %";
                    }
                    break;
                case LingvoAudioProgressView.LabelTypeValue.Time:
                    {
                        string minutes = ((element.Progress / 60000 < 10 ? "0" : "") + element.Progress / 60000).Substring(0, 2);
                        string seconds = (((element.Progress % 60000) / 1000 < 10 ? "0" : "") + (element.Progress % 60000) / 1000).Substring(0, 2);

                        progressView.Text = minutes + ":" + seconds;

                        break;
                    }
                case LingvoAudioProgressView.LabelTypeValue.None: progressView.Text = ""; break;
                case LingvoAudioProgressView.LabelTypeValue.Error: progressView.Text = "!"; break;
            }
        }
    }
}