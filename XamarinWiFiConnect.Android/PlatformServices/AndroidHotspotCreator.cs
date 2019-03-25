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
        MyHotspotCallback _myHotspotCallback = null;

        public event StringHandler OnLog;

        public event ExceptionHandler OnError;

        public event HotspotCreationHandler OnHotspotCreated;

        public string ConfiguredSSID { get; private set; }

        public string ConfiguredPassword { get; private set; }

        public AndroidHotspotCreator()
        {
            try
            {
                _wifimanager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
                if (_wifimanager == null) throw new Exception("Failed to retrieve WifiManager");

                _isWifiApEnabledMethod = _wifimanager.Class.GetDeclaredMethod("isWifiApEnabled");
                if (_isWifiApEnabledMethod == null) throw new Exception("Failed to retrieve isWifiApEnabled function");
                _isWifiApEnabledMethod.Accessible = true;

                _myHotspotCallback = new MyHotspotCallback
                {
                    AnyEvent = (m) =>
                    {
                        OnLog?.Invoke(m);
                    },
                    Error = (ex) =>
                    {
                        ConfiguredSSID = ConfiguredPassword = string.Empty;
                        OnError?.Invoke(ex);
                    },
                    Created = (ssid, pass) => {
                        ConfiguredSSID = ssid;
                        ConfiguredPassword = pass;
                        OnHotspotCreated?.Invoke(ssid, pass);
                    }
                };

                OnLog?.Invoke("AndroidHotspotCreator is successfully initiated");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("HotspotCreator can not access the device WifiManager", ex));
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
        
        public void CreateAutoHotspot()
        {
            try
            {
                if (!_wifimanager.IsWifiEnabled)
                {
                    OnError?.Invoke(new Exception("Wireless network adapter is disabled"));
                    return;
                }

                // if WiFi is on, turn it off
                if (IsHotspotEnabled)
                {
                    _wifimanager.SetWifiEnabled(false);
                }
                Device.BeginInvokeOnMainThread(() => {

                    //var p = new Android.Net.Wifi.Hotspot2.PasspointConfiguration();
                    //p.
                    //_wifimanager.AddOrUpdatePasspointConfiguration(p);

                    _wifimanager.StartLocalOnlyHotspot(_myHotspotCallback,
                    new Handler((m) =>
                    {
                        var d = m;
                    }));
                });
                //Method setWifiApEnabledMethod = _wifimanager.Class.GetMethod("setWifiApEnabled", wificonfiguration.Class, Java.Lang.Boolean.Type);
                //setWifiApEnabledMethod.Invoke(_wifimanager, wificonfiguration, !IsHotspotEnabled);
                OnLog?.Invoke("LocalOnlyHostSpot has been requested");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("HotspotCreator can not create a hotspot network", ex));
            }
        }

        public void CreateHotspot(string ssid, string password)
        {
            var formattedSsid = $"\"{ssid}\"";
            var formattedPassword = $"\"{password}\"";

            //WifiConfiguration wificonfiguration = null;

            var wificonfiguration = new WifiConfiguration
            {
                Bssid = formattedSsid,
                Ssid = formattedSsid,
                PreSharedKey = formattedPassword,
                //Priority = 1,
                //ProviderFriendlyName = formattedSsid,
                StatusField = WifiStatus.Enabled,
            };
            //wificonfiguration.AllowedProtocols.Set((int)ProtocolType.Wpa);
            //wificonfiguration.AllowedKeyManagement.Set((int)KeyManagementType.WpaPsk);

            try
            {
                if (!_wifimanager.IsWifiEnabled)
                {
                    OnError?.Invoke(new Exception("Wireless network adapter is disabled"));
                    return;
                }

                // if WiFi is on, turn it off
                if (IsHotspotEnabled)
                {
                    _wifimanager.SetWifiEnabled(false);
                }

                //TODO: Implement
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("HotspotCreator can not create a hotspot network", ex));
            }
        }
    }
    
    internal class MyHotspotCallback : WifiManager.LocalOnlyHotspotCallback
    {
        public StringHandler AnyEvent = null;
        public ExceptionHandler Error = null;
        public HotspotCreationHandler Created = null;

        public override void OnFailed([GeneratedEnum] LocalOnlyHotspotCallbackErrorCode reason)
        {
            base.OnFailed(reason);
            var m = reason.ToString();
            Error?.Invoke(new Exception("LocalOnlyHotspot failed to start. ", new Exception(m)));
        }

        public override void OnStarted(WifiManager.LocalOnlyHotspotReservation reservation)
        {
            base.OnStarted(reservation);
            var x = reservation?.WifiConfiguration;
            if (x != null)
            {
                Created?.Invoke(x.Ssid, x.PreSharedKey);
                AnyEvent?.Invoke($"LocalOnlyHotspot started.\nSSID: {x.Ssid}\nPassword: {x.PreSharedKey}");
            }
        }

        public override void OnStopped()
        {
            base.OnStopped();
            AnyEvent?.Invoke("LocalOnlyHotspot stopped.");
        }
    }
}