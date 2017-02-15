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
		public enum NetworkStatus
		{
			NotReachable,
			ReachableViaCarrierDataNetwork,
			ReachableViaWiFiNetwork
		}

		/// <summary>
		/// The default route reachability
		/// </summary>
		private static NetworkReachability defaultRouteReachability;

		/// <summary>
		/// Checks if network is reachable without requiring connection.
		/// </summary>
		/// <param name="flags">The reachability flags.</param>
		/// <returns>True if reachable, false if connection is required.</returns>
		public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
		{
			return (flags & NetworkReachabilityFlags.Reachable) != 0
				   && (((flags & NetworkReachabilityFlags.IsWWAN) != 0)
					   || (flags & NetworkReachabilityFlags.ConnectionRequired) == 0);
		}



		/// <summary>
		/// The reachability changed event.
		/// </summary>
		/// <remarks>Raised every time there is an interesting reachable event,
		/// we do not even pass the info as to what changed, and
		/// we lump all three status we probe into one.</remarks>
		public static event EventHandler ReachabilityChanged;


		/// <summary>
		/// The internet connection status.
		/// </summary>
		/// <returns>The <see cref="NetworkStatus" />.</returns>
		public static NetworkStatus InternetConnectionStatus()
		{
			NetworkReachabilityFlags flags;

			if ((IsNetworkAvailable(out flags) && ((flags & NetworkReachabilityFlags.IsDirect) != 0)) || flags == 0)
			{
				return NetworkStatus.NotReachable;
			}

			return (flags & NetworkReachabilityFlags.IsWWAN) != 0
					   ? NetworkStatus.ReachableViaCarrierDataNetwork
					   : NetworkStatus.ReachableViaWiFiNetwork;
		}

	

		/// <summary>
		/// Called when [change].
		/// </summary>
		/// <param name="flags">The flags.</param>
		private static void OnChange(NetworkReachabilityFlags flags)
		{
			var h = ReachabilityChanged;
			if (h != null)
			{
				h(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Returns network reachability flags and network availability.
		/// </summary>
		/// <param name="flags">The network reachability flags.</param>
		/// <returns>True if network is available, otherwise false.</returns>
		public static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
		{
			if (defaultRouteReachability == null)
			{
				defaultRouteReachability = new NetworkReachability(new IPAddress(0));
				defaultRouteReachability.SetNotification(OnChange);
				defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
			}

			return defaultRouteReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
		}
		public bool IsWifiConnected()
		{
			return InternetConnectionStatus() == NetworkStatus.ReachableViaWiFiNetwork;
		}
	}
}
