using CoreFoundation;
using Lingvo.MobileApp.iOS.Services;
using Lingvo.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SystemConfiguration;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetworkService))]
namespace Lingvo.MobileApp.iOS.Services
{
    public class NetworkService : INetworkService
    {
        static NetworkReachability defaultRouteReachability;

        public enum NetworkStatus
        {
            NotReachable,
            ReachableViaCarrierDataNetwork,
            ReachableViaWiFiNetwork
        }

        public static event EventHandler ReachabilityChanged;

        static void OnChange(NetworkReachabilityFlags flags)
        {
            ReachabilityChanged?.Invoke(null, EventArgs.Empty);
        }

        public bool IsWifiConnected()
        {
            return InternetConnectionStatus() == NetworkStatus.ReachableViaWiFiNetwork;
        }

        static NetworkStatus InternetConnectionStatus()
        {
            NetworkReachabilityFlags flags;
            bool defaultNetworkAvailable = IsNetworkAvailable(out flags);

            if (defaultNetworkAvailable && ((flags & NetworkReachabilityFlags.IsDirect) != 0))
                return NetworkStatus.NotReachable;

            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                return NetworkStatus.ReachableViaCarrierDataNetwork;

            if (flags == 0)
                return NetworkStatus.NotReachable;

            return NetworkStatus.ReachableViaWiFiNetwork;
        }

        static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            if (defaultRouteReachability == null)
            {
                var ipAddress = new IPAddress(0);
                defaultRouteReachability = new NetworkReachability(ipAddress.MapToIPv6());
                defaultRouteReachability.SetNotification(OnChange);
                defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }
            return defaultRouteReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
        }

        static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
        {
            // Is it reachable with the current network configuration?
            bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

            // Do we need a connection to reach it?
            bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0
                || (flags & NetworkReachabilityFlags.IsWWAN) != 0;

            return isReachable && noConnectionRequired;
        }
    }
}
