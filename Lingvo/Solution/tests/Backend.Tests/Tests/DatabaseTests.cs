using System;
using System.IO;
using System.Linq;

using Lingvo.Common.Entities;

using Xunit;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using Lingvo.Backend.Controllers;

namespace Lingvo.Backend.Tests
{
	
    public class DatabaseTests 
    {
		public DatabaseTests()
		{
			TestsFixture.Setup();
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
			Database.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL","DummyDataForServer.sql")));
		}


        [Fact]
        public void TestLoadTables()
        {
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
			Assert.Equal(4, Database.Pages.Count());
			Assert.Equal(4, Database.Recordings.Count());
			Assert.Equal(2, Database.Workbooks.Count());
        }

		[Fact]
		public void TestFindWorkbooks()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
			Assert.NotNull(Database.Workbooks.Find(1));
			Assert.NotNull(Database.Workbooks.Find(2));
			Assert.Null(Database.Workbooks.Find(3));
		}

		[Fact]
		public void TestFindPages()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
			Assert.NotNull(Database.Pages.Find(1));
			Assert.NotNull(Database.Pages.Find(4));
			Assert.Null(Database.Pages.Find(5));
		}

		[Fact]
		public void TestFindRecordings()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
			Assert.NotNull(Database.Recordings.Find(1));
			Assert.NotNull(Database.Recordings.Find(4));
			Assert.Null(Database.Recordings.Find(5));
		}

		[Fact]
		public void TestWorkbookPages()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
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
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
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
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
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
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
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

		public void TestSaveWorkbookWithPagesAndRecording()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());
			var testWorkbook = new Workbook()
			{
				Title = "Test",
				Subtitle = "Test"
			};

			var testRecording = new Recording()
			{
				Duration = 12 /* milliseconds */,
				LocalPath = "test"
			};

			var testPage = new Page()
			{
				Number = 5,
				workbookId = 1,
				Workbook = testWorkbook,
				Description = "Test",
				TeacherTrack = testRecording,
				teacherTrackId = 1
			};

			testWorkbook.Pages.Add(testPage);

			Database.Save(testWorkbook);

			var savedWorkbook = Database.FindWorkbookWithReferences(testWorkbook.Id);
			Assert.NotNull(savedWorkbook);
			Assert.Equal(savedWorkbook.Subtitle, "Test");
			Assert.Equal(savedWorkbook.Pages.Count, 1);

			Assert.Equal(((Page) savedWorkbook.Pages.ElementAt(0)).Number, 5);
			Assert.Equal(((Page)savedWorkbook.Pages.ElementAt(0)).Description, "Test");
			Assert.Equal(((Page)savedWorkbook.Pages.ElementAt(0)).Workbook.Id, testWorkbook.Id);
			Assert.Equal(((Page)savedWorkbook.Pages.ElementAt(0)).TeacherTrack.LocalPath, "test");
		}

		[Fact]
		public void TestDeleteRecording()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());

			var testRecording = new Recording()
			{
				Duration = 12 /* milliseconds */,
				LocalPath = "test"
			};

			Database.Save(testRecording);
			Assert.Equal(5, Database.Recordings.Count());

			Database.Delete(Database.Recordings.Find(testRecording.Id));

			Assert.Equal(Database.Recordings.Count(), 4);
			Assert.Null(Database.Recordings.Find(testRecording.Id));
		}

		[Fact]
		public void TestDeletePage()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());

			var v = Database.Pages.Find(1);
			Database.Delete(v);

			Assert.Null(Database.Pages.Find(1));
			Assert.NotNull(Database.Pages.Find(2));
			Assert.NotNull(Database.Pages.Find(3));
			Assert.NotNull(Database.Pages.Find(4));
			Assert.Null(Database.Recordings.Find(1));
		}

		[Fact]
		public void TestDeleteWorkbook()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());

			var deletionWorkbook = Database.FindWorkbookWithReferences(1);
			Database.Delete(deletionWorkbook);

			Assert.Equal(Database.Workbooks.Count(), 1);
			Assert.Null(Database.Pages.Find(1));
			Assert.Null(Database.Pages.Find(2));
			Assert.Null(Database.Recordings.Find(1));
			Assert.Null(Database.Recordings.Find(2));
		}


		public void TestTeacherTrackChange()
		{
			var db = DatabaseService.Connect(TestsFixture.ConnectionString, new StorageMock());

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
