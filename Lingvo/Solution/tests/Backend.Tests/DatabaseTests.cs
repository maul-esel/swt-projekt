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
				.AddEnvironmentVariables();
			var config = builder.Build();

			DatabaseTests.ConnectionString = config["MYSQLCONNSTR_localdb"];
			var db = DatabaseService.Connect(DatabaseTests.ConnectionString);
			db.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL", "server.sql")));
		}

		public void Dispose()
		{
		}
	}

    public class DatabaseTests : IClassFixture<TestsFixture>
    {
		public static string ConnectionString { get; set; }

		public DatabaseTests()
		{
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
			Database.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL","DummyDataForServer.sql")));
		}

		public void SetFixture(TestsFixture data)
		{
		}

        [Fact]
        public void TestLoadTables()
        {
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
			Assert.Equal(4, Database.Pages.Count());
			Assert.Equal(4, Database.Recordings.Count());
			Assert.Equal(2, Database.Workbooks.Count());
        }

		[Fact]
		public void TestFindWorkbooks()
		{
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
			Assert.NotNull(Database.Workbooks.Find(1));
			Assert.NotNull(Database.Workbooks.Find(2));
			Assert.Null(Database.Workbooks.Find(3));
		}

		[Fact]
		public void TestFindPages()
		{
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
			Assert.NotNull(Database.Pages.Find(1));
			Assert.NotNull(Database.Pages.Find(4));
			Assert.Null(Database.Pages.Find(5));
		}

		[Fact]
		public void TestFindRecordings()
		{
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
			Assert.NotNull(Database.Recordings.Find(1));
			Assert.NotNull(Database.Recordings.Find(4));
			Assert.Null(Database.Recordings.Find(5));
		}

		[Fact]
		public void TestWorkbookPages()
		{
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
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
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
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
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
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
			DatabaseService Database = DatabaseService.Connect(ConnectionString);
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

		[Fact]
		public void TestTeacherTrackChange()
		{
			var db = DatabaseService.Connect(ConnectionString);

			var page1 = db.FindPageWithRecording(1);
			var page2 = db.FindPageWithRecording(2);

			Assert.Equal(1, page1.TeacherTrack.Id);
			Assert.Equal(2, page2.TeacherTrack.Id);

			page1.TeacherTrack = page2.TeacherTrack;
			db.Save(page1);

			page1 = db.FindPageWithRecording(1);
			Assert.Equal(2, page1.TeacherTrack.Id);
		}
    }
}
