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
using Java.Lang;
using Java.Lang.Reflect;
using Java.Net;
using Xamarin.Forms;
using XamarinWiFiConnect.Common.Services;
using XamarinWiFiConnect.Droid.PlatformServices;
using Exception = System.Exception;

[assembly: Dependency(typeof(AndroidHotspotCreator))]
namespace XamarinWiFiConnect.Droid.PlatformServices
{
    public class AndroidHotspotCreator : Java.Lang.Object, IHotspotCreator
    {
        WifiManager _wifiManager = null;
        Method _isWifiApEnabledMethod = null;

        public AndroidHotspotCreator()
        {
            try
            {
                _wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
                if (_wifiManager == null) throw new Exception("Failed to retrieve WifiManager");

                _isWifiApEnabledMethod = _wifiManager.Class.GetDeclaredMethod("isWifiApEnabled");
                if (_isWifiApEnabledMethod == null) throw new Exception("Failed to retrieve isWifiApEnabled function");
                _isWifiApEnabledMethod.Accessible = true;
            }
            catch (Exception ex)
            {
                throw new Exception("HotspotCreator can not access the device WifiManager", ex);
            }
        }
        public bool IsHotspotEnabled
        {
            get
            {
                try
                {
                    return (bool)_isWifiApEnabledMethod.Invoke(_wifiManager);
                }
                catch //(Exception ex)
                {
                    //throw new Exception("HotspotCreator has failed to check Wifi Access Point status.", ex);
                }
                return false;
            }
        }

        private WifiConfiguration GetWifiApConfiguration()
        {
            try
            {
                var method = _wifiManager.Class.GetMethod("getWifiApConfiguration");
                return (WifiConfiguration)method.Invoke(_wifiManager, null);
            }
            catch (Exception ex)
            {
                throw new Exception("HotspotCreator can not retrieve hotspot configurations", ex);
            }
        }
        
        private void SetWifiApConfiguration(WifiConfiguration wifiConfig)
        {
            try
            {
                var method = _wifiManager.Class.GetMethod("setWifiApConfiguration", Class.FromType(typeof(WifiConfiguration)));
                method.Invoke(_wifiManager, wifiConfig);
            }
            catch (Exception ex)
            {
                throw new Exception("HotspotCreator can not configure a hotspot network", ex);
            }
        }

        public bool CreateHotspot(string ssid, string preSharedPassword)
        {
            WifiConfiguration wificonfiguration = GetWifiApConfiguration();

            //wificonfiguration = new WifiConfiguration();
            wificonfiguration.HiddenSSID = false;
            wificonfiguration.Ssid = ssid;
            wificonfiguration.Bssid = ssid;
            wificonfiguration.PreSharedKey = preSharedPassword;

            SetWifiApConfiguration(wificonfiguration);
            //wificonfiguration.
            try
            {
                // if WiFi is on, turn it off
                if (IsHotspotEnabled)
                {
                    _wifiManager.SetWifiEnabled(false);
                }
                //var setWifiApEnabledMethod = _wifimanager.Class.GetMethod("setWifiApEnabled", wificonfiguration.Class, Java.Lang.Boolean.Type);
                var setWifiApEnabledMethod = _wifiManager.Class.GetMethods().FirstOrDefault(m => m.Name == "setWifiApEnabled");
                setWifiApEnabledMethod.Invoke(_wifiManager, wificonfiguration, true);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("HotspotCreator can not create a hotspot network", ex);
            }
        }

        public bool StopHotspot()
        {
            WifiConfiguration wificonfiguration = GetWifiApConfiguration();
            try
            {
                //var setWifiApEnabledMethod = _wifimanager.Class.GetMethod("setWifiApEnabled", wificonfiguration.Class, Java.Lang.Boolean.Type);
                var setWifiApEnabledMethod = _wifiManager.Class.GetMethods().FirstOrDefault(m => m.Name == "setWifiApEnabled");
                setWifiApEnabledMethod.Invoke(_wifiManager, wificonfiguration, false);

                // if WiFi is on, turn it off
                if (IsHotspotEnabled)
                {
                    _wifiManager.SetWifiEnabled(false);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("HotspotCreator can not stop the hotspot network", ex);
            }
        }
    }
}