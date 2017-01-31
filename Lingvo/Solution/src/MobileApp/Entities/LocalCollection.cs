using System;
using System.Collections.Generic;
using Lingvo.Common.Entities;
using System.IO;

namespace Lingvo.MobileApp.Entities
{
    public class LocalCollection
    {
        public event Action<Workbook> WorkbookChanged
        {
            add { App.Database.WorkbookChanged += value; }
            remove { App.Database.WorkbookChanged -= value; }
        }
        public event Action<TeacherMemo> TeacherMemoChanged
        {
            add { App.Database.TeacherMemoChanged += value; }
            remove { App.Database.TeacherMemoChanged -= value; }
        }
        public event Action<Page> PageChanged
        {
            add { App.Database.PageChanged += value; }
            remove { App.Database.PageChanged -= value; }
        }

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

        /// <summary>
		/// Deletes a page.
		/// </summary>
		/// <param name="page">Page.</param>
        [Obsolete]
        public void DeletePage(Page page)
        {
            page.Workbook.DeletePage(page);
            App.Database.Delete(page);
        }

        /// <summary>
		/// Deletes a StudentRecording of the given page.
		/// </summary>
		/// <param name="page">Page.</param>
        [Obsolete]
        public void DeleteStudentRecording(Page page)
        {
            //File.Delete(FileUtils.getAbsolutePath(page.StudentTrack.LocalPath));
            Page local = App.Database.FindPage(page.Id);
            File.Delete(local.StudentTrack.LocalPath);
            App.Database.Delete(local.StudentTrack);
            page.DeleteStudentRecording();
            App.Database.Save(local);
        }
    }
}
