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
	/// <summary>
	/// Service for communication with the server
	/// </summary>
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

		/// <summary>
		/// Fetchs the workbooks.
		/// </summary>
		/// <returns>The workbooks.</returns>
		public async Task<Workbook[]> FetchWorkbooks()
		{
			var responseFromServer = await FetchTextFromURL(URL + "workbooks");
			return JsonConvert.DeserializeObject<Workbook[]>(responseFromServer);
		}

		/// <summary>
		/// Fetchs a workbook.
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookID">Workbook identifier.</param>
		public async Task<Workbook> FetchWorkbook(int workbookID)
		{
			var responseFromServer = await FetchTextFromURL(URL + "workbooks/" + workbookID);
			var workbook = JsonConvert.DeserializeObject<Workbook>(responseFromServer);
			await FetchPages(workbook);

			foreach (var page in workbook.Pages)
			{
				await ((PageProxy)page).Resolve();
			}

			return workbook;

		}

		/// <summary>
		/// Fetchs a page.
		/// </summary>
		/// <returns>The page.</returns>
		/// <param name="proxy">Proxy.</param>
		public async Task<Page> FetchPage(PageProxy proxy)
		{
			var recording = await FetchTeacherTrack(proxy.Workbook, proxy, 
			                                  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + 
			                                  "/lingvo/" + proxy.Workbook.Id + "/" + proxy.Number + ".mp3");
			Page page = new Page();
			page.Description = proxy.Description;
			page.Number = proxy.Number;
			page.Workbook = proxy.Workbook;
			page.workbookId = page.Workbook.Id;
			page.TeacherTrack = recording;
			page.teacherTrackId = page.TeacherTrack.Id;

			return page;

		}

		public async Task FetchPages(Workbook workbook)
		{
			var responseFromServer = await FetchTextFromURL($"{URL}workbooks/{workbook.Id}/pages");
			workbook.Pages = JsonConvert.DeserializeObject<List<PageProxy>>(responseFromServer).Cast<IPage>().ToList();
		}

		/// <summary>
		/// Fetchs a teacher track.
		/// </summary>
		/// <returns>The teacher track.</returns>
		/// <param name="workbook">Workbook.</param>
		/// <param name="page">Page.</param>
		/// <param name="localPath">Local path.</param>
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
