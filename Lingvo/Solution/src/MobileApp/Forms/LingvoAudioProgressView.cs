using Xamarin.Forms;

namespace Lingvo.MobileApp
{
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

        public delegate void StudentTrackMutedEventHandler(bool muted);

        public event StudentTrackMutedEventHandler StudentTrackMuted;

        public bool InnerProgressEnabled
        {
            get { return (bool)GetValue(InnerProgressEnabledProperty); }
            set { SetValue(InnerProgressEnabledProperty, value); }
        }

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set
            {
                SetValue(ProgressProperty, value);
            }
        }

        public int MaxProgress
        {
            get { return (int)GetValue(MaxProgressProperty); }
            set { SetValue(MaxProgressProperty, value); }
        }

        public Color InnerProgressColor
        {
            get { return (Color)GetValue(InnerProgressColorProperty); }
            set { SetValue(InnerProgressColorProperty, value); }
        }

        public Color OuterProgressColor
        {
            get { return (Color)GetValue(OuterProgressColorProperty); }
            set { SetValue(OuterProgressColorProperty, value); }
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public bool MuteEnabled
        {
            get { return (bool)GetValue(MuteEnabledProperty); }
            set { SetValue(MuteEnabledProperty, value); }
        }

        public void OnStudentTrackMuted(bool muted)
        {
            StudentTrackMuted?.Invoke(muted);
        }

        public LabelTypeValue LabelType
        {
            get { return (LabelTypeValue)GetValue(LabelTypeProperty); }
            set { SetValue(LabelTypeProperty, value); }
        }

        public enum LabelTypeValue
        {
            NOfM, Percentual, Time, None
        }
    }
}
