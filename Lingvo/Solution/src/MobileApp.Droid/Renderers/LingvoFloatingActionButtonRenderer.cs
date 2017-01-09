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
                fab.SetImageResource(Resource.Drawable.ic_content_add_white_24dp);
                fab.Show();
                SetNativeControl(fab);
            }
            if(e.OldElement != null)
            {
                fab.Click -= e.OldElement.OnFabClicked;
            }
            if(e.NewElement != null)
            {
                fab.Click += e.NewElement.OnFabClicked;
            }
        }
    }
}