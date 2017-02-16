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
		public async Task<IActionResult> EditPage([FromServices] DatabaseService db, [FromServices] IStorage storage, int id)
		{
			ViewData["Title"] = "Seite bearbeiten";

			var page = db.FindPageWithRecording(id);
			var workbook = await db.FindAsync<Workbook>(page.workbookId);
			var recordingUrl = await storage.GetAccessUrlAsync(page.TeacherTrack.LocalPath);

			var model = new PageModel(page, recordingUrl) { Workbook = workbook };
			return View("AddPage", model);
		}

		[Route("pages/delete/{id}")]
		public IActionResult DeletePage([FromServices] DatabaseService db, int id)
		{
			var page = db.Find<Page>(id);
			var workbookID = page.workbookId;
			db.Delete(page);
			return Redirect("/workbooks/" + workbookID);
		}

		[HttpPost]
		public async Task<IActionResult> UpdatePage([FromServices] DatabaseService db, [FromServices] IStorage storage, int id, PageModel model, bool nextPage = false)
		{
			var page = db.FindPageWithRecording(id);

			if (model.UploadedFile != null || model.RecordedFile != null)
			{
				var recording = await SaveRecording(db, storage, model);
				await storage.DeleteAsync(page.TeacherTrack.LocalPath);
				db.Delete(page.TeacherTrack);
				page.TeacherTrack = recording;
			}

			page.Description = model.Description;
			page.Number = model.PageNumber;
			db.Save(page);


			if (nextPage)
				return await RedirectToNextPage(db, page.workbookId, page.Number);
			else
				return Redirect("/workbooks/" + model.WorkbookID);
		}

		[HttpPost]
		public async Task<IActionResult> CreatePage([FromServices] DatabaseService db, [FromServices] IStorage storage, PageModel model, bool nextPage = false)
		{
			var recording = await SaveRecording(db, storage, model);
			db.Save(new Page()
			{
				Number = model.PageNumber,
				Description = model.Description,
				workbookId = model.WorkbookID,
				teacherTrackId = recording.Id
			});

			if (nextPage)
				return await RedirectToNextPage(db, model.WorkbookID, model.PageNumber);
			else
				return Redirect("/workbooks/" + model.WorkbookID);
		}

		private async Task<IActionResult> RedirectToNextPage(DatabaseService db, int workbookId, int pageNumber)
		{
			var page = await db.FindPageByNumberAsync(workbookId, pageNumber + 1);
			if (page == null)
				return RedirectToAction(nameof(AddPage), new { workbookId, pageNumber = pageNumber + 1 });
			return RedirectToAction(nameof(EditPage), new { id = page.Id });
		}

		private async Task<Recording> SaveRecording(DatabaseService db, IStorage storage, PageModel model)
		{
			var file = model.RecordedFile ?? model.UploadedFile;
			if (file == null)
				throw new InvalidDataException("no file uploaded");

			string prefix = (model.RecordedFile == null) ? "uploaded_" : "recorded_";
			string fileName = prefix + Guid.NewGuid().ToString();

			Stream stream = null;
			try
			{
				stream = file.OpenReadStream();
				await storage.SaveAsync(fileName, stream);
			}
			finally
			{
				stream?.Dispose();
			}

			var recording = new Recording()
			{
				Duration = model.Duration,
				LocalPath = fileName
			};
			db.Save(recording);

			return recording;
		}

		[Route("workbooks/{workbookId}/pages/add")]
		public IActionResult AddPage([FromServices] DatabaseService db, int workbookId, int? pageNumber)
		{
			ViewData["Title"] = "Neue Seite erstellen";

			var workbook = db.Find<Workbook>(workbookId);
			ViewData["pageNumber"] = pageNumber;
			return View(new PageModel() { Workbook = workbook });
		}
	}
}
