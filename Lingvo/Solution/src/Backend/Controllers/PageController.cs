using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Lingvo.Backend.ViewModels;
using Lingvo.Common.Entities;


namespace Lingvo.Backend.Controllers
{
	/// <summary>
	/// Controller responsible for working with pages in a workbook.
	/// </summary>
	#if !DEBUG
		[RequireHttps]
	#endif
	[Authorize]
	public class PageController : Controller
	{
		/// <summary>
		/// Displays the view for adding a page to the workbook indicated by <paramref name="workbookId"/>.
		/// If <paramref name="pageNumber"/> is specified, this is shown as default.
		/// </summary>
		[Route("workbooks/{workbookId}/pages/add")]
		public IActionResult AddPage([FromServices] CloudLibrary cl, int workbookId, int? pageNumber)
		{
			var workbook = cl.FindWorkbookWithReferences(workbookId);
			if (workbook == null)
				return NotFound();

			ViewData["Title"] = "Neue Seite erstellen";
			ViewData["pageNumber"] = pageNumber;

			return View(new PageModel() { Workbook = workbook });
		}

		/// <summary>
		/// Adds a page with the data given in <paramref name="model"/> to a workbook.
		/// If <paramref name="nextPage"/> is <c>true</c>, redirects to the add / edit view
		/// for the next page.
		/// </summary>
		[HttpPost, Route("workbooks/{workbookId}/pages/add")]
		public async Task<IActionResult> AddPage([FromServices] CloudLibrary cl, PageModel model, bool nextPage = false)
		{
			if (!ModelState.IsValid)
				return View(model);

			var recording = await SaveRecording(cl, model);
			await cl.Save(new Page()
			{
				Number = model.PageNumber,
				Description = model.Description,
				workbookId = model.WorkbookID,
				teacherTrackId = recording.Id
			});

			if (nextPage)
				return await RedirectToNextPage(cl, model.WorkbookID, model.PageNumber);
			else
				return Redirect("/workbooks/" + model.WorkbookID);
		}

		/// <summary>
		/// Displays the view for editing the existing page wit the given <paramref name="id"/>.
		/// </summary>
		[Route("pages/edit/{id}")]
		public async Task<IActionResult> EditPage([FromServices] CloudLibrary cl, int id)
		{
			ViewData["Title"] = "Seite bearbeiten";

			var page = cl.FindPageWithRecording(id);
			if (page == null)
				return NotFound();

			var workbook = cl.FindWorkbookWithReferences(page.workbookId);
			var recordingUrl = await cl.GetAccessUrlAsync(page.TeacherTrack);

			var model = new PageModel(page, recordingUrl) { Workbook = workbook };
			return View(nameof(AddPage), model);
		}

		/// <summary>
		/// Updates a page with the data in the given <paramref name="model"/>.
		/// If <paramref name="nextPage"/> is <c>true</c>, redirects to the add / edit view
		/// for the next page.
		/// </summary>
		[HttpPost, Route("pages/edit/{id}")]
		public async Task<IActionResult> EditPage([FromServices] CloudLibrary cl, int id, PageModel model, bool nextPage = false)
		{
			var page = cl.FindPageWithRecording(id);
			if (page == null)
				return NotFound();

			if (!ModelState.IsValid)
			{
				model.Page = page;
				return View(nameof(AddPage), model);
			}

			if (model.UploadedFile != null || model.RecordedFile != null)
			{
				var recording = await SaveRecording(cl, model);
				await cl.Delete(page.TeacherTrack);
				page.TeacherTrack = recording;
			}

			page.Description = model.Description;
			page.Number = model.PageNumber;
			await cl.Save(page);

			if (nextPage)
				return await RedirectToNextPage(cl, page.workbookId, page.Number);
			else
				return Redirect("/workbooks/" + model.WorkbookID);
		}

		/// <summary>
		/// Deletes the page with the given <paramref name="id"/>.
		/// Redirects to the workbook view for the workbook containing the page.
		/// </summary>
		[Route("pages/{id}/delete")]
		public async Task<IActionResult> DeletePage([FromServices] CloudLibrary cl, int id)
		{
			var page = cl.FindPageWithRecording(id);
			if (page == null)
				return NotFound();

			var workbookID = page.workbookId;
			await cl.Delete(page);
			return Redirect("/workbooks/" + workbookID);
		}

		/// <summary>
		/// Helper method for redirecting to the add / edit view for the next page.
		/// </summary>
		private async Task<IActionResult> RedirectToNextPage([FromServices] CloudLibrary cl, int workbookId, int pageNumber)
		{
			var page = await cl.FindPageByNumberAsync(workbookId, pageNumber + 1);
			if (page == null)
				return RedirectToAction(nameof(AddPage), new { workbookId, pageNumber = pageNumber + 1 });
			return RedirectToAction(nameof(EditPage), new { id = page.Id });
		}

		/// <summary>
		/// Helper method for saving an uploaded recording.
		/// </summary>
		private async Task<Recording> SaveRecording([FromServices] CloudLibrary cl, PageModel model)
		{
			var file = model.RecordedFile ?? model.UploadedFile;
			if (file == null)
				throw new InvalidDataException("no file uploaded");

			string prefix = (model.RecordedFile == null) ? "uploaded_" : "recorded_";
			string fileName = prefix + Guid.NewGuid().ToString();

			using (var stream = file.OpenReadStream())
				return await cl.CreateRecording(stream, fileName, model.Duration);
		}

		/// <summary>
		/// Remote validation method to verify page numbers are unique in a workbook.
		/// </summary>
		[Route("pages/validate/unique")]
		public async Task<IActionResult> UniquePageNumber([FromServices] CloudLibrary cl, int? id, int workbookId, int pageNumber)
		{
			var page = await cl.FindPageByNumberAsync(workbookId, pageNumber);
			return Json(data: page == null || page.Id == id);
		}
	}
}
