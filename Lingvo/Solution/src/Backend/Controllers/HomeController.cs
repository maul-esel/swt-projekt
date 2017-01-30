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

        public IActionResult Index()
        {
            return View();
        }

		[Route("workbooks/add")]
		public IActionResult AddWorkbook() 
		{
			ViewData["Title"] = "Neues Arbeitsheft erstellen";
			return View();
		}

		[Route("workbooks/{id}/edit")]
		public IActionResult EditWorkbook(int id)
		{
			ViewData["Workbook"] = DatabaseService.getNewContext().Find<Workbook>(id);
			ViewData["Title"] = "Arbeitsheft bearbeiten";
			return View("AddWorkbook");
		}

		[Route("workbooks/{workbookId}/pages/add")]
		public IActionResult AddPage(int workbookId)
		{
			ViewData["Workbook"] = DatabaseService.getNewContext().Find<Workbook>(workbookId);
			ViewData["Title"] = "Neue Seite erstellen";
			return View();
		}

		[Route("pages/edit/{id}")]
		public IActionResult EditPage(int id)
		{
			var context = DatabaseService.getNewContext();
			var page = context.Find<Page>(id);
			ViewData["Workbook"] = context.Find<Workbook>(page.workbookId);;
			ViewData["Page"] = page;

			ViewData["Title"] = "Seite bearbeiten";
			return View("AddPage");
		}

		[Route("workbooks/{id}")]
        public IActionResult Workbook(int id)
        {
			var workbook = DatabaseService.getNewContext().FindWorkbookWithReferences(id);
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
