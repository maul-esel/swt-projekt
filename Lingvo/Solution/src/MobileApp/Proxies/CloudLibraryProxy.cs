using System.Collections.Generic;
using System.Threading.Tasks;

using Lingvo.Common;
using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using System;
using Lingvo.MobileApp.Services;
using Xamarin.Forms;
using static Lingvo.MobileApp.APIService;
using System.Threading;

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
		internal async Task<Page> DownloadSinglePage(PageProxy proxy, IProgress<double> progress, CancellationToken cancellationToken)
		{if (DisplayWarningIfNotWifiConnected())
            {
			return await service.FetchPage(proxy, progress, cancellationToken);
}
return null;
		}

		/// <summary>
		/// Downloads a workbook and adds it to the local collection
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookID">Workbook identifier.</param>
		public async Task<Workbook> DownloadWorkbook(int workbookID, IProgress<double> progress, CancellationToken cancellationToken)
		{if (DisplayWarningIfNotWifiConnected())
            {
			var workbook = await service.FetchWorkbook(workbookID, progress, cancellationToken);


                LocalCollection.Instance.AddWorkbook(workbook);

                return workbook;
            }
            return null;
        }

        /// <summary>
        /// Returns a list of all workbooks, with page proxy objects for all existing pages on server
        /// </summary>
        /// <returns>The all workbooks.</returns>
        public async Task<Workbook[]> FetchAllWorkbooks()
        {
            var workbooks = await service.FetchWorkbooks();

            foreach (var w in workbooks)
            {
                await service.FetchPages(w);
                w.IsPublished = true;
            }

            return workbooks;
        }
    }
}
