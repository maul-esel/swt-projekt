using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lingvo.Backend
{
    public class Startup
	{
		private const string ConnectionStringVariable = "MYSQLCONNSTR_localdb";

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
			Configuration = builder.Build();

			// read DB connection string from system environment variables if present (as it is on azure)
			var envConnectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable);
			if (!string.IsNullOrEmpty(envConnectionString))
				Configuration[ConnectionStringVariable] = ConvertAzureDatabaseConfiguration(envConnectionString);
		}

		private string ConvertAzureDatabaseConfiguration(string connectionString)
		{
			var m = new Regex(@"Database=(?<db>[^;]*);Data Source=(?<host>[^:;]*):(?<port>\d+);User Id=(?<user>[^;]*);Password=(?<password>[^;]*)").Match(connectionString);
			return $"Server={m.Groups["host"]};port={m.Groups["port"]};database={m.Groups["db"]};uid={m.Groups["user"]};pwd={m.Groups["password"]};";
		}

		public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			// Add framework services
            services.AddMvc().AddDataAnnotationsLocalization();
			services.AddDbContext<DatabaseService>(
				options => options.UseMySql(Configuration[ConnectionStringVariable]),
				ServiceLifetime.Transient
			);

			services.AddIdentity<Editor, object>()
				.AddUserStore<Services.EditorStore>()
				.AddRoleStore<Services.RoleStore>()
				.AddDefaultTokenProviders();

			// custom services
			services.AddScoped<IStorage, AzureStorage>();
			services.AddScoped<CloudLibrary, CloudLibrary>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

			app.UseStatusCodePages();

			app.UseCookieAuthentication(new CookieAuthenticationOptions()
			{
				ExpireTimeSpan = TimeSpan.FromDays(7),
				SlidingExpiration = true
			});
			app.UseIdentity();
			app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Workbook}/{action=Index}/{id?}");
            });
        }
    }
}
