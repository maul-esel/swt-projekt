using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
			Id = page.Id;
			Workbook = page.Workbook;
			CurrentRecordingUrl = recordingUrl;
			CurrentRecordingName = FormatRecordingName(page.TeacherTrack);
		}

		/// <summary>
		/// Optional. The identifier of the page being edited.
		/// </summary>
		public int? Id { get; set; }

		/// <summary>
		/// The page to be edited.
		/// </summary>
		/// <remarks>Passed from the controller to the view.</remarks>
		[BindNever]
		public Page Page { get; set; }

		/// <summary>
		/// The workbook containing the new / edited page.
		/// </summary>
		/// <remarks>Passed from the controller to the view.</remarks>
		[BindNever]
		public Workbook Workbook { get; set; }

		/// <summary>
		/// If the model is created for an existing page,
		/// points to its recording.
		/// </summary>
		/// <remarks>Passed from the controller to the view.</remarks>
		[BindNever]
		public string CurrentRecordingUrl { get; }

		/// <summary>
		/// If the model is created for an existing page,
		/// contains the display name of its current recording.
		/// </summary>
		/// <remarks>Passed from the controller to the view.</remarks>
		public string CurrentRecordingName { get; }

		/// <summary>
		/// The id of the workbook containing the new / edited page.
		/// </summary>
		/// <remarks>Passed from the view to the controller.</remarks>
		[Required]
		public int WorkbookID { get; set; }

		/// <summary>
		/// The new description for the page.
		/// </summary>
		/// <remarks>Passed from the view to the controller.</remarks>
		public string Description { get; set; }

		/// <summary>
		/// The page's new page number.
		/// </summary>
		/// <remarks>Passed from the view to the controller.</remarks>
		[Required(ErrorMessage = "Es muss eine Seitennummer angegeben werden.")]
		[Remote(action: nameof(Controllers.PageController.UniquePageNumber), controller: "Page",
			AdditionalFields = "WorkbookID,Id",
			ErrorMessage = "Eine Seite mit dieser Seitenzahl existiert bereits.")]
		public int PageNumber { get; set; }

		/// <summary>
		/// Contains an uploaded MP3 file, if the user has selected one.
		/// </summary>
		/// <remarks>Passed from the view to the controller.</remarks>
		public IFormFile UploadedFile { get; set; }

		/// <summary>
		/// Contains a newly recorded MP3 file, if the user has created one.
		/// </summary>
		/// <remarks>Passed from the view to the controller.</remarks>
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