using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Lingvo.Common.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Lingvo.Backend.Controllers
{
    public class HomeController : Controller
    {

		private IHostingEnvironment environment;

		public HomeController(IHostingEnvironment environment)
		{
			this.environment = environment;
		}

        public IActionResult Index([FromServices] DatabaseService db)
        {
			ViewData["Workbooks"] = db.Workbooks;
			return View();
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
			ViewData["Workbook"] = db.Find<Workbook>(id);
			ViewData["Title"] = "Arbeitsheft bearbeiten";
			return View("AddWorkbook");
		}

		[Route("workbooks/{workbookId}/pages/add")]
		public IActionResult AddPage([FromServices] DatabaseService db, int workbookId)
		{
			ViewData["Workbook"] = db.Find<Workbook>(workbookId);
			ViewData["Title"] = "Neue Seite erstellen";
			return View();
		}

		[Route("pages/edit/{id}")]
		public IActionResult EditPage([FromServices] DatabaseService db, int id)
		{
			var page = db.Find<Page>(id);
			ViewData["Workbook"] = db.Find<Workbook>(page.workbookId);;
			ViewData["Page"] = page;

			ViewData["Title"] = "Seite bearbeiten";
			return View("AddPage");
		}

		[Route("workbooks/{id}")]
        public IActionResult Workbook([FromServices] DatabaseService db, int id)
        {
			var workbook = db.FindWorkbookWithReferences(id);
	        ViewData["workbook"] = workbook;
			return View();
        }

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
		public async Task<IActionResult> UpdatePage([FromServices] DatabaseService db, [FromServices] IStorage storage, int id, ViewModels.PageModel model)
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
		public async Task<IActionResult> CreatePage([FromServices] DatabaseService db, [FromServices] IStorage storage, ViewModels.PageModel model)
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

		private async Task<Recording> SaveRecording(DatabaseService db, IStorage storage, ViewModels.PageModel model)
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
