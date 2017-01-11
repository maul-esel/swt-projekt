using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
	using Common.Services;

	[Route("api/app")]
	public class AppController : Controller
    {
		private DatabaseService Database => Program.Database;

		[Route("workbooks"), HttpGet]
		public IActionResult GetWorkbooks()
		{
			return Json(from w in Database.Workbooks
						where w.IsPublished
						select new { w.Id, w.Title, w.Subtitle, w.LastModified, w.TotalPages });
		}

		[Route("workbooks/{workbookId}")]
		public IActionResult GetWorkbook(int workbookId)
		{

			return Json((from workbook in Database.Workbooks
						where workbook.IsPublished 
			            && workbook.Id == workbookId
			             select new { workbook.Id, workbook.Title, workbook.Subtitle, workbook.LastModified, workbook.TotalPages}).Single());
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
			
			Response.Headers["X-Audio-Length"] = page.TeacherTrack.Length.ToString();
			Response.Headers["X-Recording-Id"] = page.TeacherTrack.Id.ToString();
			Response.Headers["X-Recording-Creation-Time"] = page.TeacherTrack.CreationTime.ToString();

			// TODO: page.LocalPath might be relative
			return new FileContentResult(System.IO.File.ReadAllBytes(page.TeacherTrack.LocalPath), "audio/mpeg3")
			{
				FileDownloadName = $"w{workbookId}s{pageNumber}.mp3"
			};
		}
    }
}