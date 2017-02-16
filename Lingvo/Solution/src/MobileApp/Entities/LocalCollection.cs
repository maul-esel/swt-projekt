using System;
using System.Collections.Generic;
using Lingvo.Common.Entities;
using System.IO;
using Lingvo.Common.Services;

namespace Lingvo.MobileApp.Entities
{
    public class LocalCollection
    {
        public event Action<Workbook> WorkbookChanged;

        public event Action<TeacherMemo> TeacherMemoChanged;

        public event Action<IPage> PageChanged;

        private static LocalCollection instance;

        /// <summary>
        /// The teacher memos collection, does not return null but an empty list.
        /// </summary>
        /// <value>The teacher memos.</value>
        public IEnumerable<TeacherMemo> TeacherMemos
        {
            get; private set;
        }

        /// <summary>
        /// The workbooks, does not return null but an empty list.
        /// </summary>
        /// <value>The workbooks.</value>
        public IEnumerable<Workbook> Workbooks
        {
            get; private set;
        }


        private LocalCollection()
        {
            Workbooks = App.Database.FindWorkbooks();
            TeacherMemos = App.Database.FindTeacherMemos();
            App.Database.WorkbookChanged += OnWorkbookChanged;
            App.Database.PageChanged += OnPageChanged;
            App.Database.TeacherMemoChanged += OnTeacherMemoChanged;
        }

        /// <summary>
        /// Gets the instance of local collection (singleton pattern).
        /// </summary>
        /// <returns>The instance.</returns>
        public static LocalCollection Instance => instance ?? (instance = new LocalCollection());

        public void OnTeacherMemoChanged(TeacherMemo m)
        {
            TeacherMemos = App.Database.FindTeacherMemos();
            TeacherMemoChanged?.Invoke(m);
        }

        public void OnWorkbookChanged(Workbook w)
        {
            Workbooks = App.Database.FindWorkbooks();
            WorkbookChanged?.Invoke(w);
        }

        public void OnPageChanged(IPage p)
        {
            Workbooks = App.Database.FindWorkbooks();
            PageChanged?.Invoke(p);
        }

        /// <summary>
        /// Adds a teacher memo to the collection.
        /// </summary>
        /// <param name="memo">Memo.</param>
        public void AddTeacherMemo(TeacherMemo memo)
        {
            if (memo.TeacherTrack != null && App.Database.FindRecording(memo.TeacherTrack.Id) == null)
            {
                App.Database.Save(memo.TeacherTrack);
            }

            if (memo.StudentTrack != null && App.Database.FindRecording(memo.StudentTrack.Id) == null)
            {
                App.Database.Save(memo.StudentTrack);
            }

            memo.RecordingId = memo.TeacherTrack.Id;

            if (memo.StudentTrack != null)
            {
                memo.StudentTrackId = memo.StudentTrack.Id;
            }

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

            if (page.Workbook.Pages.Count == 0)
            {
                DeleteWorkbook(page.Workbook);
            }
        }

        /// <summary>
		/// Deletes a StudentRecording of the given page.
		/// </summary>
		/// <param name="page">Page.</param>
        [Obsolete]
        public void DeleteStudentRecording(IExercise exercise)
        {
            File.Delete(FileUtil.getAbsolutePath(exercise.StudentTrack.LocalPath));
            App.Database.Delete(exercise.StudentTrack);
            exercise.DeleteStudentRecording();

            if (exercise is Page)
            {
                App.Database.Save((Page)exercise);
            }
            else
            {
                App.Database.Save((TeacherMemo)exercise);
            }
        }
    }
}
