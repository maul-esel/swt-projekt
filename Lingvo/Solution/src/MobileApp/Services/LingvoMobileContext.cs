using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Lingvo.Common.Services;

namespace Lingvo.MobileApp
{
	public class LingvoMobileContext: LingvoContext
	{
		public LingvoMobileContext(string dbPath) : base(buildOptions(dbPath))
		{
		}

		private static DbContextOptions<LingvoContext> buildOptions(string dbPath)
		{
			var optionsBuilder = new DbContextOptionsBuilder<LingvoContext>();
			optionsBuilder.UseSqlite("Filename=" + dbPath);

			return optionsBuilder.Options;
		}
	}
}
