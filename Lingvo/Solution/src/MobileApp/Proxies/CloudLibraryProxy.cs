using System.Threading.Tasks;

using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using System;
using System.Threading;

namespace Lingvo.MobileApp.Proxies
{
    /// <summary>
    /// Interface for communication with the server: downloading and fetching available data
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
		/// <param name="cancellationToken">Token for cancelling the download</param>
        internal async Task<Page> DownloadSinglePage(PageProxy proxy, CancellationTokenSource cancellationToken)
        {
            return await service.FetchPage(proxy, cancellationToken);
        }

        /// <summary>
        /// Downloads a whole workbook and adds it to the local collection
        /// </summary>
        /// <returns>The workbook.</returns>
        /// <param name="workbookID">Workbook identifier.</param>
		/// <param name="cancellationToken">Token for cancelling the download</param>
        public async Task<Workbook> DownloadWorkbook(int workbookID, CancellationTokenSource cancellationToken)
        {
            var workbook = await service.FetchWorkbook(workbookID, cancellationToken);

            if (workbook != null)
            {
                LocalCollection.Instance.AddWorkbook(workbook);
            }

            return workbook;
        }

        /// <summary>
        /// Returns a list of all workbooks with page proxy objects for all existing pages on the server
        /// </summary>
        /// <returns>The all workbooks.</returns>
        public async Task<Workbook[]> FetchAllWorkbooks()
        {
            var workbooks = await service.FetchWorkbooks();

            if (workbooks != null)
            {
                foreach (var w in workbooks)
                {
                    await service.FetchPages(w);
                    w.IsPublished = true;
                }
            }

            return workbooks;
        }
    }
}
