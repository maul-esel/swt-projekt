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
