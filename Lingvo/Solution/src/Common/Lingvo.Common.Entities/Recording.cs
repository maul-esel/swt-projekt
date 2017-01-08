using System;
using LinqToDB.Mapping;

namespace Lingvo.Common
{
	[Table("Recordings")]
	public class Recording
	{
		private Recording() { } // used by Linq2DB

		public Recording(int id, TimeSpan length, string localPath)
		{
			Id = id;
			Length = length;
			LocalPath = localPath;
		}

		[Column, PrimaryKey]
		public int Id { get; private set; }

		[Column, NotNull]
		public DateTime CreationTime { get; private set; }

		[Column, NotNull]
		public TimeSpan Length { get; private set; }

		[Column, NotNull]
		public string LocalPath { get; private set; }
	}
}
