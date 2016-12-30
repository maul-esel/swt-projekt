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

namespace Lingvo.MobileApp.Droid
{
    class AndroidLingvoProgressView : View
    {
        private ProgressBar studentProgressBar;
        private ProgressBar teacherProgressBar;
        private Color studentColor, teacherColor;
        public TextView label { get; set; }

        public int StudentProgress
        {
            get { return studentProgressBar.Progress; }
            set { studentProgressBar.Progress = value; }
        }

        public int TeacherProgress
        {
            get { return teacherProgressBar.Progress; }
            set { teacherProgressBar.Progress = value; }
        }

        public int Max
        {
            get { return studentProgressBar.Max; }
            set { studentProgressBar.Max = value;
                teacherProgressBar.Max = value;  }
        }

        public Color StudentColor
        {
            get { return studentColor; }
            set {
                studentColor = value;
                studentProgressBar.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(value); }
        }

        public Color TeacherColor
        {
            get { return teacherColor; }
            set
            {
                teacherColor = value;
                teacherProgressBar.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(value);
            }
        }

        public AndroidLingvoProgressView(Context context) : base(context)
        {
            studentProgressBar = new ProgressBar(context);
            teacherProgressBar = new ProgressBar(context);
            label = new TextView(context);
            this.LayoutParameters = new ViewGroup.LayoutParams(-1, -1);
            studentProgressBar.ProgressDrawable = Resources.GetDrawable(Resource.Drawable.lingvo_progress_view_drawable);
            teacherProgressBar.ProgressDrawable = Resources.GetDrawable(Resource.Drawable.lingvo_progress_view_drawable);

            
        }

        public override void OnDrawForeground(Canvas canvas)
        {
            base.OnDrawForeground(canvas);
            teacherProgressBar.OnDrawForeground(canvas);
            studentProgressBar.OnDrawForeground(canvas);
            label.OnDrawForeground(canvas);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            int PROGRESSBAR_WIDTH = (int)Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, 5, Resources.DisplayMetrics);
            teacherProgressBar.Layout(l, t, r, b);
            studentProgressBar.Layout(l + PROGRESSBAR_WIDTH, t + PROGRESSBAR_WIDTH, r - 2 * PROGRESSBAR_WIDTH, b - 2 * PROGRESSBAR_WIDTH);
            label.Left = 2 * PROGRESSBAR_WIDTH;
            label.Top = 2 * PROGRESSBAR_WIDTH;
            label.LayoutParameters = new ViewGroup.LayoutParams(r - l - 4 * PROGRESSBAR_WIDTH, b - t - 4 * PROGRESSBAR_WIDTH);
            label.RequestLayout();
        }
    }
}