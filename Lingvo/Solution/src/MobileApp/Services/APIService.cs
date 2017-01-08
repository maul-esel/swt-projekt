using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

using Lingvo.Common.Entities;

namespace Lingvo.MobileApp
{
	public class APIService
	{
		private const string URL = "http://localhost:5000/api/app/";

		private List<Workbook> Workbooks { get; set; }

		public APIService()
		{
		}

		public List<Workbook> FetchWorkbooks()
		{
			string url = URL + "workbooks";

			WebRequest request = WebRequest.Create(url);
			WebResponse response = request.GetResponse();
			Console.WriteLine(((HttpWebResponse)response).StatusDescription);
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string responseFromServer = reader.ReadToEnd();
			Console.WriteLine(responseFromServer);

			reader.Close();
			response.Close();

			return null;
		}

	}
}
