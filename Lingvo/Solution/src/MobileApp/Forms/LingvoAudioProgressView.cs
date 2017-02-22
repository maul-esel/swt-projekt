using Xamarin.Forms;

namespace Lingvo.MobileApp
{
    /// <summary>
    /// A custom view for showing up to two progresses (the inner one can be disabled).
    /// Additionally, the progress can be shown in different representations <see cref="LabelTypeValue"/>.
    /// A mute button (which is visible if the inner progress is) provides a click event <see cref="StudentTrackMuted"/>.
    /// </summary>
    public class LingvoAudioProgressView : View
    {
        public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
           propertyName: "Progress",
           returnType: typeof(int),
           declaringType: typeof(LingvoAudioProgressView),
           defaultValue: 0);

        public static readonly BindableProperty MaxProgressProperty = BindableProperty.Create(
            propertyName: "MaxProgress",
            returnType: typeof(int),
            declaringType: typeof(LingvoAudioProgressView),
            defaultValue: 100);

        public static readonly BindableProperty InnerProgressColorProperty = BindableProperty.Create(
            propertyName: "InnerProgressColor",
            returnType: typeof(Color),
            declaringType: typeof(LingvoAudioProgressView),
            defaultValue: Color.Aqua);

        public static readonly BindableProperty OuterProgressColorProperty = BindableProperty.Create(
            propertyName: "OuterProgressColor",
            returnType: typeof(Color),
            declaringType: typeof(LingvoAudioProgressView),
            defaultValue: Color.Teal);

        public static readonly BindableProperty SizeProperty = BindableProperty.Create(
           propertyName: "Size",
           returnType: typeof(int),
           declaringType: typeof(LingvoAudioProgressView),
           defaultValue: 0);

        public static readonly BindableProperty InnerProgressEnabledProperty = BindableProperty.Create(
           propertyName: "InnerProgressEnabled",
           returnType: typeof(bool),
           declaringType: typeof(LingvoAudioProgressView),
           defaultValue: true);

        public static readonly BindableProperty MuteEnabledProperty = BindableProperty.Create(
           propertyName: "MuteEnabled",
           returnType: typeof(bool),
           declaringType: typeof(LingvoAudioProgressView),
           defaultValue: true);

        public static readonly BindableProperty LabelTypeProperty = BindableProperty.Create(
          propertyName: "LabelType",
          returnType: typeof(LabelTypeValue),
          declaringType: typeof(LingvoAudioProgressView),
          defaultValue: LabelTypeValue.Time);

        public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(
           propertyName: "TextSize",
           returnType: typeof(int),
           declaringType: typeof(LingvoAudioProgressView),
           defaultValue: 28);

        public delegate void StudentTrackMutedEventHandler(bool muted);

        /// <summary>
        /// An event fired when the mute button was pressed, delivering the state of the button.
        /// </summary>
        public event StudentTrackMutedEventHandler StudentTrackMuted;

        /// <summary>
        /// A property for enabling or disabling the inner progress.
        /// </summary>
        public bool InnerProgressEnabled
        {
            get { return (bool)GetValue(InnerProgressEnabledProperty); }
            set { SetValue(InnerProgressEnabledProperty, value); }
        }

        /// <summary>
        /// The current progress.
        /// Be careful: If <see cref="MaxProgress"/> is not set before, this will have no effect!
        /// </summary>
        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set
            {
                SetValue(ProgressProperty, value);
            }
        }

        /// <summary>
        /// The text size of the progress label.
        /// </summary>
        public int TextSize
        {
            get { return (int)GetValue(TextSizeProperty); }
            set
            {
                SetValue(TextSizeProperty, value);
            }
        }

        /// <summary>
        /// The maximum progress. 
        /// </summary>
        public int MaxProgress
        {
            get { return (int)GetValue(MaxProgressProperty); }
            set { SetValue(MaxProgressProperty, value); }
        }

        /// <summary>
        /// The color of the inner progress.
        /// </summary>
        public Color InnerProgressColor
        {
            get { return (Color)GetValue(InnerProgressColorProperty); }
            set { SetValue(InnerProgressColorProperty, value); }
        }

        /// <summary>
        /// The color of the outer progress.
        /// </summary>
        public Color OuterProgressColor
        {
            get { return (Color)GetValue(OuterProgressColorProperty); }
            set { SetValue(OuterProgressColorProperty, value); }
        }

        /// <summary>
        /// The size of the whole view.
        /// </summary>
        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Determines if the mute button is visible if the inner progress exists.
        /// </summary>
        public bool MuteEnabled
        {
            get { return (bool)GetValue(MuteEnabledProperty); }
            set { SetValue(MuteEnabledProperty, value); }
        }

        public void OnStudentTrackMuted(bool muted)
        {
            StudentTrackMuted?.Invoke(muted);
        }

        /// <summary>
        /// The representation of the label text.
        /// </summary>
        public LabelTypeValue LabelType
        {
            get { return (LabelTypeValue)GetValue(LabelTypeProperty); }
            set { SetValue(LabelTypeProperty, value); }
        }

        /// <summary>
        /// Different text representations.
        /// <para>NOfM: <c>"N / M"</c></para>
        /// <para>Percentual: <c>"X %"</c></para>
        /// <para>Time: <c>"mm:SS"</c></para>
        /// <para>None: <c>""</c></para>
        /// <para>Error: <c>"!"</c></para>
        /// </summary>
        public enum LabelTypeValue
        {
            NOfM, Percentual, Time, None, Error
        }
    }
}
