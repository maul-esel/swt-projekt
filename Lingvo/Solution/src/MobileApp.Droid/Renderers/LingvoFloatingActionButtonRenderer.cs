using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using Lingvo.MobileApp.Droid.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Lingvo.MobileApp.Forms.LingvoFloatingActionButton), typeof(LingvoFloatingActionButtonRenderer))]
namespace Lingvo.MobileApp.Droid.Renderers
{
    class LingvoFloatingActionButtonRenderer : Xamarin.Forms.Platform.Android.ViewRenderer<Lingvo.MobileApp.Forms.LingvoFloatingActionButton, FloatingActionButton>
    {
        FloatingActionButton fab;
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Lingvo.MobileApp.Forms.LingvoFloatingActionButton> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                fab = new FloatingActionButton(Context);
                Drawable image = Resources.GetDrawable(Resource.Drawable.ic_action_add, Resources.NewTheme());
                image.SetColorFilter(Android.Graphics.Color.White, PorterDuff.Mode.SrcIn);
                fab.SetImageDrawable(image);
                fab.Show();
                SetNativeControl(fab);
            }
            if (e.OldElement != null)
            {
                fab.Click -= e.OldElement.OnFabClicked;
            }
            if (e.NewElement != null)
            {
                fab.Click += e.NewElement.OnFabClicked;
            }
        }
    }
}