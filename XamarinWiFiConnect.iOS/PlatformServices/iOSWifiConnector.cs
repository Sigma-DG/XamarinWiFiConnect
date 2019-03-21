using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using NetworkExtension;
using UIKit;
using Xamarin.Forms;
using XamarinWiFiConnect.Common.Services;
using XamarinWiFiConnect.iOS.PlatformServices;

[assembly: Dependency(typeof(iOSWifiConnector))]
namespace XamarinWiFiConnect.iOS.PlatformServices
{
    public class iOSWifiConnector : IWifiConnector
    {
        NEHotspotConfigurationManager _wifiManager;

        public iOSWifiConnector()
        {
            try
            {
                _wifiManager = new NEHotspotConfigurationManager();
            }
            catch (Exception ex)
            {
                throw new Exception("WifiConnector can not access the device WifiManager", ex);
            }
        }

        public void ConnectToWifi(string ssid, string password)
        {
            var formattedSsid = $"\"{ssid}\"";
            var formattedPassword = $"\"{password}\"";

            NEHotspotConfiguration wifiConfig = new NEHotspotConfiguration(formattedSsid, formattedPassword, false);

            try
            {
                _wifiManager.ApplyConfiguration(wifiConfig, (error) =>
                {
                    if (error != null)
                    {
                        var message = $"Error while connecting to WiFi network {formattedSsid}: {error}";
                        Console.WriteLine(message);
                        //throw new Exception(message);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("WifiConnector can not add the new wifi network configuration", ex);
            }
        }
    }
}