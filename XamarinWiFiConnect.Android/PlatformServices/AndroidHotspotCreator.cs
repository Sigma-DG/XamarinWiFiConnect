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
using Java.Lang.Reflect;
using Xamarin.Forms;
using XamarinWiFiConnect.Common.Services;
using XamarinWiFiConnect.Droid.PlatformServices;

[assembly: Dependency(typeof(AndroidHotspotCreator))]
namespace XamarinWiFiConnect.Droid.PlatformServices
{
    public class AndroidHotspotCreator : Java.Lang.Object, IHotspotCreator
    {
        WifiManager _wifimanager = null;
        Method _isWifiApEnabledMethod = null;

        public AndroidHotspotCreator()
        {
            try
            {
                _wifimanager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
                if (_wifimanager == null) throw new Exception("Failed to retrieve WifiManager");

                _isWifiApEnabledMethod = _wifimanager.Class.GetDeclaredMethod("isWifiApEnabled");
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
                    return (bool)_isWifiApEnabledMethod.Invoke(_wifimanager);
                }
                catch //(Exception ex)
                {
                    //throw new Exception("HotspotCreator has failed to check Wifi Access Point status.", ex);
                }
                return false;
            }
        }

        public bool CreateHotspot()
        {
            WifiConfiguration wificonfiguration = null;

            wificonfiguration = new WifiConfiguration();
            wificonfiguration.Bssid = "MyTestHotSpot";
            wificonfiguration.PreSharedKey = "654987123";
            //wificonfiguration.
            try
            {
                // if WiFi is on, turn it off
                if (IsHotspotEnabled)
                {
                    _wifimanager.SetWifiEnabled(false);
                }
                Method setWifiApEnabledMethod = _wifimanager.Class.GetMethod("setWifiApEnabled", wificonfiguration.Class, Java.Lang.Boolean.Type);
                setWifiApEnabledMethod.Invoke(_wifimanager, wificonfiguration, !IsHotspotEnabled);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("HotspotCreator can not create a hotspot network", ex);
            }
        }
    }
}