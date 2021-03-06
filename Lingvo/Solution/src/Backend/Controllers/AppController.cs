﻿using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
	using Services;

	/// <summary>
	/// The controller poviding the REST API consumed by the mobile app.
	/// Data is returned as JSON.
	/// </summary>
#if !DEBUG
	[RequireHttps]
#endif
	[Route("api/app")]
	public class AppController : Controller
    {

		/// <summary>
		/// Gets all workbooks without pages.
		/// </summary>
		/// <param name="db">Supplied by dependency injection.</param>
		/// <returns>The workbooks.</returns>
		[Route("workbooks"), HttpGet]
		public IActionResult GetWorkbooks([FromServices] DatabaseService db)
		{
			return Json(from w in db.Workbooks
						where w.IsPublished
			            orderby w.Id
						select new { w.Id, w.Title, w.Subtitle, w.LastModified, w.TotalPages });
		}

		/// <summary>
		/// Gets a workbook without pages.
		/// </summary>
		/// <param name="db">Supplied by dependency injection.</param>
		/// <param name="workbookId">Workbook identifier.</param>
		/// <returns>The workbook.</returns>
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
		/// <param name="db">Supplied by dependency injection.</param>
		/// <param name="workbookId">Workbook identifier.</param>
		/// <returns>The pages.</returns>
		[Route("workbooks/{workbookId}/pages")]
		public IActionResult GetPages([FromServices] DatabaseService db, int workbookId)
		{
			return Json(from p in db.Pages
						where p.workbookId == workbookId
			            orderby p.Id
						select new { p.Id, p.workbookId, p.Number, p.Description , creationTime = p.TeacherTrack.CreationTime.ToString("dd.MM.yyyy HH:mm:ss")});
		}

		/// <summary>
		/// Gets the teacher track from the workbook with the page number.
		/// </summary>
		/// <param name="cl">Supplied by dependency injection.</param>
		/// <param name="pageId">Page identifier.</param>
		/// <returns>Data concerning the teacher track, including a download URL.</returns>
		[Route("pages/{pageId}")]
		public async Task<IActionResult> GetTeacherTrack([FromServices] CloudLibrary cl, int pageId)
		{
			var page = cl.FindPageWithRecording(pageId);
			if (page == null)
				return NotFound();

			return Json(new {
				duration = page.TeacherTrack.Duration,
				creationTime = page.TeacherTrack.CreationTime.ToString("dd.MM.yyyy HH:mm:ss"),
				url = await cl.GetAccessUrlAsync(page.TeacherTrack)
			});
		}
    }
}