
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V7.App;

namespace Lingvo.MobileApp.Droid.Activities
{
    /// <summary>
    /// The launch screen app
    /// </summary>
    [Activity(Label = "Deutsch für Dich", Icon = "@drawable/icon", Name = "com.lingvo.android.SplashActivity", Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();

            //Start the main activity; during the setup of MainActivity, this activity is shown
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            Finish();
        }
    }
}