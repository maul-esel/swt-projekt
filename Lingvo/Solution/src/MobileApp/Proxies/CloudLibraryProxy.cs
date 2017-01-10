using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lingvo.Common;
using Lingvo.Common.Entities;

using Lingvo.MobileApp;
using MobileApp.Entities;

namespace MobileApp.Proxies
{
	/// <summary>
	/// Cloud library proxy.
	/// </summary>
	public class CloudLibraryProxy
	{
		private static CloudLibraryProxy instance;

		private APIService service;

		private CloudLibraryProxy()
		{
			service = new APIService();
		}

		/// <summary>
		/// Gets the instance of cloud library proxy (singleton pattern).
		/// </summary>
		/// <returns>The instance.</returns>
		public static CloudLibraryProxy Instance => instance ?? (instance = new CloudLibraryProxy());

		/// <summary>
		/// Downloads a single page.
		/// </summary>
		/// <returns>The page.</returns>
		/// <param name="proxy">A proxy for the page that is downloaded</param>
		internal Task<Page> DownloadSinglePage(PageProxy proxy)
		{
			return service.FetchPage(proxy);
		}

		/// <summary>
		/// Downloads a workbook.
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookID">Workbook identifier.</param>
		public async Task<Workbook> DownloadWorkbook(int workbookID)
		{
			var workbook = await service.FetchWorkbook(workbookID);
			var downloadedPages = new List<IPage>();

			foreach (var page in workbook.Pages)
			{
				var downloadedPage = await DownloadSinglePage((PageProxy) page);
				downloadedPages.Add(downloadedPage);
			}

			workbook.Pages = downloadedPages;

			var collection = LocalCollection.GetInstance();

			collection.AddWorkbook(workbook);

			return workbook;
		}

		public Task<Workbook[]> FetchAllWorkbooks()
		{
			return service.FetchWorkbooks();
		}
	}
}
