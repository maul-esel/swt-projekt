using Lingvo.Common.Entities;
using System;
using System.Collections.Generic;

namespace MobileApp.Entities
{
    public class LocalCollection
	{
		private static LocalCollection instance;

		private List<TeacherMemo> teacherMemos;
		private List<Workbook> workbooks;

		/// <summary>
		/// Gets or sets the teacher memos collection, does not return null but an empty list.
		/// </summary>
		/// <value>The teacher memos.</value>
		public List<TeacherMemo> TeacherMemos
		{
			get
			{
				//Nullpointer avoidance
				if (teacherMemos == null)
				{
					teacherMemos = new List<TeacherMemo>();
				}

				return teacherMemos;
			}

			set
			{
				teacherMemos = value;
			}
		}

		/// <summary>
		/// Gets or sets the workbooks, does not return null but an empty list.
		/// </summary>
		/// <value>The workbooks.</value>
		public List<Workbook> Workbooks
		{
			get
			{
				//Nullpointer avoidance
				if (workbooks == null)
				{
					workbooks = new List<Workbook>();
				}

				return workbooks;
			}

			set
			{
				workbooks = value;
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
			if (teacherMemos == null)
			{
				teacherMemos = new List<TeacherMemo>();
			}

			teacherMemos.Add(memo);
		}

		/// <summary>
		/// Adds a workbook to the collection.
		/// </summary>
		/// <param name="workbook">Workbook.</param>
		public void AddWorkbook(Workbook workbook)
		{
			if (workbooks == null)
			{
				workbooks = new List<Workbook>();
			}

			workbooks.Add(workbook);
		}

		/// <summary>
		/// Deletes the workbook with the given id.
		/// </summary>
		/// <param name="workbookID">Workbook identifier.</param>
		public void DeleteWorkbook(String workbookID)
		{
			Workbook toBeDeleted = null;

			if (workbooks != null)
			{
				foreach (var workbook in workbooks)
				{
					if (workbook.Id.Equals(workbookID))
					{
						toBeDeleted = workbook;
						break;
					}
				}

				if (toBeDeleted != null)
				{
					workbooks.Remove(toBeDeleted);
				}
			}
		}

		/// <summary>
		/// Deletes the teacher memo.
		/// </summary>
		/// <param name="memo">Memo.</param>
		public void DeleteTeacherMemo(TeacherMemo memo)
		{
			if (teacherMemos != null)
			{
				teacherMemos.Remove(memo);
			}
		}
	}
}
