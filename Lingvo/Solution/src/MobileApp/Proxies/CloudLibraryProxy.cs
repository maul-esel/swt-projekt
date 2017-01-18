using System.Collections.Generic;
using System.Threading.Tasks;

using Lingvo.Common;
using Lingvo.Common.Entities;

namespace Lingvo.MobileApp.Proxies
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
			service = APIService.Instance;
		}

        internal Task Download(IDownloadable downloadable)
        {
            if(downloadable is PageProxy)
            {
                return DownloadSinglePage((PageProxy)downloadable);
            }
            return DownloadWorkbook(((Workbook)downloadable).Id);
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
		/// Downloads a workbook and adds it to the local collection
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookID">Workbook identifier.</param>
		public async Task<Workbook> DownloadWorkbook(int workbookID)
		{
			var workbook = await service.FetchWorkbook(workbookID);

			foreach (var page in workbook.Pages)
			{
				await ((PageProxy) page).Resolve();
			}

			//This is not necessary as workbooks is always read from the database

			//var collection = LocalCollection.GetInstance();

			//collection.AddWorkbook(workbook);

			return workbook;
		}

		/// <summary>
		/// Returns a list of all workbooks, with page proxy objects for all existing pages on server
		/// </summary>
		/// <returns>The all workbooks.</returns>
		public async Task<Workbook[]> FetchAllWorkbooks()
		{
			var workbooks =  await service.FetchWorkbooks();

			foreach (var w in workbooks)
			{
				await service.FetchPages(w);
				w.IsPublished = true;
			}

			return workbooks;
		}
	}
}
