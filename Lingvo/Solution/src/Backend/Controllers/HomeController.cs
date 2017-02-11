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
    public class HomeController : Controller
    {
		private IHostingEnvironment environment;

		public HomeController(IHostingEnvironment environment)
		{
			this.environment = environment;
		}

        public IActionResult Index([FromServices] DatabaseService db)
        {
			return View(db.Workbooks);
        }

		[Route("workbooks/add")]
		public IActionResult AddWorkbook() 
		{
			ViewData["Title"] = "Neues Arbeitsheft erstellen";
			return View();
		}

		[Route("workbooks/{id}/edit")]
		public IActionResult EditWorkbook([FromServices] DatabaseService db, int id)
		{
			ViewData["Title"] = "Arbeitsheft bearbeiten";

			var workbook = db.Find<Workbook>(id);
			return View("AddWorkbook", workbook);
		}

		[Route("workbooks/{id}/publish")]
		public IActionResult PublishWorkbook([FromServices] DatabaseService db, int id)
		{
			var workbook = db.Find<Workbook>(id);
			workbook.IsPublished = !workbook.IsPublished;
			db.Save(workbook);
			return RedirectToAction(nameof(Index));
		}

		[Route("workbooks/{id}/delete")]
		public IActionResult DeleteWorkbook([FromServices] DatabaseService db, int id)
		{
			var workbook = db.Find<Workbook>(id);
			db.Delete(workbook);
			return RedirectToAction(nameof(Index));
		}

		[Route("workbooks/{workbookId}/pages/add")]
		public IActionResult AddPage([FromServices] DatabaseService db, int workbookId)
		{
			ViewData["Title"] = "Neue Seite erstellen";

			var workbook = db.Find<Workbook>(workbookId);
			return View(new PageModel() { Workbook = workbook });
		}

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

		[Route("workbooks/{id}")]
        public IActionResult Workbook([FromServices] DatabaseService db, int id)
        {
			var workbook = db.FindWorkbookWithReferences(id);
			return View(workbook);
        }

		[AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

		[HttpPost]
		public IActionResult CreateWorkbook([FromServices] DatabaseService db, string title, string subtitle)
		{
			var w = new Workbook()
			{
				Title = title,
				Subtitle = subtitle
			};

			db.Save(w);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> UpdatePage([FromServices] DatabaseService db, [FromServices] IStorage storage, int id, PageModel model)
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

			return RedirectToAction(nameof(Workbook), model.WorkbookID);
		}

		[HttpPost]
		public async Task<IActionResult> CreatePage([FromServices] DatabaseService db, [FromServices] IStorage storage, PageModel model)
		{
			var recording = await SaveRecording(db, storage, model);
			db.Save(new Page()
			{
				Number = model.PageNumber,
				Description = model.Description,
				workbookId = model.WorkbookID,
				teacherTrackId = recording.Id
			});
			return RedirectToAction(nameof(Workbook), model.WorkbookID);
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
				Duration = 10,
				LocalPath = fileName
			};
			db.Save(recording);

			return recording;
		}
	}
}
