using System;
using System.IO;
using System.Linq;

using Lingvo.Common.Entities;

using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using Lingvo.Backend.Controllers;

namespace Lingvo.Backend.Tests
{
	public class HomeControllerTests
	{
		public HomeControllerTests()
		{
			DatabaseService Database = DatabaseService.Connect(TestsFixture.ConnectionString);
		}


		[Fact]
		public void TestLoadBackend()
		{
			var controller = new HomeController(null);

			var result = controller.Index(DatabaseService.Connect(TestsFixture.ConnectionString));

			Assert.IsType<ViewResult>(result);
		}
	}
}
