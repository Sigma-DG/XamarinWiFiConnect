using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinWiFiConnect.Common.Services
{
    public interface IWifiConnector
    {
        /// <summary>
        /// Connects the device to the specified WiFi network. If it is required it will add the network configurations first and then sends connection request to the users.
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="password"></param>
        void ConnectToWifi(string ssid, string password);//TODO: It has to be an async call ("async Task" instead of "void")
    }
}
