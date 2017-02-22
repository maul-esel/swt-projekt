using System;
using System.Collections.Generic;
using System.Text;

namespace Lingvo.MobileApp.Services
{
    /// <summary>
    /// An interface to determine if the device is connected to wifi
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Checks if the device is connected to wifi
        /// </summary>
        /// <returns><c>true</c> if connected, otherwise <c>false</c></returns>
        bool IsWifiConnected();
    }
}
