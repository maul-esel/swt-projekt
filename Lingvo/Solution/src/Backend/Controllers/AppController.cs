using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
	using Common;

	[Route("api/app")]
	public class AppController : Controller
    {
		[Route("workbooks"), HttpGet]
		public IActionResult GetWorkbooks()
		{
			return Json(from w in Database.Workbooks
						where w.IsPublished
						select new { w.Id, w.Title, w.Subtitle, w.LastModified, w.TotalPages });
		}

		[Route("workbooks/{workbookId}/pages")]
		public IActionResult GetPages(int workbookId)
		{
			return Json(from p in Database.Pages
						where p.workbookId == workbookId
						select new { p.workbookId, p.Number, p.Description });
		}

		[Route("workbooks/{workbookId}/pages/{pageNumber}")]
		public IActionResult GetTeacherTrack(int workbookId, int pageNumber)
		{
			var page = Database.Pages.Find(workbookId, pageNumber);
			if (page == null)
				return NotFound();

			// TODO: page.LocalPath might be relative
			return new FileContentResult(System.IO.File.ReadAllBytes(page.TeacherTrack.LocalPath), "audio/mpeg3")
			{
				FileDownloadName = $"w{workbookId}s{pageNumber}.mp3"
			};
		}
    }
}