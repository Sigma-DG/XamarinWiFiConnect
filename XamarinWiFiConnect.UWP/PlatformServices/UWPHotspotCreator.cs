using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.NetworkOperators;
using Xamarin.Forms;
using XamarinWiFiConnect.Common.Services;
using XamarinWiFiConnect.UWP.PlatformServices;

[assembly: Dependency(typeof(UWPHotspotCreator))]
namespace XamarinWiFiConnect.UWP.PlatformServices
{
    public class UWPHotspotCreator : IHotspotCreator
    {
        public bool IsHotspotEnabled { get; private set; }

        public string ConfiguredSSID { get; private set; }

        public string ConfiguredPassword { get; private set; }

        public event StringHandler OnLog;
        public event ExceptionHandler OnError;

        public void CreateHotspot(string ssid, string preSharedKey)
        {
            //TODO: Implement
            //As an Administrator
            //$"netsh wlan set hostednetwork mode=allow ssid={ssid} key={preSharedKey}"
            //"netsh wlan start hostednetwork"

            // Find the Ethernet profile (IANA Type 6)
            var connectionProfiles = NetworkInformation.GetConnectionProfiles();
            var ethernetConnectionProfile = connectionProfiles.FirstOrDefault(x => x.NetworkAdapter.IanaInterfaceType == 6);

            // Find an 802.11 wireless network interface (IANA Type 71)
            var wirelessConnectionProfile = connectionProfiles.FirstOrDefault(x => x.NetworkAdapter.IanaInterfaceType == 71);
            var targetNetworkAdapter = wirelessConnectionProfile.NetworkAdapter;

            if (ethernetConnectionProfile != null && targetNetworkAdapter != null)
            {
                var tetheringManager = NetworkOperatorTetheringManager.CreateFromConnectionProfile(ethernetConnectionProfile, targetNetworkAdapter);

                Task.Run(async () => { 
                    var result = await tetheringManager.StartTetheringAsync();
                    if (result.Status == TetheringOperationStatus.Success)
                    {
                        IsHotspotEnabled = true;
                        Device.BeginInvokeOnMainThread(() => {
                            OnLog?.Invoke("Hotspot is created successfully");
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            OnError?.Invoke(new Exception($"Error creating hotspot network: {result.AdditionalErrorMessage}"));
                        });
                    }
                });
            }
        }

        public void StopHotspot()
        {
            //TODO Implement
            //As Admin
            //"netsh wlan stop hostednetwork"

            // Find the Ethernet profile (IANA Type 6)
            var connectionProfiles = NetworkInformation.GetConnectionProfiles();
            var ethernetConnectionProfile = connectionProfiles.FirstOrDefault(x => x.NetworkAdapter.IanaInterfaceType == 6);

            // Find an 802.11 wireless network interface (IANA Type 71)
            var wirelessConnectionProfile = connectionProfiles.FirstOrDefault(x => x.NetworkAdapter.IanaInterfaceType == 71);
            var targetNetworkAdapter = wirelessConnectionProfile.NetworkAdapter;

            if (ethernetConnectionProfile != null && targetNetworkAdapter != null)
            {
                var tetheringManager = NetworkOperatorTetheringManager.CreateFromConnectionProfile(ethernetConnectionProfile, targetNetworkAdapter);

                Task.Run(async () => {
                    var result = await tetheringManager.StopTetheringAsync();
                    if (result.Status == TetheringOperationStatus.Success)
                    {
                        IsHotspotEnabled = false;
                        Device.BeginInvokeOnMainThread(() => {
                            OnLog?.Invoke("Hotspot is turned off successfully");
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            OnError?.Invoke(new Exception($"Error stopping hotspot network: {result.AdditionalErrorMessage}"));
                        });
                    }
                });
            }
        }
    }
}
