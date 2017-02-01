using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;

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
		public IActionResult GetTeacherTrack([FromServices] DatabaseService db, int pageId)
		{
			var page = db.FindPageWithRecording(pageId);
			if (page == null)
				return NotFound();

			Response.Headers["X-Audio-Duration"] = page.TeacherTrack.Duration.ToString();
			Response.Headers["X-Recording-Creation-Time"] = page.TeacherTrack.CreationTime.ToString("dd.MM.yyyy HH:mm:ss");

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
				FileDownloadName = $"page{pageId}.mp3"
			};
		}

    }
}