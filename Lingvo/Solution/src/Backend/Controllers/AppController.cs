using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
	using Common.Services;

	/// <summary>
	/// Controller for App accessing the server
	/// </summary>
	[Route("api/app")]
	public class AppController : Controller
    {
		private DatabaseService Database => Startup.Database;

		/// <summary>
		/// Gets all workbooks without pages.
		/// </summary>
		/// <returns>The workbooks.</returns>
		[Route("workbooks"), HttpGet]
		public IActionResult GetWorkbooks()
		{
			return Json(from w in Database.Workbooks
						where w.IsPublished
						select new { w.Id, w.Title, w.Subtitle, w.LastModified, w.TotalPages });
		}

		/// <summary>
		/// Gets a workbook without pages.
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookId">Workbook identifier.</param>
		[Route("workbooks/{workbookId}")]
		public IActionResult GetWorkbook(int workbookId)
		{

			return Json((from workbook in Database.Workbooks
						where workbook.IsPublished 
			            && workbook.Id == workbookId
			             select new { workbook.Id, workbook.Title, workbook.Subtitle, workbook.LastModified, workbook.TotalPages}).Single());
		}

		/// <summary>
		/// Gets the pages as proxies for the workbook with the given id.
		/// </summary>
		/// <returns>The pages.</returns>
		/// <param name="workbookId">Workbook identifier.</param>
		[Route("workbooks/{workbookId}/pages")]
		public IActionResult GetPages(int workbookId)
		{
			return Json(from p in Database.Pages
						where p.workbookId == workbookId
						select new { p.workbookId, p.Number, p.Description });
		}

		/// <summary>
		/// Gets the teacher track from the workbook with the page number.
		/// </summary>
		/// <returns>The teacher track.</returns>
		/// <param name="workbookId">Workbook identifier.</param>
		/// <param name="pageNumber">Page number.</param>
		[Route("workbooks/{workbookId}/pages/{pageNumber}")]
		public IActionResult GetTeacherTrack(int workbookId, int pageNumber)
		{
			var page = Database.Pages.Find(workbookId, pageNumber);
			if (page == null)
				return NotFound();

			Response.Headers["X-Audio-Duration"] = page.TeacherTrack.Duration.ToString();
			Response.Headers["X-Recording-Id"] = page.TeacherTrack.Id.ToString();
			Response.Headers["X-Recording-Creation-Time"] = page.TeacherTrack.CreationTime.ToString();

			//relative and absolute paths supported
			string path = null;
			if (Path.IsPathRooted(page.TeacherTrack.LocalPath))
			{
				path = page.TeacherTrack.LocalPath;
			}
			else
			{
				path = Path.Combine(Directory.GetCurrentDirectory(), page.TeacherTrack.LocalPath);
			}


			return new FileContentResult(System.IO.File.ReadAllBytes(path), "audio/mpeg3")
			{
				FileDownloadName = $"w{workbookId}s{pageNumber}.mp3"
			};
		}

    }
}