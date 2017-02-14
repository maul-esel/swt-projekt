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
	public class WorkbookController : Controller
	{

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
			return View(new PageModel() { Workbook = workbook });
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
	}
}
