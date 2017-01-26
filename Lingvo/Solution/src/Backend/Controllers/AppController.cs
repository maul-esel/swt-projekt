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
		public IActionResult GetWorkbooks()
		{
			var Database = DatabaseService.getNewContext();
			return Json(from w in Database.GetWorkbooksWithReferences()
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
			var Database = DatabaseService.getNewContext();
			return Json((from workbook in Database.GetWorkbooksWithReferences()
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
			var Database = DatabaseService.getNewContext();
			return Json(from p in Database.GetPagesWithReferences()
						where p.workbookId == workbookId
						select new { p.Id, p.workbookId, p.Number, p.Description });
		}

		/// <summary>
		/// Gets the teacher track from the workbook with the page number.
		/// </summary>
		/// <returns>The teacher track.</returns>
		[Route("pages/{pageId}")]
		public IActionResult GetTeacherTrack(int pageId)
		{
			var Database = DatabaseService.getNewContext();

			var page = Database.GetPagesWithReferences().Find(p => p.Id == pageId);

			Console.WriteLine("TeacherTrackId:" + page.teacherTrackId);

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
				FileDownloadName = $"page{pageId}.mp3"
			};
		}

    }
}