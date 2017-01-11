using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using Android.Widget;
using Lingvo.MobileApp.Droid.Renderers;

[assembly: Xamarin.Forms.ExportRenderer(typeof(Lingvo.MobileApp.Forms.LingvoRoundImageButton), typeof(LingvoRoundImageButtonRenderer))]
namespace Lingvo.MobileApp.Droid.Renderers
{
    class LingvoRoundImageButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<Lingvo.MobileApp.Forms.LingvoRoundImageButton, Android.Widget.ImageButton>
    {
        ImageButton button;
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Lingvo.MobileApp.Forms.LingvoRoundImageButton> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                button = new ImageButton(Context);
                SetNativeControl(button);
            }
            if(e.OldElement != null)
            {
                button.Click -= e.OldElement.OnButtonClicked;
            }
            if(e.NewElement != null)
            {
                button.Click += e.NewElement.OnButtonClicked;

                if (e.NewElement.Size != button.LayoutParameters.Width ||
                    e.NewElement.Size != button.LayoutParameters.Height)
                {
                    button.LayoutParameters = new LayoutParams(e.NewElement.Size, e.NewElement.Size);
                    button.RequestLayout();
                }

                string identifier = e.NewElement.Image.Substring(0, e.NewElement.Image.LastIndexOf('.'));
                int resourceId = (int)typeof(Resource.Drawable).GetField(identifier).GetValue(null);
                button.SetImageResource(resourceId);
            }
        }
    }
}