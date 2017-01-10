using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Lingvo.MobileApp.Droid.Views;

[assembly: ExportRenderer(typeof(Lingvo.MobileApp.LingvoSingleProgressView), typeof(Lingvo.MobileApp.Droid.Renderers.LingvoSingleProgressViewRenderer))]
namespace Lingvo.MobileApp.Droid.Renderers
{
    class LingvoSingleProgressViewRenderer : ViewRenderer<LingvoSingleProgressView, Android.Views.View>
    {
        AndroidLingvoSingleProgressView progressView;
        LingvoSingleProgressView.LabelTypeValue labelType;

        protected override void OnElementChanged(ElementChangedEventArgs<LingvoSingleProgressView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                progressView = new AndroidLingvoSingleProgressView(Context);
                SetNativeControl(progressView);
                labelType = e.NewElement.LabelType;
            }

            if (e.OldElement != null && e.NewElement == null)
            {
                e.OldElement.PropertyChanged -= updateView;
            }
            else if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += updateView;
            }
        }

        private void updateView(object sender, EventArgs e)
        {
            LingvoSingleProgressView element = (LingvoSingleProgressView)sender;
         
            if (progressView.Size != element.Size)
                progressView.Size = element.Size;
            if (!progressView.ProgressColor.Equals(element.ProgressColor.ToAndroid()))
                progressView.ProgressColor = element.ProgressColor.ToAndroid();
            if (progressView.Max != element.MaxProgress)
                progressView.Max = element.MaxProgress;

            if (progressView.Progress != element.Progress || !labelType.Equals(element.LabelType))
            {
                if (progressView.Progress != element.Progress)
                    progressView.Progress = element.Progress;

                switch (element.LabelType)
                {
                    case LingvoSingleProgressView.LabelTypeValue.NOfM: progressView.Text = element.Progress + "/" + element.MaxProgress; break;
                    case LingvoSingleProgressView.LabelTypeValue.Percentual: progressView.Text = (int)(100.0 * element.Progress / (double)element.MaxProgress) + " %"; break;
                }

                labelType = element.LabelType;
            }
        }
    }
}