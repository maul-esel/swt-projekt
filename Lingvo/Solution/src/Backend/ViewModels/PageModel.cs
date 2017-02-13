using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Lingvo.Common.Entities;

namespace Lingvo.Backend.ViewModels
{
	/// <summary>
	/// A model for the page creation / update view. This is both used to pass
	/// data to the view, as well as from the view to the controller (on submit)
	/// via ASP.NET Core model-binding.
	/// </summary>
    public class PageModel
    {
		public PageModel() { }

		/// <summary>
		/// Creates a new model initialized with data about the given page.
		/// </summary>
		public PageModel(Page page, string recordingUrl)
		{
			Page = page;
			Workbook = page.Workbook;
			CurrentRecordingUrl = recordingUrl;
			CurrentRecordingName = FormatRecordingName(page.TeacherTrack.LocalPath);
		}

		[BindNever]
		public Page Page { get; }

		[BindNever]
		public Workbook Workbook { get; set; }

		/// <summary>
		/// If the model is created for an existing page,
		/// points to its recording.
		/// </summary>
		[BindNever]
		public string CurrentRecordingUrl { get; }

		/// <summary>
		/// If the model is created for an existing page,
		/// contains the display name of its current recording.
		/// </summary>
		public string CurrentRecordingName { get; }

		[Required]
		public int WorkbookID { get; set; }

		public string Description { get; set; }

		[Required]
		public int PageNumber { get; set; }

		public IFormFile UploadedFile { get; set; }

		public IFormFile RecordedFile { get; set; }

		public int Duration { get; set; }

		private string FormatRecordingName(string filename)
		{
			return filename; // TODO: proper display name, uploaded date, ...
		}
	}
}