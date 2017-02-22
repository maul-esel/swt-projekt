
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Xamarin.Forms.Platform.Android;

namespace Lingvo.MobileApp.Droid.Activities
{
    /// <summary>
    /// The main Activity class of the Android app
    /// </summary>
    [Activity (Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
            //Set the appcompat resources for toolbar and tablayout
            FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.tabLayout;

            base.OnCreate (bundle);

            //Keep the screen on
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new MobileApp.App ());
		}

        protected override void OnResume()
        {
            base.OnResume();
            //Check if the permisson RecordAudio is granted to this application
            if(ContextCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) != (int)Permission.Granted)
            {
                //If not, request it
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.RecordAudio }, Manifest.Permission.RecordAudio.GetHashCode() & 0xFF);
            }
        }

        /// <summary>
        /// Called after permissions had been requested and the user accepted or declined the system dialog.
        /// </summary>
        /// <param name="requestCode">The code of the request.</param>
        /// <param name="permissions">The requested permissons.</param>
        /// <param name="grantResults">The request result for the permissions.</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if(requestCode == (Manifest.Permission.RecordAudio.GetHashCode() & 0xFF) && grantResults[0] != Permission.Granted)
            {
                //If we didn't get the permission, close the app (we can't guarantee consistency otherwise)
                Finish();
            }
        }
    }
}

