using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
		public IActionResult AddWorkbook() 
		{
			ViewData["Title"] = "Neues Arbeitsheft erstellen";
			return View();
		}
		public IActionResult EditWorkbook()
		{
			ViewData["Title"] = "Arbeitsheft bearbeiten";
			return View("AddWorkbook");
		}
		public IActionResult AddPage() 
		{
			ViewData["Title"] = "Neue Seite erstellen";
			return View();
		}

		public IActionResult EditPage()
		{
			ViewData["Title"] = "Seite bearbeiten";
			return View("AddPage");
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
    }
}
