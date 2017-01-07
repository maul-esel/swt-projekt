using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System;

namespace Lingvo.MobileApp.Droid
{
    class AndroidLingvoSingleProgressView : RelativeLayout
    {
        private CircleProgressBar progressBar;

        public string Text
        {
            get { return progressBar.Text; }
            set {
                progressBar.Text = value;
                Invalidate();
            }
        }

        public int Progress
        {
            get { return (int)progressBar.Progress; }
            set
            {
                progressBar.Progress = value;
                Invalidate();
            }
        }

        public int Max
        {
            get { return progressBar.Max; }
            set
            {
                progressBar.Max = value;
                Invalidate();
            }
        }

        public Color ProgressColor
        {
            get { return progressBar.FinishedStrokeColor; }
            set
            {
                progressBar.FinishedStrokeColor = value;
                progressBar.UnfinishedStrokeColor = Color.Argb(64, value.R, value.G, value.B);
                progressBar.TextColor = value;
                Invalidate();
            }
        }

        public int Size
        {
            get
            {
                return LayoutParameters.Width;
            }
            set
            {
                LayoutParameters.Width = LayoutParameters.Height = value;
                RequestLayout();
            }
        }

        public AndroidLingvoSingleProgressView(Context context) : base(context)
        {
            this.LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

            this.progressBar = new CircleProgressBar(context);

            progressBar.StartingDegree = -90;

            AddView(progressBar, 0, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));
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

            progressBar.Measure(childWidthMeasureSpec, childHeightMeasureSpec);

            int size = Math.Min(progressBar.MeasuredWidth, progressBar.MeasuredHeight);

            progressBar.FinishedStrokeWidth = size * 0.05f;
            progressBar.UnfinishedStrokeWidth = size * 0.05f;
            progressBar.TextSize = size * 0.25f;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int parentLeft = PaddingLeft;
            int parentRight = r - l - PaddingRight;
            int parentTop = PaddingTop;
            int parentBottom = b - t - PaddingBottom;

            int size = Math.Min(progressBar.MeasuredWidth, progressBar.MeasuredHeight);

            int childLeft = parentLeft + (parentRight - parentLeft - size) / 2;
            int childTop = parentTop + (parentBottom - parentTop - size) / 2;

            progressBar.Layout(childLeft, childTop, childLeft + size, childTop + size);
        }
    }
}