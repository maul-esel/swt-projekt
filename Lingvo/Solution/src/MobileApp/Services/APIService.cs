using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Lingvo.Common.Entities;
using MobileApp.Proxies;

namespace Lingvo.MobileApp
{
	public class APIService
	{
		private const string URL = "http://localhost:5000/api/app/";

		public APIService()
		{
		}

		private async Task<T> FetchFromURL<T>(string url, Func<Stream, T> convert)
		{
			using (var response = await WebRequest.Create(url).GetResponseAsync())
				using (var dataStream = response.GetResponseStream())
					return convert(dataStream);
		}

		private async Task<string> FetchTextFromURL(string url)
		{
			return await await FetchFromURL(url, stream =>
			{
				using (var reader = new StreamReader(stream))
					return reader.ReadToEndAsync();
			});
		}

		public async Task<Workbook[]> FetchWorkbooks()
		{
			var responseFromServer = await FetchTextFromURL(URL + "workbooks");
			Console.WriteLine(responseFromServer);
			return JsonConvert.DeserializeObject<Workbook[]>(responseFromServer);
		}

		public async Task FetchPages(Workbook workbook)
		{
			var responseFromServer = await FetchTextFromURL($"{URL}workbooks/{workbook.Id}/pages");
			Console.WriteLine(responseFromServer);
			workbook.Pages = JsonConvert.DeserializeObject<List<PageProxy>>(responseFromServer).Cast<IPage>().ToList();
		}

		public async Task<Recording> FetchTeacherTrack(Workbook workbook, IPage page, String localPath)
		{
			return await FetchFromURL($"{URL}workbooks/{workbook.Id}/pages/{page.Number}", dataStream =>
			{
				using (var fileStream = File.Create(localPath))
				{
					//dataStream.Seek(0, SeekOrigin.Begin);
					dataStream.CopyTo(fileStream);
				}
				return new Recording(new TimeSpan(), localPath); // TODO: read length from file
			});
		}
	}
}
