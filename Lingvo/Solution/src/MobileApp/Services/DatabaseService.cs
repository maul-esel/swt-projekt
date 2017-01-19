using LinqToDB.DataProvider.SQLite;
using LinqToDB;
using Lingvo.Common.Entities;
using Lingvo.Common.Services;
using Lingvo.MobileApp.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Lingvo.MobileApp.Services
{
	public class DatabaseService : Common.Services.DatabaseService
	{
		public DatabaseService(string connectionString)
			: base(new SQLiteDataProvider(), connectionString) { }
	
	
		public ITable<TeacherMemo> TeacherMemos => connection.GetTable<TeacherMemo>()
                                     .LoadWith(p => p.Recording);

		/// <summary>
		/// Save the specified recording, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="recording">Recording.</param>
		public void Save(Recording recording)
		{

			if (Recordings.Find(recording.Id) != null)
			{
				connection.InsertOrReplace(recording);
			}
			else
			{
				connection.InsertWithIdentity(recording);
			}

		}

		/// <summary>
		/// Save the specified page, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="page">Page.</param>
		public void Save(Page page)
		{
			
			connection.InsertOrReplace(page);

		}

		/// <summary>
		/// Save the specified workbook, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="workbook">Workbook.</param>
		public void Save(Workbook workbook)
		{

			connection.InsertOrReplace(workbook);

		}

		/// <summary>
		/// Save the specified memo.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="memo">Memo.</param>
		public void Save(TeacherMemo memo)
		{
			Save(memo.Recording);

			if (TeacherMemos.Find(memo.Id) != null)
			{
				connection.InsertOrReplace(memo);
			}
			else
			{
				int id = Convert.ToInt32((UInt64)connection.InsertWithIdentity(memo));
				memo.Id = id;
			}

		}

		public void Delete(TeacherMemo memo)
		{
			Delete(memo.Recording);
			connection.Delete(memo);
		}
	}

	public static class DatabaseExtensions
	{
		public static TeacherMemo Find(this IEnumerable<TeacherMemo> teacherMemos, int memoId)
		{
			return teacherMemos.FirstOrDefault(memo => memo.Id == memoId);
		}
	}
}
