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

            global::Xamarin.Forms.Forms.Init ();
			LoadApplication (new MobileApp.App ());

			////write a samle file to the documents dir
			//var documentsDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			//var filePath = Path.Combine(documentsDirPath, "page1.mp3");
			var bundleFileUrl = Path.Combine(NSBundle.MainBundle.BundlePath, "Content/page1.mp3");
			ObjCRuntime.Class.ThrowOnInitFailure = false;

			var bundleFileBytes = File.ReadAllBytes(bundleFileUrl);


			////Now write to the documents directory
			//File.WriteAllBytes(filePath, bundleFileBytes);
			//Console.WriteLine("filePath = " + filePath);

			return base.FinishedLaunching (app, options);
		}
	}
}
