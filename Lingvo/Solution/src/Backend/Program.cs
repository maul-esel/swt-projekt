using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Lingvo.Backend
{
    public class Program
    {
		static public IConfigurationRoot Configuration { get; set; }

		static public DatabaseService Database { get; private set; }

        public static void Main(string[] args)
        {
			var builder = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");

			Configuration = builder.Build();

			var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
			if (string.IsNullOrEmpty(password))
				password = "password"; // dummy default value for development
			Database = new DatabaseService(
				$"Server={Configuration["host"]};Port={Configuration["port"]};Database={Configuration["db"]};Uid={Configuration["user"]};Pwd={password};charset=utf8;"
			);

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
