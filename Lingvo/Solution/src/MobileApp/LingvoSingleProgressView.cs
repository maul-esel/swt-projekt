using Xamarin.Forms;

namespace Lingvo.MobileApp
{
    public class LingvoSingleProgressView : View
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

        public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(
            propertyName: "OuterProgressColor",
            returnType: typeof(Color),
            declaringType: typeof(LingvoAudioProgressView),
            defaultValue: Color.Teal);

        public static readonly BindableProperty SizeProperty = BindableProperty.Create(
           propertyName: "Size",
           returnType: typeof(int),
           declaringType: typeof(LingvoAudioProgressView),
           defaultValue: 0);

        public static readonly BindableProperty LabelTypeProperty = BindableProperty.Create(
          propertyName: "LabelType",
          returnType: typeof(LabelTypeValue),
          declaringType: typeof(LingvoAudioProgressView),
          defaultValue: LabelTypeValue.Percentual);

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public int MaxProgress
        {
            get { return (int)GetValue(MaxProgressProperty); }
            set { SetValue(MaxProgressProperty, value); }
        }

        public Color ProgressColor
        {
            get { return (Color)GetValue(ProgressColorProperty); }
            set { SetValue(ProgressColorProperty, value); }
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public LabelTypeValue LabelType
        {
            get { return (LabelTypeValue)GetValue(LabelTypeProperty); }
            set { SetValue(LabelTypeProperty, value); }
        }

        public enum LabelTypeValue
        {
            NOfM, Percentual
        }
    }
}
