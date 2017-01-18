using System;
using System.IO;
using System.Linq;

using Lingvo.Common.Entities;
using Lingvo.Common.Services;

using Xunit;
using Microsoft.Extensions.Configuration;

namespace Lingvo.Backend.Tests
{
	public class TestsFixture : IDisposable
	{
		public TestsFixture()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");

			var Configuration = builder.Build();

			var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
			if (string.IsNullOrEmpty(password))
				password = "password"; // dummy default value for development
			DatabaseTests.Database = new DatabaseService(
				$"Server={Configuration["host"]};Port={Configuration["port"]};Database={Configuration["db"]};Uid={Configuration["user"]};Pwd={password};charset=utf8;"
			);

			DatabaseTests.Database.Execute(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL", "server.sql")));
		
		}

		public void Dispose()
		{
		}
	}

    public class DatabaseTests : IClassFixture<TestsFixture>
    {

		public static DatabaseService Database;

		public DatabaseTests()
		{
			
			Database.Execute(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL","DummyDataForServer.sql")));
		}

		public void SetFixture(TestsFixture data)
		{
		}

        [Fact]
        public void TestLoadTables()
        {
			Assert.Equal(4, Database.Pages.Count());
			Assert.Equal(4, Database.Recordings.Count());
			Assert.Equal(2, Database.Workbooks.Count());
        }

		[Fact]
		public void TestFindWorkbooks()
		{
			Assert.NotNull(Database.Workbooks.Find(1));
			Assert.NotNull(Database.Workbooks.Find(2));
			Assert.Null(Database.Workbooks.Find(3));
		}

		[Fact]
		public void TestFindPages()
		{
			Assert.NotNull(Database.Pages.Find(1, 1));
			Assert.NotNull(Database.Pages.Find(2, 2));
			Assert.Null(Database.Pages.Find(1, 3));
			Assert.Null(Database.Pages.Find(3, 1));
		}

		[Fact]
		public void TestFindRecordings()
		{
			Assert.NotNull(Database.Recordings.Find(1));
			Assert.NotNull(Database.Recordings.Find(4));
			Assert.Null(Database.Recordings.Find(5));
		}

		[Fact]
		public void TestWorkbookPages()
		{
			var dbWorkbook = Database.Workbooks.Find(1);
			Assert.Equal(2, dbWorkbook.Pages.Count);

			var newWorkbook = new Workbook(42) { Title = "Irgendein Workbook", Subtitle = "neu erstellt" };
			Assert.NotNull(newWorkbook.Pages);
			Assert.Equal(0, newWorkbook.Pages.Count);

			newWorkbook.Pages.Add(new Page() { Description = "eine Seite", Number = 13, Workbook = newWorkbook });
			Assert.Equal(1, newWorkbook.Pages.Count);
		}

		[Fact]
		public void TestSaveWorkbook()
		{
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
			Assert.NotNull(Database.Pages.Find(1, 5));
		}

		[Fact]
		public void TestSaveRecording()
		{
			var testRecording = new Recording()
			{
				Length = TimeSpan.FromMilliseconds(12),
				LocalPath = "test"
			};

			Database.Save(testRecording);
			Assert.Equal(5, Database.Recordings.Count());

			var savedRecording = Database.Recordings.Find(testRecording.Id);
			Assert.NotNull(savedRecording);
			Assert.Equal(TimeSpan.FromMilliseconds(12), savedRecording.Length);
		}
    }
}
