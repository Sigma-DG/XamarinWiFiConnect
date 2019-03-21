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

        public ICommand StartCommand { get; private set; }

        public MainViewModel()
        {
            StartCommand = new Command(() => {
                IWifiConnector wifiConnector = null;

                try
                {
                    wifiConnector = DependencyService.Get<IWifiConnector>();
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
                    string ssID = "";
                    string password = "";
                    wifiConnector.ConnectToWifi(ssID, password);
                    StatusMessage = $"WiFi is connected to {ssID} successfully";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    if (ex.InnerException != null)
                        ErrorMessage = $"{ErrorMessage}\n{ex.InnerException.Message}\n\nStackTrace: {ex.InnerException.StackTrace}";
                }
            });
        }
    }
}
