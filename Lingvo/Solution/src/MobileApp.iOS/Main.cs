using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace Lingvo.MobileApp.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			APIService service = new APIService();
			var workbook = service.FetchWorkbooks().ElementAt(0);
			service.FetchPages(workbook);
			service.FetchTeacherTrack(workbook, workbook.Pages.ElementAt(0), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/file.mp3");

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");

		}
	}
}
