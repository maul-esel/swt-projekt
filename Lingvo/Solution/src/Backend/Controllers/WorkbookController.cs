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

		public IActionResult Index([FromServices] CloudLibrary cl)
		{
			return View(cl.FindWorkbooksWithReferences());
		}

		[Route("workbooks/add")]
		public IActionResult AddWorkbook()
		{
			ViewData["Title"] = "Neues Arbeitsheft erstellen";
			return View();
		}

		[Route("workbooks/{id}/edit")]
		public IActionResult EditWorkbook([FromServices] CloudLibrary cl, int id)
		{
			ViewData["Title"] = "Arbeitsheft bearbeiten";

			var workbook = cl.FindWorkbookWithReferences(id);
			return View("AddWorkbook", workbook);
		}

		[Route("workbooks/{id}/publish")]
		public async Task<IActionResult> PublishWorkbook([FromServices] CloudLibrary cl, int id)
		{
			var workbook = cl.FindWorkbookWithReferences(id);
			if (workbook.IsPublished || workbook.TotalPages > 0) // only publish non-empty workbooks (unpublish any)
			{
				workbook.IsPublished = !workbook.IsPublished;
				await cl.Save(workbook);
			}
			return RedirectToAction(nameof(Index));
		}

		[Route("workbooks/{id}/delete")]
		public async Task<IActionResult> DeleteWorkbook([FromServices] CloudLibrary cl, int id)
		{
			var workbook = cl.FindWorkbookWithReferences(id);
			await cl.Delete(workbook);
			return RedirectToAction(nameof(Index));
		}

		[Route("workbooks/{id}")]
		public IActionResult Workbook([FromServices] CloudLibrary cl, int id)
		{
			var workbook = cl.FindWorkbookWithReferences(id);
			return View(workbook);
		}

		[AllowAnonymous]
		public IActionResult Error()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateWorkbook([FromServices] CloudLibrary cl, string title, string subtitle)
		{
			var w = new Workbook()
			{
				Title = title,
				Subtitle = subtitle
			};

			await cl.Save(w);
			return RedirectToAction(nameof(Index));
		}

	}
}
