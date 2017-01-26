using System;
using SQLite;
using Lingvo.MobileApp.Entities;
using Lingvo.Common.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Lingvo.MobileApp.Services
{
	public class LingvoMobileContext
	{
		readonly SQLiteConnection database;

		public IEnumerable<Workbook> 	Workbooks => database.Table<Workbook>();
		public IEnumerable<Recording> 	Recordings => database.Table<Recording>();
		public IEnumerable<Page> 		Pages => database.Table<Page>();

		public IEnumerable<TeacherMemo>	TeacherMemos => database.Table<TeacherMemo>();

		public LingvoMobileContext(string dbPath)
		{
			database = new SQLiteConnection(dbPath);
		}

		public void createTables(string sql)
		{
			var sqls = sql.Split(new[] { "\n\n" }, StringSplitOptions.None);

			foreach (var statement in sqls)
			{
				database.Execute(statement);
			}

		}	

		public void Save(Recording recording)
		{
			database.InsertOrReplace(recording);
		}

		/// <summary>
		/// Save the specified page, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="page">Page.</param>
		public void Save(Page page)
		{
			database.InsertOrReplace(page);
		}

		/// <summary>
		/// Save the specified workbook, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="workbook">Workbook.</param>
		public void Save(Workbook workbook)
		{
			database.InsertOrReplace(workbook);
		}

		/// <summary>
		/// Save the specified memo.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="memo">Memo.</param>
		public void Save(TeacherMemo memo)
		{
			var returnedKey = database.InsertOrReplace(memo);
			if ( returnedKey > 0)
			{
				memo.Id = returnedKey;
			}
		}

		public void Delete(TeacherMemo memo)
		{
			database.Delete(memo);
		}

		public void Delete(Recording recording)
		{
			database.Delete(recording);
		}

		public void Delete(Page page)
		{
			Delete(page.TeacherTrack);

			if (page.StudentTrack != null)
			{
				Delete(page.StudentTrack);
			}

			database.Delete(page);

		}

		public void Delete(Workbook workbook)
		{
			database.Delete(workbook); //On delete cascade used for pages
		}

		public Recording FindRecording(int id)
		{
			return Recordings.FirstOrDefault(r => r.Id == id);
		}

		public Workbook FindWorkbook(int id)
		{
			return Workbooks.FirstOrDefault(w => w.Id == id);
		}

		public Page FindPage(int id)
		{
			return Pages.FirstOrDefault(p => p.Id == id);
		}

		public TeacherMemo FindTeacherMemo(int id)
		{
			return TeacherMemos.FirstOrDefault(t => t.Id == id);
		}
	}
}
