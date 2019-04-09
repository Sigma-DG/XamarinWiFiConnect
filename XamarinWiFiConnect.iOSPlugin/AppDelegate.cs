//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Foundation;
//using UIKit;

//namespace XamarinWiFiConnect.iOSPlugin
//{
//    [Register("AppDelegate")]
//    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
//    {
//        private static WifiConnectorInitiator connector = new WifiConnectorInitiator();

//        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
//        {
//            iOSPlugin.WifiConnectorInitiator.Initiate();
//            global::Xamarin.Forms.Forms.Init();
//            LoadApplication(new App());

//            return base.FinishedLaunching(app, options);
//        }
//    }
//}