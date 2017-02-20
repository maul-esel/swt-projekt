using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
	using Common.Entities;
	using ViewModels;
	using Services;

	/// <summary>
	/// Controller responsible for working with workbooks.
	/// </summary>
#if !DEBUG
	[RequireHttps]
#endif
	[Authorize]
	public class WorkbookController : Controller
	{
		/// <summary>
		/// Displays the view listing all workbooks.
		/// </summary>
		public IActionResult Index([FromServices] CloudLibrary cl)
		{
			return View(cl.FindWorkbooksWithReferences());
		}

		/// <summary>
		/// Displays the internal error view.
		/// </summary>
		[AllowAnonymous]
		public IActionResult Error()
		{
			return View();
		}

		/// <summary>
		/// Displays the workbook the the given <paramref name="id"/>.
		/// </summary>
		[Route("workbooks/{id}")]
		public IActionResult Workbook([FromServices] CloudLibrary cl, int id)
		{
			var workbook = cl.FindWorkbookWithReferences(id);
			if (workbook == null)
				return NotFound();

			return View(workbook);
		}

		/// <summary>
		/// Displays the view for creating a new workbook.
		/// </summary>
		[Route("workbooks/add")]
		public IActionResult AddWorkbook()
		{
			ViewData["Title"] = "Neues Arbeitsheft erstellen";
			return View(new WorkbookModel());
		}

		/// <summary>
		/// Creates a new workbook with the given data.
		/// </summary>
		[HttpPost, Route("workbooks/add")]
		public async Task<IActionResult> AddWorkbook([FromServices] CloudLibrary cl, WorkbookModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var w = new Workbook()
			{
				Title = model.Title,
				Subtitle = model.Subtitle
			};

			await cl.Save(w);
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// Displays the view for editing the existing workbook with the given <paramref name="id"/>.
		/// </summary>
		[Route("workbooks/{id}/edit")]
		public IActionResult EditWorkbook([FromServices] CloudLibrary cl, int id)
		{
			var workbook = cl.FindWorkbookWithReferences(id);
			if (workbook == null)
				return NotFound();

			ViewData["Title"] = "Arbeitsheft bearbeiten";
			return View(nameof(AddWorkbook), new WorkbookModel(workbook));
		}

		/// <summary>
		/// Updates an existing workbook with the data in the given <paramref name="model"/>.
		/// </summary>
		[HttpPost, Route("workbooks/{id}/edit")]
		public async Task<IActionResult> EditWorkbook([FromServices] CloudLibrary cl, int id, WorkbookModel model)
		{
			var workbook = cl.FindWorkbookWithReferences(id);
			if (workbook == null)
				return NotFound();

			if (!ModelState.IsValid)
			{
				model.Workbook = workbook;
				return View(nameof(AddWorkbook), model);
			}

			workbook.Title = model.Title;
			workbook.Subtitle = model.Subtitle;
			await cl.Save(workbook);

			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// Publishes the workbook with the given <paramref name="id"/> if permissible.
		/// </summary>
		[Route("workbooks/{id}/publish")]
		public async Task<IActionResult> PublishWorkbook([FromServices] CloudLibrary cl, int id)
		{
			var workbook = cl.FindWorkbookWithReferences(id);
			if (workbook == null)
				return NotFound();

			if (workbook.IsPublished || workbook.TotalPages > 0) // only publish non-empty workbooks (unpublish any)
			{
				workbook.IsPublished = !workbook.IsPublished;
				await cl.Save(workbook);
			}
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// Deletes the workbook wit hthe given <paramref name="id"/>.
		/// </summary>
		[Route("workbooks/{id}/delete")]
		public async Task<IActionResult> DeleteWorkbook([FromServices] CloudLibrary cl, int id)
		{
			var workbook = cl.FindWorkbookWithReferences(id);
			if (workbook == null)
				return NotFound();

			await cl.Delete(workbook);
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// Remote validation method verifying a workbook's title is unique.
		/// </summary>
		[Route("workbooks/validate/unique")]
		public async Task<IActionResult> UniqueWorkbookTitle([FromServices] CloudLibrary cl, int? id, string title)
		{
			var workbook = await cl.FindWorkbookByTitleAsync(title);
			return Json(data: workbook == null || workbook.Id == id);
		}
	}
}
