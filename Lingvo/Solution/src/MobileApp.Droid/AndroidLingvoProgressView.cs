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
    class AndroidLingvoProgressView : RelativeLayout
    {
        private CircleProgressBar studentProgressBar;
        private CircleProgressBar teacherProgressBar;

        public string Text
        {
            get { return studentProgressBar.Text; }
            set { studentProgressBar.Text = value;
                Invalidate();
            }
        }

        public int InnerProgress
        {
            get { return (int)studentProgressBar.Progress; }
            set {
                studentProgressBar.Progress = value;
                Invalidate();
            }
        }

        public int OuterProgress
        {
            get { return (int)teacherProgressBar.Progress; }
            set {
                teacherProgressBar.Progress = value;
                Invalidate();
            }
        }

        public int Max
        {
            get { return studentProgressBar.Max; }
            set { studentProgressBar.Max = value;
                teacherProgressBar.Max = value;
                Invalidate();
            }
        }

        public Color InnerProgressColor
        {
            get { return studentProgressBar.FinishedStrokeColor; }
            set {
                studentProgressBar.FinishedStrokeColor = value;
                studentProgressBar.UnfinishedStrokeColor = Color.Argb(64, value.R, value.G, value.B);
                Invalidate();
            }
        }

        public Color OuterProgressColor
        {
            get { return teacherProgressBar.FinishedStrokeColor; }
            set
            {
                teacherProgressBar.FinishedStrokeColor = value;
                teacherProgressBar.UnfinishedStrokeColor = Color.Argb(64, value.R, value.G, value.B);
                studentProgressBar.TextColor = value;
                Invalidate();
            }
        }

        public int Size
        {
            get {
                return LayoutParameters.Width;
            }
            set
            {
                LayoutParameters.Width = LayoutParameters.Height = value;
                RequestLayout();
            }
        }

        public bool InnerProgressEnabled
        {
            get { return studentProgressBar.DrawStroke; }
            set {
                studentProgressBar.DrawStroke = value;
                Invalidate();
            }
        }

        public AndroidLingvoProgressView(Context context) : base(context)
        {
            this.LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

            this.studentProgressBar = new CircleProgressBar(context);
            this.teacherProgressBar = new CircleProgressBar(context);

            teacherProgressBar.ShowText = false;
            studentProgressBar.TextColor = Color.Black;

            teacherProgressBar.StartingDegree = studentProgressBar.StartingDegree = -90;
            teacherProgressBar.Text = "";

            AddView(teacherProgressBar, 0, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));
            AddView(studentProgressBar, 1, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int width = LayoutParameters.Width <= 0 ? Math.Max(0, MeasuredWidth
                            - PaddingLeft - PaddingRight) : LayoutParameters.Width;
            int height = LayoutParameters.Height <= 0 ? Math.Max(0, MeasuredHeight
                            - PaddingTop - PaddingBottom) : LayoutParameters.Height;
            int childWidthMeasureSpec = MeasureSpec.MakeMeasureSpec(
                   width, MeasureSpecMode.Exactly);
            int childHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(
                    height, MeasureSpecMode.Exactly);

            teacherProgressBar.Measure(childWidthMeasureSpec, childHeightMeasureSpec);

            int size = Math.Min(teacherProgressBar.MeasuredWidth, teacherProgressBar.MeasuredHeight);

            studentProgressBar.FinishedStrokeWidth = size * 0.05f;
            teacherProgressBar.FinishedStrokeWidth = size * 0.05f;
            studentProgressBar.UnfinishedStrokeWidth = size * 0.05f;
            teacherProgressBar.UnfinishedStrokeWidth = size * 0.05f;
            studentProgressBar.TextSize = size * 0.25f;

            int studentWidth = width - (int)(2.0 * teacherProgressBar.FinishedStrokeWidth);
            int studentHeight = height - (int)(2.0 * teacherProgressBar.FinishedStrokeWidth);
            int studentChildWidthMeasureSpec = MeasureSpec.MakeMeasureSpec(
                    studentWidth, MeasureSpecMode.Exactly);
            int studentChildHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(
                    studentHeight, MeasureSpecMode.Exactly);

            studentProgressBar.Measure(studentChildWidthMeasureSpec, studentChildHeightMeasureSpec);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int parentLeft = PaddingLeft;
            int parentRight = r - l - PaddingRight;
            int parentTop = PaddingTop;
            int parentBottom = b - t - PaddingBottom;

            int size = Math.Min(teacherProgressBar.MeasuredWidth, teacherProgressBar.MeasuredHeight);

            int childLeft = parentLeft + (parentRight - parentLeft - size) / 2;
            int childTop = parentTop + (parentBottom - parentTop - size) / 2;

            teacherProgressBar.Layout(childLeft, childTop, childLeft + size, childTop + size);

            int studentLeft = childLeft + (int)teacherProgressBar.FinishedStrokeWidth;
            int studentTop = childTop + (int)teacherProgressBar.FinishedStrokeWidth;
            int studentRight = childLeft + size - (int)(teacherProgressBar.FinishedStrokeWidth);
            int studentBottom = childTop + size - (int)(teacherProgressBar.FinishedStrokeWidth);
            studentProgressBar.Layout(studentLeft, studentTop, studentRight, studentBottom);
        }
    }
}