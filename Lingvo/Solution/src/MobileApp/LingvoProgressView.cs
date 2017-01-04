using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lingvo.MobileApp
{
    public class LingvoProgressView : View
    {
        public static readonly BindableProperty StudentTrackProgressProperty = BindableProperty.Create(
            propertyName: "StudentTrackProgress",
            returnType: typeof(int),
            declaringType: typeof(LingvoProgressView),
            defaultValue: 0);

        public static readonly BindableProperty TeacherTrackProgressProperty = BindableProperty.Create(
            propertyName: "TeacherTrackProgress",
            returnType: typeof(int),
            declaringType: typeof(LingvoProgressView),
            defaultValue: 0);

        public static readonly BindableProperty MaxProgressProperty = BindableProperty.Create(
            propertyName: "MaxProgress",
            returnType: typeof(int),
            declaringType: typeof(LingvoProgressView),
            defaultValue: 100);

        public static readonly BindableProperty StudentTrackColorProperty = BindableProperty.Create(
            propertyName: "StudentTrackColor",
            returnType: typeof(Color),
            declaringType: typeof(LingvoProgressView),
            defaultValue: Color.Orange);

        public static readonly BindableProperty TeacherTrackColorProperty = BindableProperty.Create(
            propertyName: "TeacherTrackColor",
            returnType: typeof(Color),
            declaringType: typeof(LingvoProgressView),
            defaultValue: Color.Aqua);

        public static readonly BindableProperty SizeProperty = BindableProperty.Create(
           propertyName: "Size",
           returnType: typeof(int),
           declaringType: typeof(LingvoProgressView),
           defaultValue: 40);

        public static readonly BindableProperty LabelTypeProperty = BindableProperty.Create(
          propertyName: "LabelType",
          returnType: typeof(LabelTypeValue),
          declaringType: typeof(LingvoProgressView),
          defaultValue: LabelTypeValue.Percentual);

        public int StudentTrackProgress
        {
            get { return (int)GetValue(StudentTrackProgressProperty); }
            set { SetValue(StudentTrackProgressProperty, value); }
        }

        public int TeacherTrackProgress
        {
            get { return (int)GetValue(TeacherTrackProgressProperty); }
            set { SetValue(TeacherTrackProgressProperty, value); }
        }

        public int MaxProgress
        {
            get { return (int)GetValue(MaxProgressProperty); }
            set { SetValue(MaxProgressProperty, value); }
        }

        public Color StudentTrackColor
        {
            get { return (Color)GetValue(StudentTrackColorProperty); }
            set { SetValue(StudentTrackColorProperty, value); }
        }

        public Color TeacherTrackColor
        {
            get { return (Color)GetValue(TeacherTrackColorProperty); }
            set { SetValue(TeacherTrackColorProperty, value); }
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
            NOfM, Percentual, Time 
        }
    }
}
