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

        public event StringHandler OnLog;
        public event ExceptionHandler OnError;

        public AndroidWifiConnector()
        {
            try
            {
                _wifiManager = (WifiManager)Android.App.Application.Context
                                .GetSystemService(Context.WifiService);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("WifiConnector can not access the device WifiManager", ex));
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
                OnError?.Invoke(new Exception("WifiConnector can not add the new wifi network configuration", ex));
            }

            WifiConfiguration network = null;
            try
            {
                network = _wifiManager.ConfiguredNetworks
                         .FirstOrDefault(n => n.Ssid == formattedSsid);

                if (network == null)
                {
                    OnError?.Invoke(new Exception("WifiConnector can not connect to the specified wifi network"));
                    return;
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("WifiConnector can not get the list of configured wifi networks", ex));
                return;
            }
            
            try
            {
                _wifiManager.Disconnect();
                var enableNetwork = _wifiManager.EnableNetwork(network.NetworkId, true);
                if (enableNetwork && _wifiManager.ConnectionInfo != null && _wifiManager.ConnectionInfo.SSID.Equals(formattedSsid))
                    OnLog?.Invoke("WiFi network has been connected");
                else OnError?.Invoke(new Exception("Te specified wifi network does not exist"));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("Activating the connection to the configured wifi network failed", ex));
            }
        }
    }
}