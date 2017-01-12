using Xunit;
using System.Linq;
using Lingvo.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace Lingvo.Backend.Tests
{
    public class DatabaseTests
    {

		private DatabaseService Database;

		public DatabaseTests()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");

			var Configuration = builder.Build();

			var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
			if (string.IsNullOrEmpty(password))
				password = "password"; // dummy default value for development
			Database = new DatabaseService(
				$"Server={Configuration["host"]};Port={Configuration["port"]};Database={Configuration["db"]};Uid={Configuration["user"]};Pwd={password};charset=utf8;"
			);
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
		public void TestSaveWorkbook()
		{
			//TODO
		}

		[Fact]
		public void TestSavePage()
		{
			//TODO
		}

		[Fact]
		public void TestSaverecording()
		{
			//TODO
		}
    }
}
