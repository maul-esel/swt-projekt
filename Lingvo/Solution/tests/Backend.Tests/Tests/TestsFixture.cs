using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace Lingvo.Backend.Tests
{
	public class TestsFixture 
	{
		public static string ConnectionString { get; set; }

		private static Boolean isExecuted = false;

		public static void Setup()
		{
			if (!isExecuted)
			{
				var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				var builder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json")
					.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
					.AddEnvironmentVariables();
				var config = builder.Build();

				ConnectionString = config["MYSQLCONNSTR_localdb"];
				var db = DatabaseService.Connect(ConnectionString);
				db.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL", "server.sql")));

				isExecuted = true;
			}
		}

	}
}
