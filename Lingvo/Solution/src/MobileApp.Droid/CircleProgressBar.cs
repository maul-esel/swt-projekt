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
using Android.Util;
using Android.Graphics;
using Android.Content.Res;
using Android.Text;

namespace Lingvo.MobileApp.Droid
{
    [Register("lingvo.mobileapp.droid.CircleProgressBar")]
    public class CircleProgressBar : View
    {
        private Paint finishedPaint;
        private Paint unfinishedPaint;
        private Paint innerCirclePaint;

        protected Paint textPaint;
        protected Paint innerBottomTextPaint;

        private RectF finishedOuterRect = new RectF();
        private RectF unfinishedOuterRect = new RectF();

        private int attributeResourceId = 0;
        private bool showText;
        private float textSize;
        private Color textColor;
        private Color innerBottomTextColor;
        private float progress = 0;
        private int max;
        private Color finishedStrokeColor;
        private Color unfinishedStrokeColor;
        private int startingDegree;
        private float finishedStrokeWidth;
        private float unfinishedStrokeWidth;
        private Color innerBackgroundColor;
        private string prefixText = "";
        private string suffixText = "%";
        private string text = null;
        private float innerBottomTextSize;
        private string innerBottomText;
        private float innerBottomTextHeight;

        private float default_stroke_width;
        private int default_finished_color = Color.Rgb(66, 145, 241);
        private int default_unfinished_color = Color.Rgb(204, 204, 204);
        private int default_text_color = Color.Rgb(66, 145, 241);
        private int default_inner_bottom_text_color = Color.Rgb(66, 145, 241);
        private int default_inner_background_color = Color.Transparent;
        private int default_max = 100;
        private int default_startingDegree = 0;
        private float default_text_size;
        private float default_inner_bottom_text_size;
        private int min_size;


        private static readonly string INSTANCE_STATE = "saved_instance";
        private static readonly string INSTANCE_TEXT_COLOR = "text_color";
        private static readonly string INSTANCE_TEXT_SIZE = "text_size";
        private static readonly string INSTANCE_TEXT = "text";
        private static readonly string INSTANCE_INNER_BOTTOM_TEXT_SIZE = "inner_bottom_text_size";
        private static readonly string INSTANCE_INNER_BOTTOM_TEXT = "inner_bottom_text";
        private static readonly string INSTANCE_INNER_BOTTOM_TEXT_COLOR = "inner_bottom_text_color";
        private static readonly string INSTANCE_FINISHED_STROKE_COLOR = "finished_stroke_color";
        private static readonly string INSTANCE_UNFINISHED_STROKE_COLOR = "unfinished_stroke_color";
        private static readonly string INSTANCE_MAX = "max";
        private static readonly string INSTANCE_PROGRESS = "progress";
        private static readonly string INSTANCE_SUFFIX = "suffix";
        private static readonly string INSTANCE_PREFIX = "prefix";
        private static readonly string INSTANCE_FINISHED_STROKE_WIDTH = "finished_stroke_width";
        private static readonly string INSTANCE_UNFINISHED_STROKE_WIDTH = "unfinished_stroke_width";
        private static readonly string INSTANCE_BACKGROUND_COLOR = "inner_background_color";
        private static readonly string INSTANCE_STARTING_DEGREE = "starting_degree";
        private static readonly string INSTANCE_INNER_DRAWABLE = "inner_drawable";

        public CircleProgressBar(Context context) : this(context, null) { }

        public CircleProgressBar(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }

        public CircleProgressBar(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            default_text_size = TypedValue.ApplyDimension(ComplexUnitType.Sp, 18, Resources.DisplayMetrics);
            min_size = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 100, Resources.DisplayMetrics);
            default_stroke_width = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, Resources.DisplayMetrics);
            default_inner_bottom_text_size = TypedValue.ApplyDimension(ComplexUnitType.Sp, 18, Resources.DisplayMetrics);

