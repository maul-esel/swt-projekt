using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Lingvo.Common.Entities;

using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using Lingvo.Backend.Controllers;
using Lingvo.Backend.ViewModels;

namespace Lingvo.Backend.Tests
{
	public class HomeControllerTests
	{
		public HomeControllerTests()
		{
			TestsFixture.Setup();
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString);
			Database.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL", "DummyDataForServer.sql")));
		}


		[Fact]
		public void TestLoadBackend()
		{
			var controller = new WorkbookController();

			var result = controller.Index(DatabaseService.Connect(TestsFixture.ConnectionString));

			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void TestWorkbookPublishingAndUnpublishing()
		{
			var controller = new WorkbookController();
			var db = DatabaseService.Connect(TestsFixture.ConnectionString);

			controller.PublishWorkbook(db, 1);
			Assert.Equal(db.FindWorkbookWithReferences(1).IsPublished, false);

			controller.PublishWorkbook(db, 1);
			Assert.Equal(db.FindWorkbookWithReferences(1).IsPublished, true);

			db.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL", "DummyDataForServer.sql")));

			Assert.NotNull(db.Pages.Find(1));
			Assert.NotNull(db.Pages.Find(2));
			Assert.NotNull(db.Pages.Find(3));
			Assert.NotNull(db.Pages.Find(4));
		}

		[Fact]
		public void TestWorkbookDeletion()
		{
			var controller = new WorkbookController();
			var db = DatabaseService.Connect(TestsFixture.ConnectionString);

			Assert.NotNull(db.FindWorkbookWithReferences(1));
			Assert.NotNull(db.Pages.Find(1));
			Assert.NotNull(db.Recordings.Find(1));

			controller.DeleteWorkbook(db, 1);
			Assert.Null(db.FindWorkbookWithReferences(1));
			Assert.Null(db.Pages.Find(1));
			Assert.Null(db.Recordings.Find(1));

		}

		[Fact]
		public void TestPageDeletion()
		{
			var controller = new PageController();
			var db = DatabaseService.Connect(TestsFixture.ConnectionString);

			Assert.NotNull(db.Pages.Find(1));
			Assert.NotNull(db.Recordings.Find(1));

			controller.DeletePage(db, 1);
			Assert.Null(db.Pages.Find(1));
			Assert.Null(db.Recordings.Find(1));

		}

		[Fact]
		public async Task TestEditPage()
		{
			var controller = new PageController();
			var db = DatabaseService.Connect(TestsFixture.ConnectionString);

			var result = await controller.EditPage(db, new StorageMock(), 1);

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<PageModel>(viewResult.ViewData.Model);
			Assert.Equal(1, model.Page.Id);
			Assert.Equal(1, model.Workbook.Id);
			Assert.Equal("abc", model.CurrentRecordingUrl);
		}

		[Fact]
		public void TestCreateWorkbook()
		{
			var controller = new WorkbookController();
			var db = DatabaseService.Connect(TestsFixture.ConnectionString);

			var result = controller.CreateWorkbook(db, "NeuesWorkbook", "Nur zum Testen");

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);

			Assert.Equal(3, db.Workbooks.Count());
			Assert.Equal("NeuesWorkbook", db.Workbooks.Last().Title);
			Assert.Equal("Nur zum Testen", db.Workbooks.Last().Subtitle);
		}

		[Fact]
		public async Task TestUpdatePage()
		{
			var controller = new PageController();
			var db = DatabaseService.Connect(TestsFixture.ConnectionString);

			var p = new PageModel
			{
				Description = "new",
				PageNumber = 11
			};

			var result = await controller.UpdatePage(db, new StorageMock(), 1, p);

			Assert.Equal("new", db.FindPageWithRecording(1).Description);
			Assert.Equal(11, db.FindPageWithRecording(1).Number);
		}

		[Fact]
		public async Task TestCreatePage()
		{
			var controller = new PageController();
			var db = DatabaseService.Connect(TestsFixture.ConnectionString);

			var p = new PageModel
			{
				Description = "new",
				PageNumber = 11,
				WorkbookID = 1,
				UploadedFile = new FileMock()
			};

			var result = await controller.CreatePage(db, new StorageMock(), p);

			var w = db.FindWorkbookWithReferences(1);
			Assert.Equal(5, db.Pages.Count());

			foreach (var page in w.Pages)
			{
				if (page.Id != 1 && page.Id != 2)
				{
					Assert.Equal("new", page.Description);
					Assert.Equal(11, page.Number);
					break;
				}
			}
		}
	}
}
