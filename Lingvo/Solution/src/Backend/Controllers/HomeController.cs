using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
			var workbook = db.Workbooks.Find(id);
			db.Delete(workbook);
			return RedirectToAction(nameof(Index));
		}

		[Route("workbooks/{workbookId}/pages/add")]
		public IActionResult AddPage([FromServices] DatabaseService db, int workbookId)
		{
			ViewData["Title"] = "Neue Seite erstellen";

			var workbook = db.Find<Workbook>(workbookId);
			return View(new Tuple<Workbook, Page>(workbook, null));
		}

		[Route("pages/edit/{id}")]
		public IActionResult EditPage([FromServices] DatabaseService db, int id)
		{
			ViewData["Title"] = "Seite bearbeiten";

			var page = db.Find<Page>(id);
			var workbook = db.Find<Workbook>(page.workbookId);
			return View("AddPage", Tuple.Create(workbook, page));
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
		public async Task<IActionResult> UploadFile(IFormFile file)
		{

			var uploads = Path.Combine(environment.WebRootPath, "uploaded");
			var filePath = Path.Combine(uploads, file.FileName);

			Console.WriteLine(filePath);
			Console.WriteLine(file);
			//TODO: Create Recording and save it to database
			//TODO: Get sound duration somehow from file => crazy magic!

			var stream = new FileStream(filePath, FileMode.Create);
			await file.CopyToAsync(stream);
			stream.Dispose();

			return View("Workbook"); // TODO: redirect to proper workbook page
		}
	}
}
