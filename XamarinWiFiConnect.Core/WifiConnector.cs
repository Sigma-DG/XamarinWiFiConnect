using System;
using XamarinWiFiConnect.Common.Services;

namespace XamarinWiFiConnect.Core
{
    public static class WifiConnector
    {
        static bool isIOSRegistered = false;
        public static IWifiConnector GetService()
        {
            if (!isIOSRegistered)
            {
                Xamarin.Forms.DependencyService.Register<iOSPlugin.PlatformServices.iOSWifiConnector>();
                isIOSRegistered = true;
            }
            return Xamarin.Forms.DependencyService.Get<IWifiConnector>();
        }
    }
}
