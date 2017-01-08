using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.MySql;

namespace Lingvo.Backend.Controllers
{
	using Common;

	[Route("api/app")]
	public class AppController : Controller
    {
		private DataConnection _connection;

		private DataConnection Connection
		{
			get
			{
				if (_connection == null)
				{
					_connection = MySqlTools.CreateDataConnection("Server=127.0.0.1;Port=3306;Database=lingvo;Uid=root;Pwd=password;charset=utf8;");
					_connection.MappingSchema.SetConverter<int, TimeSpan>(ms => TimeSpan.FromMilliseconds(ms));
				}
				return _connection;
			}
		}

		[Route("workbooks"), HttpGet]
		public IActionResult GetWorkbooks()
		{
			var workbooks = Connection.GetTable<Workbook>().LoadWith(w => w.Pages);
			return Json(from w in workbooks
						where w.IsPublished
						select new { w.Id, w.Title, w.Subtitle, w.LastModified, w.TotalPages });
		}

		[Route("workbooks/{workbookId}/pages")]
		public IActionResult GetPages(int workbookId)
		{
			var pages = Connection.GetTable<Page>();
			return Json(from p in pages
						where p.workbookId == workbookId
						select new { p.workbookId, p.Number, p.Description });
		}

		[Route("workbooks/{workbookId}/pages/{pageNumber}")]
		public IActionResult GetTeacherTrack(int workbookId, int pageNumber)
		{
			var pages = Connection.GetTable<Page>().LoadWith(p => p.TeacherTrack);
			var page = pages.FirstOrDefault(p => p.workbookId == workbookId && p.Number == pageNumber);
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