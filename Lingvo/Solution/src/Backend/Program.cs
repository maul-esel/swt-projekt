using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.Data;
using LinqToDB.DataProvider.MySql;

namespace Lingvo.Backend
{
	using Common;

    public class Program
    {
        public static void Main(string[] args)
        {
			using (var conn = MySqlTools.CreateDataConnection("Server=127.0.0.1;Port=3306;Database=lingvo;Uid=root;Pwd=password;charset=utf8;"))
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
