
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Platform.Android;

namespace Lingvo.MobileApp.Droid
{
    [Activity (Label = "Lingvo",Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabLayout;

            base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new MobileApp.App ());
		}
	}
}

