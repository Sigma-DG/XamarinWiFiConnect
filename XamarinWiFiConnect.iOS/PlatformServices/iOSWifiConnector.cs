using System;
using NetworkExtension;
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
            NEHotspotConfiguration wifiConfig = new NEHotspotConfiguration(ssid, password, false);
            wifiConfig.JoinOnce = true;

            try
            {
                _wifiManager.RemoveConfiguration(ssid);
                _wifiManager.ApplyConfiguration(wifiConfig, (error) =>
                {
                    if (error != null)//if you get internal error you have to check and manually add your Entitlements.plist instead of using automatic provisioning profile.
                    {
                        if (error.ToString().Contains("internal"))
                        {
                            Console.WriteLine("Please check and manually add your Entitlements.plist instead of using automatic provisioning profile.");
                        }
                        else
                        {
                            var message = $"Error while connecting to WiFi network {ssid}: {error}";
                            Console.WriteLine(message);
                        }
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