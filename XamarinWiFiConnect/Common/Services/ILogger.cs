using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinWiFiConnect.Common.Services
{
    public delegate void StringHandler(string text);
    public delegate void ExceptionHandler(Exception exception);

    public interface ILogger
    {
        event StringHandler OnLog;
        event ExceptionHandler OnError;
    }
}
