
using Android.App;
using Android.Content;
using Android.Support.V7.App;

namespace Lingvo.MobileApp.Droid.Activities
{
    [Activity(Label = "Lingvo", Icon = "@drawable/icon", Name = "com.lingvo.android.SplashActivity", Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            Finish();
        }
    }
}