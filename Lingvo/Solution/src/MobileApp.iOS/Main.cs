using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Lingvo.Common.Entities;
using UIKit;

namespace Lingvo.MobileApp.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
