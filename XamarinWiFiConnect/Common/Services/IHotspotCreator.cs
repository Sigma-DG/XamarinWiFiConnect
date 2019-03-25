using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinWiFiConnect.Common.Services
{
    public interface IHotspotCreator
    {
        bool IsHotspotEnabled { get; }

        bool CreateHotspot();
    }
}
