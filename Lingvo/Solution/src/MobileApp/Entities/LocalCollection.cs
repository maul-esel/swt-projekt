using System;
using System.Collections.Generic;
using Lingvo.Common.Entities;

namespace Lingvo.MobileApp.Entities
{
    public class LocalCollection
	{
        public delegate void OnWorkbooksChanged();
        public delegate void OnTeacherMemosChanged();

        public event OnWorkbooksChanged WorkbooksChanged;
        public event OnTeacherMemosChanged TeacherMemosChanged;

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
         public static LocalCollection Instance => instance ?? (instance = new LocalCollection());


        /// <summary>
        /// Adds a teacher memo to the collection.
        /// </summary>
        /// <param name="memo">Memo.</param>
        public void AddTeacherMemo(TeacherMemo memo)
		{
			App.Database.Save(memo);

            TeacherMemosChanged?.Invoke();
		}

		/// <summary>
		/// Adds a workbook to the collection.
		/// </summary>
		/// <param name="workbook">Workbook.</param>
		public void AddWorkbook(Workbook workbook)
		{
			App.Database.Save(workbook);

            WorkbooksChanged?.Invoke();
		}

		/// <summary>
		/// Deletes the workbook.
		/// </summary>
		/// <param name="workbook">Workbook.</param>
		public void DeleteWorkbook(Workbook workbook)
		{
			App.Database.Delete(workbook);

            WorkbooksChanged?.Invoke();
		}

		/// <summary>
		/// Deletes the teacher memo.
		/// </summary>
		/// <param name="memo">Memo.</param>
		public void DeleteTeacherMemo(TeacherMemo memo)
		{
			App.Database.Delete(memo);

            TeacherMemosChanged?.Invoke();
		}

        public void OnWorkbookChanged()
        {
            WorkbooksChanged?.Invoke();
        }
	}
}
