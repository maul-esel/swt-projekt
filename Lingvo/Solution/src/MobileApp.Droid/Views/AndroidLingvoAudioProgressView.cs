using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Support.Graphics.Drawable;
using Android.Support.V4.View.Animation;
using Android.Widget;
using System;
using Android.Util;

namespace Lingvo.MobileApp.Droid.Views
{
    class AndroidLingvoAudioProgressView : RelativeLayout
    {
        private CircleProgressBar studentProgressBar;
        private CircleProgressBar teacherProgressBar;
        private ImageButton studentMuteButton;
        private bool studentMuted;

        private static readonly int INSTANCE_STUDENT_MUTED = "student_muted".GetHashCode();

        public string Text
        {
            get { return studentProgressBar.Text; }
        }

        public int Progress
        {
            get { return (int)teacherProgressBar.Progress; }
            set
            {
                if (InnerProgressEnabled)
                {
                    studentProgressBar.Progress = value;
                }
                teacherProgressBar.Progress = value;

                string minutes = (value / 60 < 10 ? "0" : "") + value / 60;
                string seconds = (value % 60 < 10 ? "0" : "") + value % 60;

                studentProgressBar.Text = minutes + ":" + seconds;

                Invalidate();
            }
        }

        public int Max
        {
            get { return studentProgressBar.Max; }
            set
            {
                studentProgressBar.Max = value;
                teacherProgressBar.Max = value;
                Invalidate();
            }
        }

        public Color InnerProgressColor
        {
            get { return studentProgressBar.FinishedStrokeColor; }
            set
            {
                studentProgressBar.FinishedStrokeColor = value;
                studentProgressBar.UnfinishedStrokeColor = Color.Argb(64, value.R, value.G, value.B);
                studentMuteButton.SetColorFilter(value, PorterDuff.Mode.SrcIn);
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

        public bool InnerProgressEnabled
        {
            get { return studentProgressBar.DrawStroke; }
            set
            {
                studentProgressBar.DrawStroke = value;
                Invalidate();
            }
        }

        public bool InnerMuteButtonVisible
        {
            get { return studentMuteButton.Visibility == ViewStates.Visible; }
            set
            {
                studentMuteButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        public delegate void StudentTrackMutedEventHandler(bool muted);
        
        public event StudentTrackMutedEventHandler StudentTrackMuted;
        
        public AndroidLingvoAudioProgressView(Context context) : base(context)
        {
            this.LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

            //ProgressBars

            this.studentProgressBar = new CircleProgressBar(context);
            this.teacherProgressBar = new CircleProgressBar(context);

            teacherProgressBar.ShowText = false;
            studentProgressBar.TextColor = Color.Black;

            teacherProgressBar.StartingDegree = studentProgressBar.StartingDegree = -90;
            teacherProgressBar.Text = "";

            studentProgressBar.CenterText = false;

            Progress = 0;

            AddView(teacherProgressBar, 0, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));
            AddView(studentProgressBar, 1, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));

            //Mute Buttons

            studentMuted = false;

            studentMuteButton = new ImageButton(context)
            {
                Background = null
            };

            studentMuteButton.SetScaleType(ImageView.ScaleType.FitXy);

            SetButtonResource(studentMuteButton, studentMuted);

            studentMuteButton.Click += (o, e) =>
            {
                studentMuted = !studentMuted;
                StudentTrackMuted?.Invoke(studentMuted);
                SetButtonResource(studentMuteButton, studentMuted);
            };

            AddView(studentMuteButton, 2, new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent));
        }

        private void SetButtonResource(ImageButton button, bool isEnabled)
        {
            int resourceId = isEnabled ? Resource.Drawable.ic_volume_off_black_24px : Resource.Drawable.ic_volume_up_black_24px;
            button.SetImageDrawable(VectorDrawableCompat.Create(Resources, resourceId, Resources.NewTheme()));
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            MeasureChild(studentMuteButton, widthMeasureSpec, heightMeasureSpec);

            int width = LayoutParameters.Width <= 0 ? Math.Max(0, MeasuredWidth
                            - PaddingLeft - PaddingRight) : LayoutParameters.Width;
            int height = LayoutParameters.Height <= 0 ? Math.Max(0, MeasuredHeight
                            - PaddingTop - PaddingBottom) : LayoutParameters.Height;

            width = Math.Max(width, studentMuteButton.MeasuredWidth * 4);
            height = Math.Max(height, studentMuteButton.MeasuredHeight * 4);

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
            studentProgressBar.TextSize = size * 0.2f;

            int studentWidth = width - (int)(2.0 * teacherProgressBar.FinishedStrokeWidth);
            int studentHeight = height - (int)(2.0 * teacherProgressBar.FinishedStrokeWidth);
            int studentChildWidthMeasureSpec = MeasureSpec.MakeMeasureSpec(
                    studentWidth, MeasureSpecMode.Exactly);
            int studentChildHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(
                    studentHeight, MeasureSpecMode.Exactly);

            studentProgressBar.Measure(studentChildWidthMeasureSpec, studentChildHeightMeasureSpec);

            int buttonWidth = Math.Max((int)(0.2f * size), studentMuteButton.MeasuredWidth);
            int buttonHeight = Math.Max((int)(0.2f * size), studentMuteButton.MeasuredHeight);
            int muteWidthMeasureSpec = MeasureSpec.MakeMeasureSpec(
                    buttonWidth, MeasureSpecMode.Exactly);
            int muteHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(
                    buttonHeight, MeasureSpecMode.Exactly);
            studentMuteButton.Measure(muteWidthMeasureSpec, muteHeightMeasureSpec);
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

            studentMuteButton.Layout(childLeft + size / 2 - studentMuteButton.MeasuredWidth / 2, childTop + size / 2 + (int)(0.05f * size), childLeft + size / 2 + studentMuteButton.MeasuredWidth / 2, childTop + size / 2 + studentMuteButton.MeasuredHeight + (int)(0.05f * size));
        }

        public override void SaveHierarchyState(SparseArray container)
        {
            base.SaveHierarchyState(container);
            container.Append(INSTANCE_STUDENT_MUTED, studentMuted);
        }

        public override void RestoreHierarchyState(SparseArray container)
        {
            base.RestoreHierarchyState(container);
            studentMuted = (bool)container.Get(INSTANCE_STUDENT_MUTED);
        }
    }
}