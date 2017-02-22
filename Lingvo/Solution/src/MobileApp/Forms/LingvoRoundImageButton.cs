using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Forms
{
    /// <summary>
    /// A custom rounded image button, which can have a border and text.
    /// </summary>
    public class LingvoRoundImageButton : View
    {
        public static readonly string PlayImage = "ic_play_arrow.png";
        public static readonly string PauseImage = "ic_pause.png";
        public static readonly string RewindImage = "ic_rewind.png";
        public static readonly string ForwardImage = "ic_forward.png";
        public static readonly string StopImage = "ic_stop.png";
        public static readonly string RecordImage = "ic_fiber_manual_record.png";

        public static readonly BindableProperty ImageProperty = BindableProperty.Create(
          propertyName: "Image",
          returnType: typeof(string),
          declaringType: typeof(LingvoRoundImageButton),
          defaultValue: "");

        /// <summary>
        /// The button image.
        /// </summary>
        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
          propertyName: "Color",
          returnType: typeof(Color),
          declaringType: typeof(LingvoRoundImageButton),
          defaultValue: Color.Black);

        /// <summary>
        /// The button overlay color.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly BindableProperty BorderProperty = BindableProperty.Create(
          propertyName: "Border",
          returnType: typeof(bool),
          declaringType: typeof(LingvoRoundImageButton),
          defaultValue: false);

        /// <summary>
        /// Determines if the button has a circle border.
        /// </summary>
        public bool Border
        {
            get { return (bool)GetValue(BorderProperty); }
            set { SetValue(BorderProperty, value); }
        }

        public static readonly BindableProperty FilledProperty = BindableProperty.Create(
          propertyName: "Filled",
          returnType: typeof(bool),
          declaringType: typeof(LingvoRoundImageButton),
          defaultValue: false);

        /// <summary>
        /// Determines if the button is filled.
        /// </summary>
        public bool Filled
        {
            get { return (bool)GetValue(FilledProperty); }
            set { SetValue(FilledProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
          propertyName: "Text",
          returnType: typeof(string),
          declaringType: typeof(LingvoRoundImageButton),
          defaultValue: "");

        /// <summary>
        /// The text of the button.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextOrientationProperty = BindableProperty.Create(
          propertyName: "TextOrientation",
          returnType: typeof(TextAlignment),
          declaringType: typeof(LingvoRoundImageButton),
          defaultValue: TextAlignment.Center);

        /// <summary>
        /// The orientation of the text.
        /// </summary>
        public TextAlignment TextOrientation
        {
            get { return (TextAlignment)GetValue(TextOrientationProperty); }
            set { SetValue(TextOrientationProperty, value); }
        }

        public delegate void OnClickedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// The event fired when the button is clicked.
        /// </summary>
        public event OnClickedEventHandler OnClicked;

        public void OnButtonClicked(object sender, EventArgs e)
        {
            OnClicked?.Invoke(this, e);
        }
    }
}
