using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinWiFiConnect.Common.Services;

namespace XamarinWiFiConnect.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementations
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return;

            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private bool isHotspotProvider = false;

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set {
                _statusMessage = value;
                NotifyPropertyChanged("StatusMessage");
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set {
                _errorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        public ICommand ConnectCommand { get; private set; }

        public ICommand CreateCommand { get; private set; }

        public MainViewModel()
        {
            ConnectCommand = new Command(() => {
                IWifiConnector wifiConnector = null;

                try
                {
                    wifiConnector = DependencyService.Get<IWifiConnector>();
                    if (wifiConnector == null)
                    {
                        ErrorMessage = "Platform specific WiFi service is not available.";
                        return;
                    }
                    StatusMessage = "WiFi service has been retrieved successfully";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    if (ex.InnerException != null)
                        ErrorMessage = $"{ErrorMessage}\n{ex.InnerException.Message}\n\nStackTrace: {ex.InnerException.StackTrace}";
                    return;
                }

                try
                {
                    string ssID = "MyTestHotSpot";//TODO: put existing WiFi ssID
                    string password = "654987123";//TODO: put existing WiFi password
                    wifiConnector.ConnectToWifi(ssID, password);
                    StatusMessage = $"WiFi is connected to {ssID} successfully";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    if (ex.InnerException != null)
                        ErrorMessage = $"{ErrorMessage}\n{ex.InnerException.Message}\n\nStackTrace: {ex.InnerException.StackTrace}";
                }
            }, () => !isHotspotProvider);

            CreateCommand = new Command(() => {
                IHotspotCreator hotspotCreator = null;

                try
                {
                    hotspotCreator = DependencyService.Get<IHotspotCreator>();
                    if (hotspotCreator == null)
                    {
                        ErrorMessage = "Platform specific HotSpot service is not available.";
                        return;
                    }
                    StatusMessage = "HotSpot service has been retrieved successfully";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    if (ex.InnerException != null)
                        ErrorMessage = $"{ErrorMessage}\n{ex.InnerException.Message}\n\nStackTrace: {ex.InnerException.StackTrace}";
                    return;
                }

                try
                {
                    if (!hotspotCreator.IsHotspotEnabled)
                    {
                        hotspotCreator.CreateHotspot();
                        isHotspotProvider = true;
                        ((Command)ConnectCommand).ChangeCanExecute();

                        StatusMessage = "HotSpot service has been successfully enabled";
                    }
                    else StatusMessage = "HotSpot service is already enabled";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    if (ex.InnerException != null)
                        ErrorMessage = $"{ErrorMessage}\n{ex.InnerException.Message}\n\nStackTrace: {ex.InnerException.StackTrace}";
                    return;
                }


            }, () => Device.RuntimePlatform.Equals(Device.Android));
        }
    }
}
