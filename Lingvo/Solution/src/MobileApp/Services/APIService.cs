using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using Lingvo.Common.Entities;
using MobileApp.Proxies;

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
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string responseFromServer = reader.ReadToEnd();
			Console.WriteLine(responseFromServer);

			List<Workbook> workbooks = JsonConvert.DeserializeObject<List<Workbook>>(responseFromServer);

			reader.Close();
			response.Close();

			return workbooks;
		}

		public void FetchPages(Workbook workbook)
		{
			string url = URL + "workbooks/" + workbook.Id + "/pages";

			WebRequest request = WebRequest.Create(url);
			WebResponse response = request.GetResponse();
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string responseFromServer = reader.ReadToEnd();
			Console.WriteLine(responseFromServer);

			var pages = JsonConvert.DeserializeObject<List<PageProxy>>(responseFromServer).Cast<IPage>().ToList();
			workbook.Pages = pages;

			reader.Close();
			response.Close();
		}

		public Recording FetchTeacherTrack(Workbook workbook, IPage page, String localPath)
		{
			string url = URL + "workbooks/" + workbook.Id + "/pages/" + page.Number;

			WebRequest request = WebRequest.Create(url);
			WebResponse response = request.GetResponse();
			Stream dataStream = response.GetResponseStream();

			using (var fileStream = File.Create(localPath))
			{
				//dataStream.Seek(0, SeekOrigin.Begin);
				dataStream.CopyTo(fileStream);

			}

			Recording recording = new Recording(new TimeSpan(), localPath);

			response.Close();

			return recording;
		}
	}
}
