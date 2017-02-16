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
			CurrentRecordingName = FormatRecordingName(page.TeacherTrack);
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

		private string FormatRecordingName(Recording recording)
		{
			string time = recording.CreationTime.ToString("dd. MM. yyyy u\\m HH:mm U\\hr");
			if (recording.LocalPath.StartsWith("uploaded_"))
			{
				return "Hochgeladen am " + time;
			}
			else if (recording.LocalPath.StartsWith("recorded_"))
			{
				return "Aufgenommen am " + time;
			}
			return recording.LocalPath;
		}
	}
}