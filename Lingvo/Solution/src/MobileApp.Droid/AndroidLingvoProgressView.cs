using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;

namespace Lingvo.MobileApp.Droid
{
    class AndroidLingvoProgressView
    {
        private CircleProgressBar studentProgressBar;
        private CircleProgressBar teacherProgressBar;
        
        private View view;
        public View View
        {
            get { return view; }
        }

        public string Label
        {
            get { return studentProgressBar.Text; }
            set { studentProgressBar.Text = value; }
        }

        public int StudentProgress
        {
            get { return (int)studentProgressBar.Progress; }
            set {
                studentProgressBar.Progress = value;
            }
        }

        public int TeacherProgress
        {
            get { return (int)teacherProgressBar.Progress; }
            set {
                teacherProgressBar.Progress = value;
            }
        }

        public int Max
        {
            get { return studentProgressBar.Max; }
            set { studentProgressBar.Max = value;
                teacherProgressBar.Max = value;  }
        }

        public Color StudentColor
        {
            get { return studentProgressBar.FinishedStrokeColor; }
            set {
                studentProgressBar.FinishedStrokeColor = value;
                studentProgressBar.UnfinishedStrokeColor = Color.Argb(64, value.R, value.G, value.B);
            }
        }

        public Color TeacherColor
        {
            get { return teacherProgressBar.FinishedStrokeColor; }
            set
            {
                teacherProgressBar.FinishedStrokeColor = value;
                teacherProgressBar.UnfinishedStrokeColor = Color.Argb(64, value.R, value.G, value.B);
            }
        }

        public int Size
        {
            get {
                return teacherProgressBar.LayoutParameters.Width;
            }
            set
            {
                studentProgressBar.FinishedStrokeWidth = value * 0.05f;
                teacherProgressBar.FinishedStrokeWidth = value * 0.05f;
                studentProgressBar.UnfinishedStrokeWidth = value * 0.05f;
                teacherProgressBar.UnfinishedStrokeWidth = value * 0.05f;

                teacherProgressBar.LayoutParameters.Width = teacherProgressBar.LayoutParameters.Height = value;
                studentProgressBar.LayoutParameters.Width = studentProgressBar.LayoutParameters.Height = value - (int)(2.0 * teacherProgressBar.FinishedStrokeWidth);

                studentProgressBar.TextSize = value * 0.25f;

                teacherProgressBar.RequestLayout();
                studentProgressBar.RequestLayout();
            }
        }

        public AndroidLingvoProgressView(Context context)
        {
            this.view = View.Inflate(context, Resource.Layout.android_lingvo_progress_view, null);
            this.studentProgressBar = view.FindViewById<CircleProgressBar>(Resource.Id.studentProgressBar);
            this.teacherProgressBar = view.FindViewById<CircleProgressBar>(Resource.Id.teacherProgressBar);

            teacherProgressBar.ShowText = false;
            studentProgressBar.TextColor = Color.Black;

            teacherProgressBar.StartingDegree = -90;
            studentProgressBar.StartingDegree = -90;
        }
    }
}