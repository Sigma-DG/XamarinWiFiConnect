using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinWiFiConnect.Common.Services
{
    public delegate void HotspotCreationHandler(string ssid, string password);

    public interface IHotspotCreator : ILogger
    {
        //event HotspotCreationHandler OnHotspotCreated;

        bool IsHotspotEnabled { get; }

        string ConfiguredSSID { get; }

        string ConfiguredPassword { get; }

        //void CreateAutoHotspot();

        void CreateHotspot(string ssid, string preSharedKey);

        void StopHotspot();
    }
}
