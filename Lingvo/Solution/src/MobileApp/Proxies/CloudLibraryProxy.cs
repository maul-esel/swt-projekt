using System;
using Lingvo.Common;
using Lingvo.Common.Entities;

namespace MobileApp.Proxies
{
	/// <summary>
	/// Cloud library proxy.
	/// </summary>
	public class CloudLibraryProxy
	{
		private static CloudLibraryProxy instance;

		private CloudLibraryProxy()
		{
		}

		/// <summary>
		/// Gets the instance of cloud library proxy (singleton pattern).
		/// </summary>
		/// <returns>The instance.</returns>
		public static CloudLibraryProxy GetInstance()
		{
			if (instance == null)
			{
				instance = new CloudLibraryProxy();
			}

			return instance;
		}

		/// <summary>
		/// Downloads a single page.
		/// </summary>
		/// <returns>The page.</returns>
		/// <param name="workbookID">Workbook identifier.</param>
		/// <param name="pageNumber">Page number.</param>
		public Page DownloadSinglePage(String workbookID, int pageNumber)
		{
			// to do: get correspondent page from server
			return null;
		}

		/// <summary>
		/// Downloads a workbook.
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookID">Workbook identifier.</param>
		public Workbook DownloadWorkbook(String workbookID)
		{
			// to do: get correspondent workbook from server
			return null;
		}
	}
}
