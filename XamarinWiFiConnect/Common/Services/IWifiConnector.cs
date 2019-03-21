using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinWiFiConnect.Common.Services
{
    public interface IWifiConnector
    {
        void ConnectToWifi(string ssid, string password);
    }
}
