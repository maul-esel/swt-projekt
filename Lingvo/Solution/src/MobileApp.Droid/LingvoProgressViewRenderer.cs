using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Lingvo.MobileApp.LingvoProgressView), typeof(Lingvo.MobileApp.Droid.LingvoProgressViewRenderer))]
namespace Lingvo.MobileApp.Droid
{
    class LingvoProgressViewRenderer : ViewRenderer<LingvoProgressView, Android.Views.View>
    {
        AndroidLingvoProgressView progressView;
        private LingvoProgressView.LabelTypeValue labelType;

        protected override void OnElementChanged(ElementChangedEventArgs<LingvoProgressView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                progressView = new AndroidLingvoProgressView(Context);
                labelType = e.NewElement.LabelType;
                SetNativeControl(progressView);
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
            LingvoProgressView element = (LingvoProgressView)sender;

            if (progressView.InnerProgressEnabled != element.InnerProgressEnabled)
                progressView.InnerProgressEnabled = element.InnerProgressEnabled;
            if (progressView.Size != element.Size)
                progressView.Size = element.Size;
            if (!progressView.OuterProgressColor.Equals(element.OuterProgressColor.ToAndroid()))
                progressView.OuterProgressColor = element.OuterProgressColor.ToAndroid();
            if (!progressView.InnerProgressColor.Equals(element.InnerProgressColor.ToAndroid()))
                progressView.InnerProgressColor = element.InnerProgressColor.ToAndroid();
            if (progressView.Max != element.MaxProgress)
                progressView.Max = element.MaxProgress;
            if (progressView.InnerProgress != element.InnerProgress)
                progressView.InnerProgress = element.InnerProgress;

            if (progressView.OuterProgress != element.OuterProgress || !labelType.Equals(element.LabelType))
            {
                if (progressView.OuterProgress != element.OuterProgress)
                    progressView.OuterProgress = element.OuterProgress;
                switch (element.LabelType)
                {
                    case LingvoProgressView.LabelTypeValue.NOfM: progressView.Text = element.OuterProgress + " / " + element.MaxProgress; break;
                    case LingvoProgressView.LabelTypeValue.Time: progressView.Text = (element.OuterProgress / 60 < 10 ? "0" : "") + element.OuterProgress / 60 + ":" + (element.OuterProgress % 60 < 10 ? "0" : "") + element.OuterProgress % 60; break;
                    default: progressView.Text = (int)(100 * ((double)element.OuterProgress) / element.MaxProgress) + " %"; break;
                }
                labelType = element.LabelType;
            }

        }
    }
}