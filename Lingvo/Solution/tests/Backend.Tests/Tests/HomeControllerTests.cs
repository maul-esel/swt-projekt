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

			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));
			var result = controller.Index(cl);

			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task TestWorkbookPublishingAndUnpublishing()
		{
			var controller = new WorkbookController();

			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));

			await controller.PublishWorkbook(cl, 1);
			Assert.Equal(cl.FindWorkbookWithReferences(1).IsPublished, false);

			await controller.PublishWorkbook(cl, 1);
			Assert.Equal(cl.FindWorkbookWithReferences(1).IsPublished, true);

			Assert.NotNull(cl.FindPageWithRecording(1));
			Assert.NotNull(cl.FindPageWithRecording(2));
			Assert.NotNull(cl.FindPageWithRecording(3));
			Assert.NotNull(cl.FindPageWithRecording(4));
		}

		[Fact]
		public async Task TestWorkbookDeletion()
		{
			var controller = new WorkbookController();
			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));

			Assert.NotNull(cl.FindWorkbookWithReferences(1));
			Assert.NotNull(cl.FindPageWithRecording(1));
			Assert.NotNull(cl.FindRecording(1));

			await controller.DeleteWorkbook(cl, 1);
			Assert.Null(cl.FindWorkbookWithReferences(1));
			Assert.Null(cl.FindPageWithRecording(1));
			Assert.Null(cl.FindRecording(1));

		}

		[Fact]
		public async Task TestPageDeletion()
		{
			var controller = new PageController();
			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));

			Assert.NotNull(cl.FindPageWithRecording(1));
			Assert.NotNull(cl.FindRecording(1));

			await controller.DeletePage(cl, 1);

			Assert.Null(cl.FindPageWithRecording(1));
			Assert.Null(cl.FindRecording(1));

		}

		[Fact]
		public async Task TestEditPage()
		{
			var controller = new PageController();
			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));

			var result = await controller.EditPage(cl, 1);

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
			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));

			var result = controller.AddWorkbook(cl, new WorkbookModel() { Title = "NeuesWorkbook", Subtitle = "Nur zum Testen" });

			Assert.Equal(3, cl.FindWorkbooksWithReferences().Count());
			Assert.Equal("NeuesWorkbook", cl.FindWorkbooksWithReferences().Last().Title);
			Assert.Equal("Nur zum Testen", cl.FindWorkbooksWithReferences().Last().Subtitle);
		}

		[Fact]
		public async Task TestUpdatePage()
		{
			var controller = new PageController();
			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));

			var p = new PageModel
			{
				Description = "new",
				PageNumber = 11
			};

			var result = await controller.EditPage(cl, 1, p);

			Assert.Equal("new", cl.FindPageWithRecording(1).Description);
			Assert.Equal(11, cl.FindPageWithRecording(1).Number);
		}

		[Fact]
		public async Task TestCreatePage()
		{
			var controller = new PageController();
			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));

			var p = new PageModel
			{
				Description = "new",
				PageNumber = 11,
				WorkbookID = 1,
				UploadedFile = new FileMock()
			};

			var result = await controller.AddPage(cl, p);

			var w = cl.FindWorkbookWithReferences(1);

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
