using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lingvo.MobileApp
{
    public class LingvoProgressView : View
    {
        public static readonly BindableProperty InnerProgressProperty = BindableProperty.Create(
            propertyName: "InnerProgress",
            returnType: typeof(int),
            declaringType: typeof(LingvoProgressView),
            defaultValue: 0);

        public static readonly BindableProperty OuterProgressProperty = BindableProperty.Create(
            propertyName: "OuterProgress",
            returnType: typeof(int),
            declaringType: typeof(LingvoProgressView),
            defaultValue: 0);

        public static readonly BindableProperty MaxProgressProperty = BindableProperty.Create(
            propertyName: "MaxProgress",
            returnType: typeof(int),
            declaringType: typeof(LingvoProgressView),
            defaultValue: 100);

        public static readonly BindableProperty InnerProgressColorProperty = BindableProperty.Create(
            propertyName: "InnerProgressColor",
            returnType: typeof(Color),
            declaringType: typeof(LingvoProgressView),
            defaultValue: Color.Aqua);

        public static readonly BindableProperty OuterProgressColorProperty = BindableProperty.Create(
            propertyName: "OuterProgressColor",
            returnType: typeof(Color),
            declaringType: typeof(LingvoProgressView),
            defaultValue: Color.Teal);

        public static readonly BindableProperty SizeProperty = BindableProperty.Create(
           propertyName: "Size",
           returnType: typeof(int),
           declaringType: typeof(LingvoProgressView),
           defaultValue: 0);

        public static readonly BindableProperty LabelTypeProperty = BindableProperty.Create(
          propertyName: "LabelType",
          returnType: typeof(LabelTypeValue),
          declaringType: typeof(LingvoProgressView),
          defaultValue: LabelTypeValue.Percentual);

        public static readonly BindableProperty InnerProgressEnabledProperty = BindableProperty.Create(
           propertyName: "InnerProgressEnabled",
           returnType: typeof(bool),
           declaringType: typeof(LingvoProgressView),
           defaultValue: true);

        public bool InnerProgressEnabled
        {
            get { return (bool)GetValue(InnerProgressEnabledProperty); }
            set { SetValue(InnerProgressEnabledProperty, value); }
        }

        public int InnerProgress
        {
            get { return (int)GetValue(InnerProgressProperty); }
            set { SetValue(InnerProgressProperty, value); }
        }

        public int OuterProgress
        {
            get { return (int)GetValue(OuterProgressProperty); }
            set { SetValue(OuterProgressProperty, value); }
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
