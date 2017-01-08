using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.Data;
using LinqToDB.DataProvider.MySql;

namespace Lingvo.Backend
{
	using Common;

    public class Program
    {
		static public IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
			var builder = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");

			Configuration = builder.Build();

			if (System.Environment.GetEnvironmentVariable("db-password") == null)
			{
				System.Environment.SetEnvironmentVariable("db-password", "password");
			}

			using (var conn = MySqlTools.CreateDataConnection("Server=" + Configuration["host"] + ";Port=" + Configuration["port"] + ";Database=" + Configuration["db"] + ";Uid=" + Configuration["user"] + ";Pwd=" + System.Environment.GetEnvironmentVariable("db-password") + ";charset=utf8;"))
			{
				conn.MappingSchema.SetConverter<int, TimeSpan>(ms => TimeSpan.FromMilliseconds(ms));

				var nonEmpty = from r in conn.GetTable<Recording>()
							   select r;
				foreach (var r in nonEmpty) Console.WriteLine(r.LocalPath +" "+ r.Length.Milliseconds +" "+ r.CreationTime);
				Console.WriteLine();

				var pages = conn.GetTable<Page>();
				foreach (var p in pages) Console.WriteLine(p.Number + " " + p.Description + " " + " " + p.Workbook?.Id + " "+ p.TeacherTrack?.LocalPath);
			}

			var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
