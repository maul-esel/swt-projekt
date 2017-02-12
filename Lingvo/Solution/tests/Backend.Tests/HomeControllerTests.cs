using System;
using System.IO;
using System.Linq;

using Lingvo.Common.Entities;

using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using Lingvo.Backend.Controllers;

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
			var controller = new HomeController(null);

			var result = controller.Index(DatabaseService.Connect(TestsFixture.ConnectionString));

			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void TestWorkbookPublishingAndUnpublishing()
		{
			var controller = new HomeController(null);
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
			var controller = new HomeController(null);
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
			var controller = new HomeController(null);
			var db = DatabaseService.Connect(TestsFixture.ConnectionString);

			Assert.NotNull(db.Pages.Find(1));
			Assert.NotNull(db.Recordings.Find(1));

			controller.DeletePage(db, 1);
			Assert.Null(db.Pages.Find(1));
			Assert.Null(db.Recordings.Find(1));

		}
	}
}
