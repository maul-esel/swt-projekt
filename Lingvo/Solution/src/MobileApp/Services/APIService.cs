using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Lingvo.Common.Entities;
using Lingvo.MobileApp.Proxies;

namespace Lingvo.MobileApp
{
	/// <summary>
	/// Service for communication with the server
	/// </summary>
	public class APIService
	{
		private const string URL = "http://10.0.2.2:5000/api/app/";

		private static APIService instance;

		public static APIService Instance => instance ?? (instance = new APIService());

		private APIService()
		{
		}

		/// <summary>
		/// Fetchs Data from URL.
		/// </summary>
		/// <returns>The from URL.</returns>
		/// <param name="url">URL.</param>
		/// <param name="convert">Function for data conversion</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private async Task<T> FetchFromURLAsync<T>(string url, Func<WebResponse, Task<T>> convert)
		{
			using (var response = await WebRequest.Create(url).GetResponseAsync())
				return await convert(response);
		}

		private Task<string> ReadTextAsync(WebResponse response)
		{
			var stream = response.GetResponseStream();
			using (var reader = new StreamReader(stream))
				return reader.ReadToEndAsync();
		}

		/// <summary>
		/// Fetchs data from the URL as text.
		/// </summary>
		/// <returns>The text from URL.</returns>
		/// <param name="url">URL.</param>
		private Task<string> FetchTextFromURLAsync(string url)
		{
			return FetchFromURLAsync(url, ReadTextAsync);
		}

		/// <summary>
		/// Fetchs the workbooks.
		/// </summary>
		/// <returns>The workbooks.</returns>
		public async Task<Workbook[]> FetchWorkbooks()
		{
			var responseFromServer = await FetchTextFromURLAsync(URL + "workbooks");
			return JsonConvert.DeserializeObject<Workbook[]>(responseFromServer);
		}

		/// <summary>
		/// Fetchs a workbook.
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookID">Workbook identifier.</param>
		public async Task<Workbook> FetchWorkbook(int workbookID)
		{
			var responseFromServer = await FetchTextFromURLAsync(URL + "workbooks/" + workbookID);
			var workbook = JsonConvert.DeserializeObject<Workbook>(responseFromServer);

			await FetchPages(workbook);
			await Task.WhenAll(
				workbook.Pages.Cast<PageProxy>().Select(page => page.Resolve())
			);

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
			                                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
			                                                     "w" + proxy.Workbook.Id + "s" + proxy.Number + ".mp3"));
			Page page = new Page();
			page.Description = proxy.Description;
			page.Number = proxy.Number;
			page.Workbook = proxy.Workbook;
			page.workbookId = page.Workbook.Id;
			page.TeacherTrack = recording;
			page.teacherTrackId = page.TeacherTrack.Id;

			return page;

		}

		/// <summary>
		/// Fetchs the pages for a workbook.
		/// </summary>
		/// <returns>Nothing, the pages are attached to the given workbook object</returns>
		/// <param name="workbook">Workbook.</param>
		public async Task FetchPages(Workbook workbook)
		{
			var responseFromServer = await FetchTextFromURLAsync($"{URL}workbooks/{workbook.Id}/pages");
			workbook.Pages.AddRange(JsonConvert.DeserializeObject<List<PageProxy>>(responseFromServer));

			foreach (var page in workbook.Pages)
			{
				page.Workbook = workbook;
			}
		}

		/// <summary>
		/// Fetchs a teacher track.
		/// </summary>
		/// <returns>The teacher track.</returns>
		/// <param name="workbook">Workbook.</param>
		/// <param name="page">Page.</param>
		/// <param name="localPath">Local path.</param>
		public Task<Recording> FetchTeacherTrack(Workbook workbook, IPage page, String localPath)
		{
			return FetchFromURLAsync($"{URL}workbooks/{workbook.Id}/pages/{page.Number}", async (response) =>
			{
				using (var fileStream = File.Create(localPath))
					await response.GetResponseStream().CopyToAsync(fileStream);

				return new Recording(int.Parse(response.Headers["X-Recording-Id"]), 
				                     int.Parse(response.Headers["X-Audio-Duration"]), 
				                     localPath,
				                     DateTime.Parse(response.Headers["X-Recording-Creation-Time"]));
			});
		}
	}
}
