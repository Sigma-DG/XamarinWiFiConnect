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

        public event StringHandler OnLog;
        public event ExceptionHandler OnError;

        public iOSWifiConnector()
        {
            try
            {
                _wifiManager = new NEHotspotConfigurationManager();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("WifiConnector can not access the device WifiManager", ex));
            }
        }

        public void ConnectToWifi(string ssid, string password)
        {
            NEHotspotConfiguration wifiConfig = new NEHotspotConfiguration(ssid, password, false);
            wifiConfig.JoinOnce = true;

            try
            {
                if(_wifiManager == null)
                {
                    OnError?.Invoke(new Exception("WifiConnector can not access the device WifiManager"));
                    return;
                }
                _wifiManager.RemoveConfiguration(ssid);
                _wifiManager.ApplyConfiguration(wifiConfig, (error) =>
                {
                    if (error != null)//if you get internal error you have to check and manually add your Entitlements.plist instead of using automatic provisioning profile.
                    {
                        if (error.ToString().Contains("internal"))
                        {
                            var message = "Please check and manually add your Entitlements.plist instead of using automatic provisioning profile.";
                            Console.WriteLine(message);
                            OnError?.Invoke(new Exception(message));
                        }
                        else
                        {
                            var message = $"Error while connecting to WiFi network {ssid}: {error}";
                            OnError?.Invoke(new Exception(message));
                            Console.WriteLine(message);
                        }
                    }
                    else OnLog?.Invoke("WiFi network has been connected");
                });
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("WifiConnector can not add the new wifi network configuration", ex));
            }
        }
    }
}