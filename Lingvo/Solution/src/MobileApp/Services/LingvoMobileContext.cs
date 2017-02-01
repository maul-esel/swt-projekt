﻿using System;
using SQLite.Net;
using Lingvo.MobileApp.Entities;
using Lingvo.Common.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SQLiteNetExtensions.Extensions;
using Lingvo.Common.Services;
using System.IO;

#if __ANDROID__
		using SQLitePlatform = SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid;
#elif __IOS__
using SQLitePlatform = SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS;
#endif

namespace Lingvo.MobileApp.Services
{
    public class LingvoMobileContext
    {
        readonly SQLiteConnection database;

        public IEnumerable<Workbook> Workbooks => database.Table<Workbook>();
        public IEnumerable<Recording> Recordings => database.Table<Recording>();
        public IEnumerable<Page> Pages => database.Table<Page>();
        public IEnumerable<TeacherMemo> TeacherMemos => database.Table<TeacherMemo>();

        public event Action<Workbook> WorkbookChanged;
        public event Action<TeacherMemo> TeacherMemoChanged;
        public event Action<Page> PageChanged;
        public event Action<Recording> RecordingChanged;

        public LingvoMobileContext(string dbPath)
        {
            database = new SQLiteConnection(new SQLitePlatform(), dbPath);
        }

        public void createTables()
        {
            database.CreateTable<Workbook>();
            database.CreateTable<Recording>();
            database.CreateTable<Page>();
            database.CreateTable<TeacherMemo>();

        }

        public IEnumerable<Workbook> getWorkbooksWithReferences()
        {
            var val = new List<Workbook>();
            foreach (var w in Workbooks)
            {
                setWorkbookPages(w);
                val.Add(w);
            }
            return val;
        }

        public Workbook getWorkbookWithReferences(int workbookId)
        {
            var result = database.Query<Workbook>("select * from Workbook where Id = ?", workbookId);

            if (result == null || result.Count == 0)
            {
                return null;
            }
            var w = result.First();

            setWorkbookPages(w);

            return w;

        }

        private void setWorkbookPages(Workbook w)
        {
            var pages = database.Query<Page>("select * from Pages where workbookId = ?", w.Id);
            var pagesWithReferences = new List<Page>();

            foreach (var p in pages)
            {
                if (p.teacherTrackId > 0)
                {
                    p.TeacherTrack = FindRecording(p.teacherTrackId);
                }

                if (p.studentTrackId != null)
                {
                    p.StudentTrack = FindRecording(p.studentTrackId.Value);
                }
                p.Workbook = w;
                pagesWithReferences.Add(p);
            }

            w.Pages = pagesWithReferences.Cast<IPage>().ToList();
        }

        public IEnumerable<TeacherMemo> getTeacherMemosWithReferences()
        {
            var val = new List<TeacherMemo>();
            foreach (var t in TeacherMemos)
            {
                if (t.RecordingId > 0)
                {
                    t.Recording = FindRecording(t.RecordingId);
                }

                if (t.StudentTrackId != null)
                {
                    t.StudentTrack = FindRecording(t.StudentTrackId.Value);
                }

                val.Add(t);
            }
            return val;
        }

        public TeacherMemo getTeacherMemoWithReferences(int teacherMemoId)
        {
            var t = FindTeacherMemo(teacherMemoId);

            if (t == null)
            {
                return null;
            }

            if (t.RecordingId > 0)
            {
                t.Recording = FindRecording(t.RecordingId);
            }

            if (t.StudentTrackId != null)
            {
                t.StudentTrack = FindRecording(t.StudentTrackId.Value);
            }

            return t;
        }

        public void Save(Recording recording)
        {
            database.InsertOrReplace(recording);

            RecordingChanged?.Invoke(recording);
        }

        /// <summary>
        /// Save the specified page, updates it if it already exists.
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="page">Page.</param>
        public void Save(Page page)
        {
            database.InsertOrReplace(page);
            database.UpdateWithChildren(page);

            PageChanged?.Invoke(page);
        }

        /// <summary>
        /// Save the specified workbook, updates it if it already exists.
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="workbook">Workbook.</param>
        public void Save(Workbook workbook)
        {
            database.InsertOrReplace(workbook);
            database.UpdateWithChildren(workbook);

            WorkbookChanged?.Invoke(workbook);
        }

        /// <summary>
        /// Save the specified memo.
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="memo">Memo.</param>
        public void Save(TeacherMemo memo)
        {
            database.InsertOrReplace(memo);
            database.UpdateWithChildren(memo);

            TeacherMemoChanged?.Invoke(memo);
        }

        public void Delete(TeacherMemo memo)
        {
            database.Delete(memo);

            Delete(memo.Recording);

            if (memo.StudentTrack != null)
            {
                Delete(memo.StudentTrack);
            }

            TeacherMemoChanged?.Invoke(memo);
        }

        public void Delete(Recording recording)
        {
            database.Delete(recording);

            File.Delete(FileUtil.getAbsolutePath(recording.LocalPath));

            RecordingChanged?.Invoke(recording);
        }

        public void Delete(Page page)
        {
            Delete(page.TeacherTrack);

            if (page.StudentTrack != null)
            {
                Delete(page.StudentTrack);
            }

            database.Delete(page);

            PageChanged?.Invoke(page);
        }

        public void Delete(Workbook workbook)
        {
            workbook.Pages.ForEach(p => Delete((Page)p));

            database.Delete(workbook); //On delete cascade used for pages

            WorkbookChanged?.Invoke(workbook);
        }

        public Recording FindRecording(int id)
        {
            return Recordings.FirstOrDefault(r => r.Id == id);
        }

        /// <summary>
        /// Important: does not load pages! Use getWorkbookWithReferences instead if you need them.
        /// </summary>
        /// <returns>The workbook.</returns>
        /// <param name="id">Identifier.</param>
        public Workbook FindWorkbook(int id)
        {
            return Workbooks.FirstOrDefault(w => w.Id == id);
        }

        /// <summary>
        /// Important: does not load teacher track and student track!
        /// </summary>
        /// <returns>The page.</returns>
        /// <param name="id">Identifier.</param>
        public Page FindPage(int id)
        {
            return Pages.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Important: does not load recording and student track! Use getTeacherMemoWithReferences instead if you need them.
        /// </summary>
        /// <returns>The teacher memo.</returns>
        /// <param name="id">Identifier.</param>
        public TeacherMemo FindTeacherMemo(int id)
        {
            return TeacherMemos.FirstOrDefault(t => t.Id == id);
        }
    }
}
