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
		public IActionResult AddWorkbook() 
		{
			return View();
		}
		public IActionResult AddPage() 
		{
			return View();
		}


        public IActionResult Workbook()
		{

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

		internal bool DummyTestMethod()
		{
			return true;
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

			return View("Workbook");
		}
	}
}
