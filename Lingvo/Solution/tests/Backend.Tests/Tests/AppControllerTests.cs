using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Lingvo.Backend.Tests
{
	using Controllers;
	using Services;

	public class AppControllerTests
	{

		public AppControllerTests()
		{
			TestsFixture.Setup();
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString);
			Database.Database.ExecuteSqlCommand(File.ReadAllText(Path.Combine("bin", "Debug", "netcoreapp1.0", "SQL", "DummyDataForServer.sql")));

		}

		[Fact]
		public void TestGetWorkbooks()
		{
			DatabaseService db = DatabaseService.Connect(TestsFixture.ConnectionString);
			var controller = new AppController();

			var result = controller.GetWorkbooks(db);
			Assert.IsType<JsonResult>(result);

			var json = JsonConvert.SerializeObject(((JsonResult)result).Value);

			Assert.Equal("[{\"Id\":1,\"Title\":\"Deutschkurs für Asylbewerber\",\"Subtitle\":\"Thannhauser Modell\",\"LastModified\":\"",
			             json.Substring(0, 96));

			Assert.Equal("\",\"TotalPages\":2},{\"Id\":2,\"Title\":\"Willkommen! Die deutsche Sprache - erste Schritte\",\"Subtitle\":\"Flüchtlingshilfe München e.V.\",\"LastModified\":\"",
						json.Substring(115, 145));

			Assert.Equal("\",\"TotalPages\":2}]",
			            json.Substring(279));
			
		}

		[Fact]
		public void TestGetWorkbook()
		{
			DatabaseService db = DatabaseService.Connect(TestsFixture.ConnectionString);
			var controller = new AppController();

			var result = controller.GetWorkbook(db, 1);
			Assert.IsType<JsonResult>(result);

			var json = JsonConvert.SerializeObject(((JsonResult)result).Value);

			Assert.Equal("{\"Id\":1,\"Title\":\"Deutschkurs für Asylbewerber\",\"Subtitle\":\"Thannhauser Modell\",\"LastModified\":\"",
						json.Substring(0, 95));

			Assert.Equal("\",\"TotalPages\":2}",
						json.Substring(114));

			result = controller.GetWorkbook(db, 2);
			Assert.IsType<JsonResult>(result);

			json = JsonConvert.SerializeObject(((JsonResult)result).Value);

			Assert.Equal("{\"Id\":2,\"Title\":\"Willkommen! Die deutsche Sprache - erste Schritte\",\"Subtitle\":\"Flüchtlingshilfe München e.V.\",\"LastModified\":\"",
			            json.Substring(0, 127));

			Assert.Equal("\",\"TotalPages\":2}",
						json.Substring(146));
		}

		[Fact]
		public void TestGetPages()
		{
			DatabaseService db = DatabaseService.Connect(TestsFixture.ConnectionString);
			var controller = new AppController();

			var result = controller.GetPages(db, 1);
			Assert.IsType<JsonResult>(result);

			var json = JsonConvert.SerializeObject(((JsonResult)result).Value);

			Assert.Equal("[{\"Id\":1,\"workbookId\":1,\"Number\":1,\"Description\":\"Begrüßung, Vorstellung und Familie\"},{\"Id\":2,\"workbookId\":1,\"Number\":2,\"Description\":\"Wie geht es Ihnen?\"}]",
						json);

			result = controller.GetPages(db, 2);
			Assert.IsType<JsonResult>(result);

			json = JsonConvert.SerializeObject(((JsonResult)result).Value);

			Assert.Equal("[{\"Id\":3,\"workbookId\":2,\"Number\":1,\"Description\":\"Das Alphabet\"},{\"Id\":4,\"workbookId\":2,\"Number\":2,\"Description\":\"Erste Gespräche\"}]",
			            json);
		}

		[Fact]
		public async Task TestGetTeacherTrack()
		{
			var cl = new CloudLibrary(new StorageMock(), DatabaseService.Connect(TestsFixture.ConnectionString));
			var controller = new AppController();

			var result = await controller.GetTeacherTrack(cl, 1);
			Assert.IsType<JsonResult>(result);

			var json = JsonConvert.SerializeObject(((JsonResult)result).Value);

			Assert.Equal("{\"duration\":127000,\"creationTime\":\"",
			             json.Substring(0, 35));
			Assert.Equal("\",\"url\":\"abc\"}",
						 json.Substring(54));

			result = await controller.GetTeacherTrack(cl, 4);
			Assert.IsType<JsonResult>(result);

			json = JsonConvert.SerializeObject(((JsonResult)result).Value);

			Assert.Equal("{\"duration\":368000,\"creationTime\":\"",
						 json.Substring(0, 35));
			Assert.Equal("\",\"url\":\"abc\"}",
						 json.Substring(54));


		}
	}
}
