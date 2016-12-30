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
    class LingvoProgressViewRenderer : ViewRenderer<LingvoProgressView, AndroidLingvoProgressView>
    {
        AndroidLingvoProgressView progressView;

        protected override void OnElementChanged(ElementChangedEventArgs<LingvoProgressView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                progressView = new AndroidLingvoProgressView(Context);
                SetNativeControl(progressView);
            }

            if (e.OldElement != null)
            {
                progressView.TeacherColor = e.OldElement.TeacherTrackColor.ToAndroid();
                progressView.StudentColor = e.OldElement.StudentTrackColor.ToAndroid();
                progressView.TeacherProgress = e.OldElement.TeacherTrackProgress;
                progressView.StudentProgress = e.OldElement.StudentTrackProgress;
                progressView.Max = e.OldElement.MaxProgress;
            }
            if (e.NewElement != null)
            {
                progressView.TeacherColor = e.NewElement.TeacherTrackColor.ToAndroid();
                progressView.StudentColor = e.NewElement.StudentTrackColor.ToAndroid();
                progressView.TeacherProgress = e.NewElement.TeacherTrackProgress;
                progressView.StudentProgress = e.NewElement.StudentTrackProgress;
                progressView.Max = e.NewElement.MaxProgress;
            }
        }
    }
}