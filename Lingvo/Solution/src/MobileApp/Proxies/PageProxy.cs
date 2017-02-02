using System;
using System.Threading.Tasks;
using Lingvo.Common;
using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;

namespace Lingvo.MobileApp.Proxies
{
	public class PageProxy : IPage
	{
        public int Id { get; set; }

        public delegate void OnPageChanged(int id);

		private int number;
		private String description;

		private Page original;

		public Workbook Workbook { get; set; }

		public int workbookId { get; set; }

		/// <summary>
		/// Gets or sets the page number.
		/// </summary>
		/// <value>The number.</value>
		public int Number
		{
			get
			{
				return number;
			}
			set
			{
				number = value;
			}
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public String Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		/// <summary>
		/// Gets or sets the teacher track if a real page exisits for this proxy
		/// </summary>
		/// <value>The teacher track.</value>
		public Recording TeacherTrack
		{
			get
			{
				if (original != null)
				{
					return original.TeacherTrack;
				}
				else
				{
					return null;
				}
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Gets or sets the student track if a real page exisits for this proxy
		/// </summary>
		/// <value>The student track.</value>
		public Recording StudentTrack
		{
			get
			{
				if (original != null)
				{
					return original.StudentTrack;
				}
				else
				{
					return null;
				}
			}
			set
			{
				throw new InvalidOperationException();
			}
		}


		public PageProxy()
		{
		}

		/// <summary>
		/// Load the real page object for this proxy
		/// </summary>
		/// <returns>The resolve.</returns>
		public async Task Resolve()
		{

			if (original == null)
			{
				var service = CloudLibraryProxy.Instance;

				var db = App.Database;

				if (db.FindWorkbook(this.Workbook.Id) == null)
				{
					LocalCollection.Instance.AddWorkbook(Workbook);

					await DownloadPage();
				}
				else
				{
					var p = db.FindPage(this.Id);

					if (p != null)
					{
						original = p;
					}
					else
					{
						await DownloadPage();
					}
				}

			}
		}

		private async Task DownloadPage()
		{
			var service = CloudLibraryProxy.Instance;
			var db = App.Database;

			var page = await service.DownloadSinglePage(this);
			original = page;

			db.Save(original.TeacherTrack);
			db.Save(original);
		}

		/// <summary>
		/// Deletes the student recording if a real page exisits for this proxy
		/// </summary>
		public void DeleteStudentRecording()
		{
			if (original != null)
			{
				original.DeleteStudentRecording();
			}
		}
	}
}
