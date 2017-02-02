using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
	/// <summary>
	/// Controller for App accessing the server
	/// </summary>
	[Route("api/app")]
	public class AppController : Controller
    {

		/// <summary>
		/// Gets all workbooks without pages.
		/// </summary>
		/// <returns>The workbooks.</returns>
		[Route("workbooks"), HttpGet]
		public IActionResult GetWorkbooks([FromServices] DatabaseService db)
		{
			return Json(from w in db.Workbooks
						where w.IsPublished
						select new { w.Id, w.Title, w.Subtitle, w.LastModified, w.TotalPages });
		}

		/// <summary>
		/// Gets a workbook without pages.
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookId">Workbook identifier.</param>
		[Route("workbooks/{workbookId}")]
		public IActionResult GetWorkbook([FromServices] DatabaseService db, int workbookId)
		{
			var workbook = db.FindWorkbookWithReferences(workbookId);
			if (workbook.IsPublished)
				return Json(new { workbook.Id, workbook.Title, workbook.Subtitle, workbook.LastModified, workbook.TotalPages });
			return Unauthorized();
		}

		/// <summary>
		/// Gets the pages as proxies for the workbook with the given id.
		/// </summary>
		/// <returns>The pages.</returns>
		/// <param name="workbookId">Workbook identifier.</param>
		[Route("workbooks/{workbookId}/pages")]
		public IActionResult GetPages([FromServices] DatabaseService db, int workbookId)
		{
			return Json(from p in db.Pages
						where p.workbookId == workbookId
						select new { p.Id, p.workbookId, p.Number, p.Description });
		}

		/// <summary>
		/// Gets the teacher track from the workbook with the page number.
		/// </summary>
		/// <returns>The teacher track.</returns>
		[Route("pages/{pageId}")]
		public async Task<IActionResult> GetTeacherTrack([FromServices] DatabaseService db, [FromServices] IStorage storage, int pageId)
		{
			var page = db.FindPageWithRecording(pageId);
			if (page == null)
				return NotFound();

			return Json(new {
				duration = page.TeacherTrack.Duration,
				creationTime = page.TeacherTrack.CreationTime.ToString("dd.MM.yyyy HH:mm:ss"),
				url = await storage.GetAccessUrlAsync(page.TeacherTrack.LocalPath)
			});
		}
    }
}