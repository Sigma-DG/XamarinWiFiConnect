using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using XamarinWiFiConnect.Common.Services;
using XamarinWiFiConnect.iOS.PlatformServices;

[assembly: Dependency(typeof(iOSWifiConnector))]
namespace XamarinWiFiConnect.iOS.PlatformServices
{
    public class iOSWifiConnector : IWifiConnector
    {
        public iOSWifiConnector()
        {

        }

        public void ConnectToWifi(string ssid, string password)
        {
            //TODO: Implement
        }
    }
}