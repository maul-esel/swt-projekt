using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Lingvo.MobileApp.Droid.Services;
using Lingvo.MobileApp.Services;
using Android.Net;

[assembly: Dependency(typeof(NetworkService))]
namespace Lingvo.MobileApp.Droid.Services
{
    public class NetworkService : INetworkService
    {
        public bool IsWifiConnected()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Android.App.Application.ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

            return networkInfo.Type == ConnectivityType.Wifi;
        }
    }
}