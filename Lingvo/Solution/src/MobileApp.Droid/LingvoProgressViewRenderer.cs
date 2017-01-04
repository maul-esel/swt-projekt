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
                SetNativeControl(progressView.View);
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

            if (progressView.Size != element.Size)
                progressView.Size = element.Size;
            if (!progressView.TeacherColor.Equals(element.TeacherTrackColor.ToAndroid()))
                progressView.TeacherColor = element.TeacherTrackColor.ToAndroid();
            if (!progressView.StudentColor.Equals(element.StudentTrackColor.ToAndroid()))
                progressView.StudentColor = element.StudentTrackColor.ToAndroid();
            if (progressView.Max != element.MaxProgress)
                progressView.Max = element.MaxProgress;
            if (progressView.StudentProgress != element.StudentTrackProgress)
                progressView.StudentProgress = element.StudentTrackProgress;

            if (progressView.TeacherProgress != element.TeacherTrackProgress || !labelType.Equals(element.LabelType))
            {
                progressView.TeacherProgress = element.TeacherTrackProgress;

                switch (element.LabelType)
                {
                    case LingvoProgressView.LabelTypeValue.NOfM: progressView.Label = element.TeacherTrackProgress + " / " + element.MaxProgress; break;
                    case LingvoProgressView.LabelTypeValue.Time: progressView.Label = element.TeacherTrackProgress / 60 + ":" + element.TeacherTrackProgress % 60; break;
                    default: progressView.Label = (int)(100 * ((double)element.TeacherTrackProgress) / element.MaxProgress) + " %"; break;
                }
                labelType = element.LabelType;
            }

        }
    }
}