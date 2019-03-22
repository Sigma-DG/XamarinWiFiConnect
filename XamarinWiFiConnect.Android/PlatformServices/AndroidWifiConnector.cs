using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using XamarinWiFiConnect;
using XamarinWiFiConnect.Common.Services;
using XamarinWiFiConnect.Droid.PlatformServices;

[assembly: Dependency(typeof(AndroidWifiConnector))]
namespace XamarinWiFiConnect.Droid.PlatformServices
{
    public class AndroidWifiConnector : Java.Lang.Object, IWifiConnector
    {
        WifiManager _wifiManager;

        public AndroidWifiConnector()
        {
            try
            {
                _wifiManager = (WifiManager)Android.App.Application.Context
                                .GetSystemService(Context.WifiService);
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

            var wifiConfig = new WifiConfiguration
            {
                Ssid = formattedSsid,
                PreSharedKey = formattedPassword
            };

            try
            {
                var addNetwork = _wifiManager.AddNetwork(wifiConfig);
            }
            catch (Exception ex)
            {
                throw new Exception("WifiConnector can not add the new wifi network configuration", ex);
            }

            WifiConfiguration network;
            try
            {
                network = _wifiManager.ConfiguredNetworks
                         .FirstOrDefault(n => n.Ssid == formattedSsid);

                if (network == null)
                {
                    throw new Exception("WifiConnector can not connect to the specified wifi network");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("WifiConnector can not get the list of configured wifi networks", ex);
            }
            
            try
            {
                _wifiManager.Disconnect();
                var enableNetwork = _wifiManager.EnableNetwork(network.NetworkId, true);
            }
            catch (Exception ex)
            {
                throw new Exception("Activating the connection to the configured wifi network failed", ex);
            }
        }
    }
}