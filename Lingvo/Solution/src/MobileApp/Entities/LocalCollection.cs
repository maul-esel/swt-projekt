using System;
using System.Collections.Generic;
using Lingvo.Common.Entities;

namespace Lingvo.MobileApp.Entities
{
    public class LocalCollection
	{
		private static LocalCollection instance;

		/// <summary>
		/// The teacher memos collection, does not return null but an empty list.
		/// </summary>
		/// <value>The teacher memos.</value>
		public IEnumerable<TeacherMemo> TeacherMemos => App.Database.TeacherMemos;

		/// <summary>
		/// The workbooks, does not return null but an empty list.
		/// </summary>
		/// <value>The workbooks.</value>
		public IEnumerable<Workbook> Workbooks 
		{ 
			get
			{
				return App.Database.getWorkbooksWithReferences();
			}
		}


		private LocalCollection()
		{
            
        }

		/// <summary>
		/// Gets the instance of local collection (singleton pattern).
		/// </summary>
		/// <returns>The instance.</returns>
		public static LocalCollection GetInstance()
		{
			if (instance == null)
			{
				instance = new LocalCollection();
			}

			return instance;
		}

		/// <summary>
		/// Adds a teacher memo to the collection.
		/// </summary>
		/// <param name="memo">Memo.</param>
		public void AddTeacherMemo(TeacherMemo memo)
		{
			App.Database.Save(memo);
		}

		/// <summary>
		/// Adds a workbook to the collection.
		/// </summary>
		/// <param name="workbook">Workbook.</param>
		public void AddWorkbook(Workbook workbook)
		{
			App.Database.Save(workbook);
		}

		/// <summary>
		/// Deletes the workbook.
		/// </summary>
		/// <param name="workbook">Workbook.</param>
		public void DeleteWorkbook(Workbook workbook)
		{
			App.Database.Delete(workbook);
		}

		/// <summary>
		/// Deletes the teacher memo.
		/// </summary>
		/// <param name="memo">Memo.</param>
		public void DeleteTeacherMemo(TeacherMemo memo)
		{
			App.Database.Delete(memo);
		}
	}
}
