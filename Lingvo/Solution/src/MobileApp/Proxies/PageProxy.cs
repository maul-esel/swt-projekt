using System;
using System.Threading.Tasks;
using Lingvo.Common;
using Lingvo.Common.Entities;
using Lingvo.Common.Services;

namespace Lingvo.MobileApp.Proxies
{
	public class PageProxy : IPage
	{
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

		public async Task Resolve()
		{

			if (original == null)
			{
				var service = CloudLibraryProxy.Instance;
				var page = await service.DownloadSinglePage(this);
				original = page;

				var db = App.Database;
				if (db.Workbooks.Find(this.Workbook.Id) != null)
				{
					db.Save(this.Workbook);
				}

				db.Save(original.TeacherTrack);
				db.Save(original);
			}
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
