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

			var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
			if (string.IsNullOrEmpty(password))
				password = "password"; // dummy default value for development
			Database.ConnectionString = $"Server={Configuration["host"]};Port={Configuration["port"]};Database={Configuration["db"]};Uid={Configuration["user"]};Pwd={password};charset=utf8;";

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
