using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Lingvo.Backend.ViewModels;
using Lingvo.Common.Entities;


namespace Lingvo.Backend.Controllers
{
	#if !DEBUG
		[RequireHttps]
	#endif
	[Authorize]
	public class PageController : Controller
	{

		[Route("pages/edit/{id}")]
		public async Task<IActionResult> EditPage([FromServices] CloudLibrary cl, int id)
		{
			ViewData["Title"] = "Seite bearbeiten";

			var page = cl.FindPageWithRecording(id);
			var workbook = cl.FindWorkbookWithReferences(page.workbookId);
			var recordingUrl = await cl.GetAccessUrlAsync(page.TeacherTrack.LocalPath);

			var model = new PageModel(page, recordingUrl) { Workbook = workbook };
			return View("AddPage", model);
		}

		[Route("pages/delete/{id}")]
		public async Task<IActionResult> DeletePage([FromServices] CloudLibrary cl, int id)
		{
			var page = cl.FindPageWithRecording(id);
			var workbookID = page.workbookId;
			await cl.Delete(page);
			return Redirect("/workbooks/" + workbookID);
		}

		[HttpPost]
		public async Task<IActionResult> UpdatePage([FromServices] CloudLibrary cl, int id, PageModel model, bool nextPage = false)
		{
			var page = cl.FindPageWithRecording(id);

			if (model.UploadedFile != null || model.RecordedFile != null)
			{
				var recording = await SaveRecording(cl, model);
				await cl.DeleteAsyncFromStorage(page.TeacherTrack.LocalPath);
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

		[HttpPost]
		public async Task<IActionResult> CreatePage([FromServices] CloudLibrary cl, PageModel model, bool nextPage = false)
		{
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

		private async Task<IActionResult> RedirectToNextPage([FromServices] CloudLibrary cl, int workbookId, int pageNumber)
		{
			var page = await cl.FindPageByNumberAsync(workbookId, pageNumber + 1);
			if (page == null)
				return RedirectToAction(nameof(AddPage), new { workbookId, pageNumber = pageNumber + 1 });
			return RedirectToAction(nameof(EditPage), new { id = page.Id });
		}

		private async Task<Recording> SaveRecording([FromServices] CloudLibrary cl, PageModel model)
		{
			var file = model.RecordedFile ?? model.UploadedFile;
			if (file == null)
				throw new InvalidDataException("no file uploaded");

			string prefix = (model.RecordedFile == null) ? "uploaded_" : "recorded_";
			string fileName = prefix + Guid.NewGuid().ToString();

			return await cl.SaveRecordingToStorage(file, fileName, model.Duration);
		}
	
		[Route("workbooks/{workbookId}/pages/add")]
		public IActionResult AddPage([FromServices] CloudLibrary cl, int workbookId)
		{
			ViewData["Title"] = "Neue Seite erstellen";

			var workbook = cl.FindWorkbookWithReferences(workbookId);
			return View(new PageModel() { Workbook = workbook });
		}
	}
}
