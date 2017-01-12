using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Lingvo.MobileApp.Droid.Renderers;
using Lingvo.MobileApp.Forms;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(Lingvo.MobileApp.Forms.LingvoRoundImageButton), typeof(LingvoRoundImageButtonRenderer))]
namespace Lingvo.MobileApp.Droid.Renderers
{
    class LingvoRoundImageButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<Lingvo.MobileApp.Forms.LingvoRoundImageButton, Android.Widget.Button>
    {
        Button button;
        int customPaddingLeft = 0, customPaddingRight = 0;
        Xamarin.Forms.TextAlignment alignment;
        bool isAligned = false;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Lingvo.MobileApp.Forms.LingvoRoundImageButton> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                button = new Button(Context);
                SetNativeControl(button);
            }
            if (e.OldElement != null && e.NewElement == null)
            {
                e.NewElement.PropertyChanged -= updateView;
                button.Click -= e.OldElement.OnButtonClicked;
            }
            else if (e.NewElement != null)
            {
                button.Click += e.NewElement.OnButtonClicked;
                e.NewElement.PropertyChanged += updateView;
            }
        }

        public void updateView(object sender, PropertyChangedEventArgs e)
        {
            if (Control == null)
            {
                return;
            }

            LingvoRoundImageButton element = (LingvoRoundImageButton)sender;

            string identifier = element.Image?.Substring(0, element.Image.LastIndexOf('.'));
            int resourceId = (int)typeof(Resource.Drawable).GetField(identifier).GetValue(null);

            Color color = element.IsEnabled ? element.Color.ToAndroid() : Color.LightGray;

            if (element.Border)
            {
                Drawable[] layers = new Drawable[2];
                layers[0] = Resources.GetDrawable(Resource.Drawable.round_button_border);
                layers[0].SetColorFilter(color, PorterDuff.Mode.SrcAtop);
                layers[1] = Resources.GetDrawable(resourceId);
                layers[1].SetColorFilter(color, PorterDuff.Mode.SrcIn);
                LayerDrawable layerDrawable = new LayerDrawable(layers);

                button.Background = layerDrawable;
            }
            else
            {
                Drawable image = Resources.GetDrawable(resourceId);
                image.SetColorFilter(color, PorterDuff.Mode.SrcIn);
                button.Background = image;
            }

            if (element.Text?.Length > 0)
            {
                button.SetTextColor(color);
            }

            if (element.Text?.Length > 0)
            {
                int textHeight = (int)(element.HeightRequest * 0.25);
                button.Text = element.Text;

                button.TextSize = textHeight;

                if (element.TextOrientation != alignment || !isAligned)
                {
                    if (isAligned)
                    {
                        button.SetPadding(button.PaddingLeft - customPaddingLeft, button.PaddingTop, button.PaddingRight - customPaddingRight, button.PaddingEnd - customPaddingRight - customPaddingLeft);
                    }

                    switch (element.TextOrientation)
                    {
                        case Xamarin.Forms.TextAlignment.Start:
                            button.Gravity = Android.Views.GravityFlags.Left | Android.Views.GravityFlags.Bottom;
                            customPaddingLeft = textHeight;
                            button.SetPadding(button.PaddingLeft + textHeight, button.PaddingTop, button.PaddingRight, button.PaddingEnd + textHeight);
                            button.TextAlignment = Android.Views.TextAlignment.Gravity;
                            break;
                        case Xamarin.Forms.TextAlignment.End:
                            button.Gravity = Android.Views.GravityFlags.Right | Android.Views.GravityFlags.Bottom;
                            customPaddingRight = textHeight;
                            button.SetPadding(button.PaddingLeft, button.PaddingTop, button.PaddingRight + textHeight, button.PaddingEnd + textHeight);
                            button.TextAlignment = Android.Views.TextAlignment.Gravity;
                            break;
                        case Xamarin.Forms.TextAlignment.Center:
                            button.Gravity = Android.Views.GravityFlags.Center;
                            button.TextAlignment = Android.Views.TextAlignment.Center;
                            break;
                    }

                    isAligned = true;
                    alignment = element.TextOrientation;
                }
            }
        }
    }
}