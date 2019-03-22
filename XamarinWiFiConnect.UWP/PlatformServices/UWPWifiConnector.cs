﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.Security.Credentials;
using Xamarin.Forms;
using XamarinWiFiConnect.Common.Services;

[assembly: Dependency(typeof(XamarinWiFiConnect.UWP.PlatformServices.UWPWifiConnector))]
namespace XamarinWiFiConnect.UWP.PlatformServices
{
    internal enum ConnectorStatuses : byte
    {
        NoStatus = 0,
        IsInitializing = 1,
        Initialized = 2,
        IsConnecting = 3,
        Connected = 4
    }

    public class UWPWifiConnector : IWifiConnector
    {
        WiFiAdapter _wifiAdapter;
        static object initializeLocker = new object();
        static ConnectorStatuses status;
        int tryCount = 0;

        private async Task InitializeFirstAdapter()
        {
            lock (initializeLocker)
            {
                status = ConnectorStatuses.IsInitializing;
            }
            var access = await WiFiAdapter.RequestAccessAsync();
            if (access != WiFiAccessStatus.Allowed)
            {
                lock (initializeLocker)
                {
                    status = ConnectorStatuses.NoStatus;
                }
                throw new Exception("WiFiAccessStatus not allowed");
            }
            else
            {
                var wifiAdapterResults =
                  await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
                if (wifiAdapterResults.Count >= 1)
                {
                    this._wifiAdapter =
                      await WiFiAdapter.FromIdAsync(wifiAdapterResults[0].Id);
                    lock (initializeLocker)
                    {
                        status = ConnectorStatuses.Initialized;
                    }
                }
                else
                {
                    lock (initializeLocker)
                    {
                        status = ConnectorStatuses.NoStatus;
                    }
                    throw new Exception("WiFi Adapter not found.");
                }
            }
        }

        public UWPWifiConnector()
        {
            InitializeFirstAdapter();
        }

        public void ConnectToWifi(string ssid, string password)
        {
            lock (initializeLocker)
            {
                switch (status)
                {
                    case ConnectorStatuses.NoStatus:
                        throw new Exception("Wifi addapter has failed to initialize. Please check the application access and restart the application..");
                    case ConnectorStatuses.IsInitializing:
                        if(tryCount<2)
                        {
                            tryCount++;
                            Task.Run(async () => {
                                await Task.Delay(1000);//wait for 1 second and try again
                                ConnectToWifi(ssid, password);
                            });
                            return;
                        }
                        tryCount = 0;
                        throw new Exception("Wifi addapter is still initializing. Please check the application access and try again..");
                    case ConnectorStatuses.Initialized:
                        tryCount = 0;
                        status = ConnectorStatuses.IsConnecting;
                        break;
                    case ConnectorStatuses.IsConnecting:
                        return;
                    case ConnectorStatuses.Connected:
                        return;
                    default:
                        break;
                }
            }

            var t = new Task(async () => { 
                await _wifiAdapter.ScanAsync();

                if (_wifiAdapter.NetworkReport?.AvailableNetworks == null)
                {
                    lock (initializeLocker)
                    {
                        status = ConnectorStatuses.Initialized;
                    }
                    return;
                }

                var availableNetwork = _wifiAdapter.NetworkReport.AvailableNetworks.FirstOrDefault(x=>x.Ssid.Equals(ssid));

                if (availableNetwork == null)
                {
                    lock (initializeLocker)
                    {
                        status = ConnectorStatuses.Initialized;
                    }
                    throw new Exception("The specified wifi network ");
                }

                WiFiReconnectionKind reconnectionKind = WiFiReconnectionKind.Manual;
                if (availableNetwork.SecuritySettings.NetworkAuthenticationType == NetworkAuthenticationType.Open80211)
                {
                    await _wifiAdapter.ConnectAsync(availableNetwork, reconnectionKind);
                }
                else
                {
                    var credential = new PasswordCredential();
                    if (!string.IsNullOrEmpty(password))
                    {
                        credential.Password = password;
                    }
                    try
                    {
                        await _wifiAdapter.ConnectAsync(availableNetwork, reconnectionKind, credential);
                        lock (initializeLocker)
                        {
                            status = ConnectorStatuses.Connected;
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (initializeLocker)
                        {
                            status = ConnectorStatuses.Initialized;
                        }
                        throw new Exception("Activating the connection to the configured wifi network failed", ex);
                    }
                }
            });
            t.Start();
        }
    }
}
