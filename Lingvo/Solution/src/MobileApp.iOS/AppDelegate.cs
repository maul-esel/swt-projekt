using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.IO;
using ObjCRuntime;

namespace Lingvo.MobileApp.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            UIApplication.SharedApplication.IdleTimerDisabled = true;
			UIApplication.SharedApplication.SetStatusBarHidden(false, UIStatusBarAnimation.None);

            global::Xamarin.Forms.Forms.Init ();

			ObjCRuntime.Class.ThrowOnInitFailure = false;

			//configure app colors
			UINavigationBar.Appearance.BarTintColor = new UIColor(74.0f / 255.0f, 144.0f / 255.0f, 226.0f / 255.0f, 1.0f);
			UINavigationBar.Appearance.TintColor = UIColor.White;
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				TextColor = UIColor.White,
				Font = UIFont.FromName("Georgia-Bold", 20)
			});
			UINavigationBar.Appearance.BarStyle = UIBarStyle.Black;
		

			LoadApplication (new MobileApp.App ());





			/*
			UINavigationBar.appearance().barStyle = .black
        
        UINavigationBar.appearance().barTintColor = .appGreen
        UINavigationBar.appearance().titleTextAttributes = [
            NSForegroundColorAttributeName: UIColor.white,
            NSFontAttributeName: UIFont(name: "Georgia-Bold", size: 20)!
        ]
        UINavigationBar.appearance().tintColor = .white
        
        
        UIBarButtonItem.appearance().setTitleTextAttributes([
            NSForegroundColorAttributeName: UIColor.white,
            NSFontAttributeName: UIFont(name: "OpenSans-Semibold", size: 18)!
            
        ], for: .normal)

			*/

			return base.FinishedLaunching (app, options);
		}
	}
}
