using System;
using Lingvo.Common.Entities;
using System.Collections.Generic;

namespace MobileApp
{
	/// <summary>
	/// Cloud library proxy.
	/// </summary>
	public class CloudLibrary
	{
		private static CloudLibrary instance;

		private List<Workbook> workbooks;

		/// <summary>
		/// Gets or sets the workbooks, does not return null but an empty list.
		/// </summary>
		/// <value>The workbooks.</value>
		public List<Workbook> Workbooks
		{
			get
			{
				//Nullpointer avoidance
				if (workbooks == null)
				{
					workbooks = new List<Workbook>();
				}

				return workbooks;
			}

			set
			{
				workbooks = value;
			}
		}

		private CloudLibrary()
		{
		}

		/// <summary>
		/// Gets the instance of cloud library proxy (singleton pattern).
		/// </summary>
		/// <returns>The instance.</returns>
		public static CloudLibrary getInstance()
		{
			if (instance == null)
			{
				instance = new CloudLibrary();
			}

			return instance;
		}

		/// <summary>
		/// Creates a new workbook and adds it to the collection.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="subtitle">Subtitle.</param>
		public void CreateWorkbook(String title, String subtitle)
		{
			if (workbooks == null)
			{
				workbooks = new List<Workbook>();
			}

			Workbook workbook = new Workbook();
			workbook.Title = title;
			workbook.Subtitle = subtitle;

			workbooks.Add(workbook);
		}
	}
}
