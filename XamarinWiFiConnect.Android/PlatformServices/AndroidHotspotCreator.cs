using System.Linq;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Java.Lang.Reflect;
using Xamarin.Forms;
using XamarinWiFiConnect.Common.Services;
using XamarinWiFiConnect.Droid.PlatformServices;
using Exception = System.Exception;

[assembly: Dependency(typeof(AndroidHotspotCreator))]
namespace XamarinWiFiConnect.Droid.PlatformServices
{
    public class AndroidHotspotCreator : Object, IHotspotCreator
    {
        WifiManager _wifiManager = null;
        Method _isWifiApEnabledMethod = null;
        //MyHotspotCallback _myHotspotCallback = null;

        public event StringHandler OnLog;

        public event ExceptionHandler OnError;

        //public event HotspotCreationHandler OnHotspotCreated;

        public string ConfiguredSSID { get; private set; }

        public string ConfiguredPassword { get; private set; }

        public AndroidHotspotCreator()
        {
            try
            {
                _wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
                if (_wifiManager == null) throw new Exception("Failed to retrieve WifiManager");

                _isWifiApEnabledMethod = _wifiManager.Class.GetDeclaredMethod("isWifiApEnabled");
                if (_isWifiApEnabledMethod == null) throw new Exception("Failed to retrieve isWifiApEnabled function");
                _isWifiApEnabledMethod.Accessible = true;

                //_myHotspotCallback = new MyHotspotCallback
                //{
                //    AnyEvent = (m) =>
                //    {
                //        OnLog?.Invoke(m);
                //    },
                //    Error = (ex) =>
                //    {
                //        ConfiguredSSID = ConfiguredPassword = string.Empty;
                //        OnError?.Invoke(ex);
                //    },
                //    Created = (ssid, pass) => {
                //        ConfiguredSSID = ssid;
                //        ConfiguredPassword = pass;
                //        OnHotspotCreated?.Invoke(ssid, pass);
                //    }
                //};

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
        
        //public void CreateAutoHotspot()
        //{
        //    try
        //    {
        //        //if (!_wifiManager.IsWifiEnabled)
        //        //{
        //        //    OnError?.Invoke(new Exception("Wireless network adapter is disabled"));
        //        //    return;
        //        //}

        //        // if WiFi is on, turn it off
        //        if (IsHotspotEnabled)
        //        {
        //            _wifiManager.SetWifiEnabled(false);
        //        }
        //        Device.BeginInvokeOnMainThread(() => {
                    
        //            //_wifimanager.AddOrUpdatePasspointConfiguration(p);

        //            //var callback = new WifiManager.LocalOnlyHotspotCallback();
                    
        //            _wifiManager.StartLocalOnlyHotspot(null,
        //            new Handler((m) =>
        //            {
        //                var d = m;
        //            }));
        //        });
        //        //Method setWifiApEnabledMethod = _wifimanager.Class.GetMethod("setWifiApEnabled", wificonfiguration.Class, Java.Lang.Boolean.Type);
        //        //setWifiApEnabledMethod.Invoke(_wifimanager, wificonfiguration, !IsHotspotEnabled);
        //        OnLog?.Invoke("LocalOnlyHostSpot has been requested");
        //    }
        //    catch (Exception ex)
        //    {
        //        OnError?.Invoke(new Exception("HotspotCreator can not create a hotspot network", ex));
        //    }
        //}
        
        public void CreateHotspot(string ssid, string preSharedPassword)
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
                OnLog?.Invoke("HostSpot has been created");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("HotspotCreator can not create a hotspot network", ex));
            }
        }

        public void StopHotspot()
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
                OnLog?.Invoke("HostSpot has been turned off");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new Exception("HotspotCreator can not stop the hotspot network", ex));
            }
        }
    }
    
    //internal class MyHotspotCallback : WifiManager.LocalOnlyHotspotCallback
    //{
    //    public StringHandler AnyEvent = null;
    //    public ExceptionHandler Error = null;
    //    public HotspotCreationHandler Created = null;

    //    public override void OnFailed([GeneratedEnum] LocalOnlyHotspotCallbackErrorCode reason)
    //    {
    //        base.OnFailed(reason);
    //        var m = reason.ToString();
    //        Error?.Invoke(new Exception("LocalOnlyHotspot failed to start. ", new Exception(m)));
    //    }

    //    public override void OnStarted(WifiManager.LocalOnlyHotspotReservation reservation)
    //    {
    //        base.OnStarted(reservation);
    //        var x = reservation?.WifiConfiguration;
    //        if (x != null)
    //        {
    //            Created?.Invoke(x.Ssid, x.PreSharedKey);
    //            AnyEvent?.Invoke($"LocalOnlyHotspot started.\nSSID: {x.Ssid}\nPassword: {x.PreSharedKey}");
    //        }
    //    }

    //    public override void OnStopped()
    //    {
    //        base.OnStopped();
    //        AnyEvent?.Invoke("LocalOnlyHotspot stopped.");
    //    }
    //}
}