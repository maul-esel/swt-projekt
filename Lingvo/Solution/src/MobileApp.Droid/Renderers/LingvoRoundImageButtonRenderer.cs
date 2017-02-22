using Android.Content.Res;
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
    /// <summary>
    /// The Android custom renderer for the <see cref="LingvoRoundImageButton"/>.
    /// </summary>
    class LingvoRoundImageButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<Lingvo.MobileApp.Forms.LingvoRoundImageButton, Android.Widget.Button>
    {
        Button button;
        int customPaddingLeft = 0, customPaddingRight = 0;
        Xamarin.Forms.TextAlignment alignment;
        bool isAligned = false;

        /// <summary>
        /// Called on each instatiation of the <c>Xamarin.Forms.View</c>.
        /// Registers or unregisters event listeners and <see cref="updateView(object, EventArgs)"/>.
        /// </summary>
        /// <param name="e">The <c>ElementChangedEventArgs</c> containing the <see cref="LingvoRoundImageButton"/>.</param>
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

        /// <summary>
        /// Changes the appearance of the button according to properties of the <see cref="LingvoRoundImageButton"/>.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The <c>EventArgs</c>.</param>
        public void updateView(object sender, PropertyChangedEventArgs e)
        {
            if (Control == null)
            {
                return;
            }

            LingvoRoundImageButton element = (LingvoRoundImageButton)sender;

            button.ContentDescription = element.AutomationId;

            Color color = element.IsEnabled ? element.Color.ToAndroid() : Color.LightGray;

            string identifier = element.Image?.Length > 0 ? element.Image.Substring(0, element.Image.LastIndexOf('.')) : null;
            int resourceId = 0;

            //If we got an image, get its android resource id
            if (identifier != null)
            {
                resourceId = (int)typeof(Resource.Drawable).GetField(identifier).GetValue(null);
            }

            if (resourceId != 0)
            {
                //Here, we got an image resource

                if (element.Border)
                {
                    //If we additionally have to draw a border, create a LayerDrawable
                    //and add the circle background and the image; additionally, take 
                    //care of the filled property!

                    int offset = element.Filled ? 1 : 0;
                    Drawable[] layers = new Drawable[2 + offset];
                    layers[0 + offset] = Resources.GetDrawable(Resource.Drawable.round_button_border);
                    layers[0 + offset].SetColorFilter(color, PorterDuff.Mode.SrcAtop);
                    layers[1 + offset] = Resources.GetDrawable(resourceId).Mutate();
                    if (element.Filled)
                    {
                        //If we should fill the button on top, fill the background and
                        //draw the image in white

                        layers[0] = Resources.GetDrawable(Resource.Drawable.round_button_filled);
                        layers[0].SetColorFilter(color, PorterDuff.Mode.SrcAtop);
                        layers[1 + offset].SetColorFilter(Color.White, PorterDuff.Mode.SrcIn);
                    }
                    else
                    {
                        //Otherwise, draw the image in the given color

                        layers[1 + offset].SetColorFilter(color, PorterDuff.Mode.SrcIn);
                    }
                    button.Background = new LayerDrawable(layers);
                }
                else
                {
                    //Okay, no border - means one background image less.

                    Drawable image = Resources.GetDrawable(resourceId).Mutate();

                    if (element.Filled)
                    {
                        //If we should fill the button, create a LayerDrawable with filled background
                        //and draw the image in white

                        Drawable[] layers = new Drawable[2];
                        layers[0] = Resources.GetDrawable(Resource.Drawable.round_button_filled);
                        layers[0].SetColorFilter(color, PorterDuff.Mode.SrcAtop);
                        layers[1] = image;
                        layers[1].SetColorFilter(Color.White, PorterDuff.Mode.SrcIn);
                        button.Background = new LayerDrawable(layers);
                    }
                    else
                    {
                        //No border, no filling - simply draw the image in the given color

                        image.SetColorFilter(color, PorterDuff.Mode.SrcIn);
                        button.Background = image;
                    }
                }
            }

            if (element.Text?.Length > 0)
            {
                //We got a text, so set all relevant text properties and the text itself

                int textHeight = (int)(element.HeightRequest * 0.25);
                button.Text = element.Text;

                button.TextSize = textHeight;

                button.SetTextColor(color);

                if (element.TextOrientation != alignment || !isAligned)
                {
                    //Uh oh - a custom alignment. So compute the padding, change the gravity
                    //and save the alignment for a potential undo operation (means the next re-alignment)

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