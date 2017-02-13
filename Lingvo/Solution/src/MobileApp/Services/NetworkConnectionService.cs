using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Services
{
    public class NetworkConnectionService
    {
        private static NetworkConnectionService instance;

        public static NetworkConnectionService Instance => instance ?? (instance = new NetworkConnectionService());

        /// <summary>
        /// Displays a warning if not connected to wifi
        /// </summary>
        public async Task<bool> DisplayWarningIfWifiConnected()
        {
            INetworkService networkService = DependencyService.Get<INetworkService>();
            if (!networkService.IsWifiConnected())
            {
                string title = ((Span)App.Current.Resources["label_warning"]).Text;
                string desc = ((Span)App.Current.Resources["desc_notConnectedToWlan"]).Text;
                string accept = ((Span)App.Current.Resources["label_download"]).Text;
                string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

                if (!await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