            TypedArray attributes = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.CircleProgressBar, defStyleAttr, 0);
            initByAttributes(attributes);
            attributes.Recycle();

            initPainters();
        }

        public CircleProgressBar(Context context, IAttributeSet attrs, int defStyleAttr) : this(context, attrs, defStyleAttr, 0)
        {
        }

        protected void initPainters()
        {
            if (showText)
            {
                textPaint = new TextPaint();
                textPaint.Color = textColor;
                textPaint.TextSize = textSize;
                textPaint.AntiAlias = true;

                innerBottomTextPaint = new TextPaint();
                innerBottomTextPaint.Color = innerBottomTextColor;
                innerBottomTextPaint.TextSize = innerBottomTextSize;
                innerBottomTextPaint.AntiAlias = true;
            }

            finishedPaint = new Paint();
            finishedPaint.Color = finishedStrokeColor;
            finishedPaint.SetStyle(Paint.Style.Stroke);
            finishedPaint.AntiAlias = true;
            finishedPaint.StrokeWidth = finishedStrokeWidth;

            unfinishedPaint = new Paint();
            unfinishedPaint.Color = unfinishedStrokeColor;
            unfinishedPaint.SetStyle(Paint.Style.Stroke);
            unfinishedPaint.AntiAlias = true;
            unfinishedPaint.StrokeWidth = unfinishedStrokeWidth;

            innerCirclePaint = new Paint();
            innerCirclePaint.Color = innerBackgroundColor;
            innerCirclePaint.AntiAlias = true;
        }

        protected void initByAttributes(TypedArray attributes)
        {
            finishedStrokeColor = attributes.GetColor(Resource.Styleable.CircleProgressBar_cpb_finished_color, default_finished_color);
            unfinishedStrokeColor = attributes.GetColor(Resource.Styleable.CircleProgressBar_cpb_unfinished_color, default_unfinished_color);
            showText = attributes.GetBoolean(Resource.Styleable.CircleProgressBar_cpb_show_text, true);
            attributeResourceId = attributes.GetResourceId(Resource.Styleable.CircleProgressBar_cpb_inner_drawable, 0);

            Max = attributes.GetInt(Resource.Styleable.CircleProgressBar_cpb_max, default_max);
            Progress = attributes.GetFloat(Resource.Styleable.CircleProgressBar_cpb_progress, 0);
            finishedStrokeWidth = attributes.GetDimension(Resource.Styleable.CircleProgressBar_cpb_finished_stroke_width, default_stroke_width);
            unfinishedStrokeWidth = attributes.GetDimension(Resource.Styleable.CircleProgressBar_cpb_unfinished_stroke_width, default_stroke_width);

            if (showText)
            {
                if (attributes.GetString(Resource.Styleable.CircleProgressBar_cpb_prefix_text) != null)
                {
                    prefixText = attributes.GetString(Resource.Styleable.CircleProgressBar_cpb_prefix_text);
                }
                if (attributes.GetString(Resource.Styleable.CircleProgressBar_cpb_suffix_text) != null)
                {
                    suffixText = attributes.GetString(Resource.Styleable.CircleProgressBar_cpb_suffix_text);
                }
                if (attributes.GetString(Resource.Styleable.CircleProgressBar_cpb_text) != null)
                {
                    text = attributes.GetString(Resource.Styleable.CircleProgressBar_cpb_text);
                }

                textColor = attributes.GetColor(Resource.Styleable.CircleProgressBar_cpb_text_color, default_text_color);
                textSize = attributes.GetDimension(Resource.Styleable.CircleProgressBar_cpb_text_size, default_text_size);
                innerBottomTextSize = attributes.GetDimension(Resource.Styleable.CircleProgressBar_cpb_inner_bottom_text_size, default_inner_bottom_text_size);
                innerBottomTextColor = attributes.GetColor(Resource.Styleable.CircleProgressBar_cpb_inner_bottom_text_color, default_inner_bottom_text_color);
                innerBottomText = attributes.GetString(Resource.Styleable.CircleProgressBar_cpb_inner_bottom_text);
            }

            innerBottomTextSize = attributes.GetDimension(Resource.Styleable.CircleProgressBar_cpb_inner_bottom_text_size, default_inner_bottom_text_size);
            innerBottomTextColor = attributes.GetColor(Resource.Styleable.CircleProgressBar_cpb_inner_bottom_text_color, default_inner_bottom_text_color);
            innerBottomText = attributes.GetString(Resource.Styleable.CircleProgressBar_cpb_inner_bottom_text);

            startingDegree = attributes.GetInt(Resource.Styleable.CircleProgressBar_cpb_circle_starting_degree, default_startingDegree);
            innerBackgroundColor = attributes.GetColor(Resource.Styleable.CircleProgressBar_cpb_background_color, default_inner_background_color);
        }

        public override void Invalidate()
        {
            initPainters();
            base.Invalidate();
        }

        public bool ShowText
        {
            get { return showText; }
            set { showText = value; }
        }

        public float FinishedStrokeWidth
        {
            get { return finishedStrokeWidth; }
            set {
                if (value > default_stroke_width)
                {
                    finishedStrokeWidth = value; Invalidate();
                }
            }
        }

        public float UnfinishedStrokeWidth
        {
            get { return unfinishedStrokeWidth; }
            set {
                if (value > default_stroke_width)
                {
                    unfinishedStrokeWidth = value; Invalidate();
                }
            }
        }

        public float Progress
        {
            get { return progress; }
            set {
                this.progress = value;
                if (progress > Max)
                {
                    progress %= Max;
                }
                Invalidate();
            }
        }

        private float GetProgressAngle()
        {
            return Progress / (float)max * 360f;
        }


        public int Max
        {
            get { return max; }
            set {
                if (value > 0)
                {
                    max = value;
                    Invalidate();
                }
            }
        }

        public float TextSize
        {
            get { return textSize; }
            set {
                if (value > default_text_size)
                {
                    textSize = value;
                    Invalidate();
                }
            }
        }

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value;
                Invalidate();
            }
        }

        public Color FinishedStrokeColor
        {
            get { return finishedStrokeColor; }
            set { finishedStrokeColor = value;
                Invalidate();
            }
        }

        public Color UnfinishedStrokeColor
        {
            get { return unfinishedStrokeColor; }
            set
            {
                unfinishedStrokeColor = value;
                Invalidate();
            }
        }

        public string Text
        {
            get { return text; }
            set { text = value;
                Invalidate();
            }
        }

        public string SuffixText
        {
            get { return suffixText; }
            set { suffixText = value;
                Invalidate();
            }
        }

        public string PrefixText
        {
            get { return prefixText; }
            set
            {
                prefixText = value;
                Invalidate();
            }
        }

        public Color InnerBackgroundColor
        {
            get { return innerBackgroundColor; }
            set
            {
                innerBackgroundColor = value;
                Invalidate();
            }
        }

        public string InnerBottomText
        {
            get { return innerBottomText; }
            set { innerBottomText = value;
                Invalidate();
            }
        }

        public float InnerBottomTextSize
        {
            get { return innerBottomTextSize; }
            set
            {
                innerBottomTextSize = value;
                Invalidate();
            }
        }

        public Color InnerBottomTextColor
        {
            get { return innerBottomTextColor; }
            set { innerBottomTextColor = value;
                Invalidate();
            }
        }

        public int StartingDegree
        {
            get { return startingDegree; }
            set { startingDegree = value;
                Invalidate();
            }
        }

        public int AttributeResourceId
        {
            get { return attributeResourceId; }
            set { attributeResourceId = value;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            SetMeasuredDimension(measure(widthMeasureSpec), measure(heightMeasureSpec));

            //TODO calculate inner circle height and then position bottom text at the bottom (3/4)
            innerBottomTextHeight = Height - (Height * 3) / 4;
        }

        private int measure(int measureSpec)
        {
            int result;
            MeasureSpecMode mode = MeasureSpec.GetMode(measureSpec);
            int size = MeasureSpec.GetSize(measureSpec);
            if (mode == MeasureSpecMode.Exactly)
            {
                result = size;
            }
            else
            {
                result = min_size;
                if (mode == MeasureSpecMode.AtMost)
                {
                    result = Math.Min(result, size);
                }
            }
            return result;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            float delta = Math.Max(finishedStrokeWidth, unfinishedStrokeWidth);
            finishedOuterRect.Set(delta,
                    delta,
                    Width - delta,
                    Height - delta);
            unfinishedOuterRect.Set(delta,
                    delta,
                    Width - delta,
                    Height - delta);

            float innerCircleRadius = (Width - Math.Min(finishedStrokeWidth, unfinishedStrokeWidth) + Math.Abs(finishedStrokeWidth - unfinishedStrokeWidth)) / 2f;
            canvas.DrawCircle(Width / 2.0f, Height / 2.0f, innerCircleRadius, innerCirclePaint);
            canvas.DrawArc(finishedOuterRect, StartingDegree, GetProgressAngle(), false, finishedPaint);
            canvas.DrawArc(unfinishedOuterRect, StartingDegree + GetProgressAngle(), 360 - GetProgressAngle(), false, unfinishedPaint);

            if (showText)
            {
                string text = this.text != null ? this.text : prefixText + progress + suffixText;
                if (!TextUtils.IsEmpty(text))
                {
                    textPaint.TextSize = textSize;
                    textPaint.TextSize /= textPaint.MeasureText(text) / (2 * innerCircleRadius - 6 * FinishedStrokeWidth);
                    float textHeight = textPaint.Descent() + textPaint.Ascent();
                    canvas.DrawText(text, (Width - textPaint.MeasureText(text)) / 2.0f, (Width - textHeight) / 2.0f, textPaint);
                }

                if (!TextUtils.IsEmpty(InnerBottomText))
                {
                    innerBottomTextPaint.TextSize = innerBottomTextSize;
                    float bottomTextBaseline = Height - innerBottomTextHeight - (textPaint.Descent() + textPaint.Ascent()) / 2;
                    canvas.DrawText(InnerBottomText, (Width - innerBottomTextPaint.MeasureText(InnerBottomText)) / 2.0f, bottomTextBaseline, innerBottomTextPaint);
                }
            }

            if (attributeResourceId != 0)
            {
                Bitmap bitmap = BitmapFactory.DecodeResource(Resources, attributeResourceId);
                canvas.DrawBitmap(bitmap, (Width - bitmap.Width) / 2.0f, (Height - bitmap.Height) / 2.0f, null);
            }
        }

        protected override IParcelable OnSaveInstanceState()
        {
            Bundle bundle = new Bundle();
            bundle.PutParcelable(INSTANCE_STATE, base.OnSaveInstanceState());
            bundle.PutInt(INSTANCE_TEXT_COLOR, TextColor.ToArgb());
            bundle.PutFloat(INSTANCE_TEXT_SIZE, TextSize);
            bundle.PutFloat(INSTANCE_INNER_BOTTOM_TEXT_SIZE, InnerBottomTextSize);
            bundle.PutInt(INSTANCE_INNER_BOTTOM_TEXT_COLOR, InnerBottomTextColor.ToArgb());
            bundle.PutString(INSTANCE_INNER_BOTTOM_TEXT, InnerBottomText);
            bundle.PutInt(INSTANCE_FINISHED_STROKE_COLOR, FinishedStrokeColor.ToArgb());
            bundle.PutInt(INSTANCE_UNFINISHED_STROKE_COLOR, UnfinishedStrokeColor.ToArgb());
            bundle.PutInt(INSTANCE_MAX, Max);
            bundle.PutInt(INSTANCE_STARTING_DEGREE, StartingDegree);
            bundle.PutFloat(INSTANCE_PROGRESS, Progress);
            bundle.PutString(INSTANCE_SUFFIX, SuffixText);
            bundle.PutString(INSTANCE_PREFIX, PrefixText);
            bundle.PutString(INSTANCE_TEXT, Text);
            bundle.PutFloat(INSTANCE_FINISHED_STROKE_WIDTH, FinishedStrokeWidth);
            bundle.PutFloat(INSTANCE_UNFINISHED_STROKE_WIDTH, UnfinishedStrokeWidth);
            bundle.PutInt(INSTANCE_BACKGROUND_COLOR, InnerBackgroundColor);
            bundle.PutInt(INSTANCE_INNER_DRAWABLE, AttributeResourceId);
            return bundle;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (state is Bundle) {
                Bundle bundle = (Bundle)state;
                int colorValue = bundle.GetInt(INSTANCE_TEXT_COLOR);
                textColor = Color.Argb(Color.GetAlphaComponent(colorValue), Color.GetRedComponent(colorValue), Color.GetGreenComponent(colorValue), Color.GetBlueComponent(colorValue));
                textSize = bundle.GetFloat(INSTANCE_TEXT_SIZE);
                innerBottomTextSize = bundle.GetFloat(INSTANCE_INNER_BOTTOM_TEXT_SIZE);
                innerBottomText = bundle.GetString(INSTANCE_INNER_BOTTOM_TEXT);
                colorValue = bundle.GetInt(INSTANCE_INNER_BOTTOM_TEXT_COLOR);
                innerBottomTextColor = Color.Argb(Color.GetAlphaComponent(colorValue), Color.GetRedComponent(colorValue), Color.GetGreenComponent(colorValue), Color.GetBlueComponent(colorValue));
                colorValue = bundle.GetInt(INSTANCE_FINISHED_STROKE_COLOR);
                finishedStrokeColor = Color.Argb(Color.GetAlphaComponent(colorValue), Color.GetRedComponent(colorValue), Color.GetGreenComponent(colorValue), Color.GetBlueComponent(colorValue));
                colorValue = bundle.GetInt(INSTANCE_UNFINISHED_STROKE_COLOR);
                unfinishedStrokeColor = Color.Argb(Color.GetAlphaComponent(colorValue), Color.GetRedComponent(colorValue), Color.GetGreenComponent(colorValue), Color.GetBlueComponent(colorValue));
                finishedStrokeWidth = bundle.GetFloat(INSTANCE_FINISHED_STROKE_WIDTH);
                unfinishedStrokeWidth = bundle.GetFloat(INSTANCE_UNFINISHED_STROKE_WIDTH);
                colorValue = bundle.GetInt(INSTANCE_BACKGROUND_COLOR);
                innerBackgroundColor = Color.Argb(Color.GetAlphaComponent(colorValue), Color.GetRedComponent(colorValue), Color.GetGreenComponent(colorValue), Color.GetBlueComponent(colorValue));
                attributeResourceId = bundle.GetInt(INSTANCE_INNER_DRAWABLE);
                initPainters();
                Max = bundle.GetInt(INSTANCE_MAX);
                StartingDegree = bundle.GetInt(INSTANCE_STARTING_DEGREE);
                Progress = bundle.GetFloat(INSTANCE_PROGRESS);
                prefixText = bundle.GetString(INSTANCE_PREFIX);
                suffixText = bundle.GetString(INSTANCE_SUFFIX);
                text = bundle.GetString(INSTANCE_TEXT);
                base.OnRestoreInstanceState((IParcelable)bundle.GetParcelable(INSTANCE_STATE));
                return;
            }
            base.OnRestoreInstanceState(state);
        }
    }
}