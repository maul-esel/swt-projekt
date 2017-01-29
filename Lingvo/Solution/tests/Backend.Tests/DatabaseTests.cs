using System;
using System.IO;
using System.Linq;

using Lingvo.Common.Entities;

using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Lingvo.Backend.Tests
{
	public class TestsFixture : IDisposable
	{
		public TestsFixture()
		{
			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
				.AddEnvironmentVariables("LINGVO_");
			var config = builder.Build();

			DatabaseService.Connect(config);

			DatabaseService Database = DatabaseService.getNewContext();
			Database.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL", "server.sql")));
		}

		public void Dispose()
		{
		}
	}

    public class DatabaseTests : IClassFixture<TestsFixture>
    {
		public DatabaseTests()
		{
			DatabaseService Database = DatabaseService.getNewContext();
			Database.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL","DummyDataForServer.sql")));
		}

		public void SetFixture(TestsFixture data)
		{
		}

        [Fact]
        public void TestLoadTables()
        {
			DatabaseService Database = DatabaseService.getNewContext();
			Assert.Equal(4, Database.Pages.Count());
			Assert.Equal(4, Database.Recordings.Count());
			Assert.Equal(2, Database.Workbooks.Count());
        }

		[Fact]
		public void TestFindWorkbooks()
		{
			DatabaseService Database = DatabaseService.getNewContext();
			Assert.NotNull(Database.Workbooks.Find(1));
			Assert.NotNull(Database.Workbooks.Find(2));
			Assert.Null(Database.Workbooks.Find(3));
		}

		[Fact]
		public void TestFindPages()
		{
			DatabaseService Database = DatabaseService.getNewContext();
			Assert.NotNull(Database.Pages.Find(1));
			Assert.NotNull(Database.Pages.Find(4));
			Assert.Null(Database.Pages.Find(5));
		}

		[Fact]
		public void TestFindRecordings()
		{
			DatabaseService Database = DatabaseService.getNewContext();
			Assert.NotNull(Database.Recordings.Find(1));
			Assert.NotNull(Database.Recordings.Find(4));
			Assert.Null(Database.Recordings.Find(5));
		}

		[Fact]
		public void TestWorkbookPages()
		{
			DatabaseService Database = DatabaseService.getNewContext();
			var dbWorkbook = Database.GetWorkbooksWithReferences().Find(w => w.Id == 1);
			Assert.Equal(2, dbWorkbook.Pages.Count);

			var newWorkbook = new Workbook() { Title = "Irgendein Workbook", Subtitle = "neu erstellt" };
			Assert.NotNull(newWorkbook.Pages);
			Assert.Equal(0, newWorkbook.Pages.Count);

			newWorkbook.Pages.Add(new Page() { Description = "eine Seite", Number = 13, Workbook = newWorkbook });
			Assert.Equal(1, newWorkbook.Pages.Count);
		}

		[Fact]
		public void TestSaveWorkbook()
		{
			DatabaseService Database = DatabaseService.getNewContext();
			var testWorkbook = new Workbook()
			{
				Title = "Test",
				Subtitle = "Test"
			};

			Database.Save(testWorkbook);
			Assert.Equal(3, Database.Workbooks.Count());
			Assert.NotNull(Database.Workbooks.Find(testWorkbook.Id));
		}

		[Fact]
		public void TestSavePage()
		{
			DatabaseService Database = DatabaseService.getNewContext();
			var testPage = new Page()
			{
				Number = 5,
				workbookId = 1,
				Workbook = Database.Workbooks.Find(1),
				Description = "Test",
				TeacherTrack = Database.Recordings.Find(1),
				teacherTrackId = 1					
			};

			Console.WriteLine(testPage.StudentTrack);
			Console.WriteLine(testPage.studentTrackId);
			Database.Save(testPage);
			Assert.Equal(5, Database.Pages.Count());
			Assert.NotNull(Database.Pages.Find(testPage.Id));
		}

		[Fact]
		public void TestSaveRecording()
		{
			DatabaseService Database = DatabaseService.getNewContext();
			var testRecording = new Recording()
			{
				Duration = 12 /* milliseconds */,
				LocalPath = "test"
			};

			Database.Save(testRecording);
			Assert.Equal(5, Database.Recordings.Count());

			var savedRecording = Database.Recordings.Find(testRecording.Id);
			Assert.NotNull(savedRecording);
			Assert.Equal(12, savedRecording.Duration);
		}
    }
}
