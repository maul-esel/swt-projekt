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
    public class DatabaseService
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

        public DatabaseService(string dbPath)
        {
            database = new SQLiteConnection(new SQLitePlatform(), dbPath, false);
        }

        public void createTables()
        {
            database.CreateTable<Workbook>();
            database.CreateTable<Recording>();
            database.CreateTable<Page>();
            database.CreateTable<TeacherMemo>();

        }

        /// <summary>
        /// Finds the recording by id.
        /// </summary>
        /// <returns>The recording.</returns>
        /// <param name="id">Identifier.</param>
        public Recording FindRecording(int id)
        {
            return Recordings.FirstOrDefault(r => r.Id == id);
        }

        /// <summary>
        /// Gets all the workbooks with all references: all associated pages and for all pages the existing recordings 
        /// </summary>
        /// <returns>The workbooks with references.</returns>
        public IEnumerable<Workbook> FindWorkbooks()
        {
            var val = new List<Workbook>();
            foreach (var w in Workbooks)
            {
                setWorkbookPages(w);
                val.Add(w);
            }
            return val;
        }

        /// <summary>
        /// Gets the workbook with all references: all associated pages and for all pages the existing recordings 
        /// </summary>
        /// <returns>The workbook with references.</returns>
        /// <param name="workbookId">Workbook identifier.</param>
        public Workbook FindWorkbook(int workbookId)
        {
            var result = database.Query<Workbook>("select * from Workbooks where Id = ?", workbookId);

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

        /// <summary>
        /// Gets all the teacher memos with references (all existing recordings)
        /// </summary>
        /// <returns>The teacher memos with references.</returns>
        public IEnumerable<TeacherMemo> FindTeacherMemos()
        {
            var val = new List<TeacherMemo>();
            foreach (var t in TeacherMemos)
            {
                GetTeacherMemosRecordings(t);

                val.Add(t);
            }
            return val;
        }

        /// <summary>
        /// Gets the teacher memo with references (all existing recordings)
        /// </summary>
        /// <returns>The teacher memo with references.</returns>
        /// <param name="teacherMemoId">Teacher memo identifier.</param>
        public TeacherMemo FindTeacherMemo(int teacherMemoId)
        {
            var t = TeacherMemos.FirstOrDefault(m => m.Id == teacherMemoId);

            if (t == null)
            {
                return null;
            }

            GetTeacherMemosRecordings(t);

            return t;
        }

        /// <summary>
        /// Gets the page with references (all existing recordings)
        /// </summary>
        /// <returns>The page with references.</returns>
        /// <param name="pageId">Page identifier.</param>
        public Page FindPage(int pageId)
        {
            var p = Pages.FirstOrDefault(pp => pp.Id == pageId);

            if (p == null)
            {
                return null;
            }

            if (p.teacherTrackId > 0)
            {
                p.TeacherTrack = FindRecording(p.teacherTrackId);
            }

            if (p.studentTrackId != null)
            {
                p.StudentTrack = FindRecording(p.studentTrackId.Value);
            }

            return p;
        }

        private void GetTeacherMemosRecordings(TeacherMemo t)
        {
            if (t.RecordingId > 0)
            {
                t.TeacherTrack = FindRecording(t.RecordingId);
            }

            if (t.StudentTrackId != null)
            {
                t.StudentTrack = FindRecording(t.StudentTrackId.Value);
            }
        }

        /// <summary>
        /// Save the specified recording.
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="recording">Recording.</param>

        public void Save(Recording recording)
        {
            if (recording.Id > 0 && FindRecording(recording.Id) != null)
            {
                database.Update(recording);
            }
            else
            {
                database.Insert(recording);
            }

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
            WorkbookChanged?.Invoke(page.Workbook);
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
        /// Save the specified memo 
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="memo">Memo.</param>
        public void Save(TeacherMemo memo)
        {
            if (memo.StudentTrack != null)
            {
                memo.StudentTrackId = memo.StudentTrack.Id;
            }

            if (memo.Id > 0 && TeacherMemos.FirstOrDefault(t => t.Id == memo.Id) != null)
            {
                database.Update(memo);
            }
            else
            {
                database.Insert(memo);
            }

            TeacherMemoChanged?.Invoke(memo);
        }

        /// <summary>
        /// Delete the specified memo and its recordings.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="memo">Memo.</param>
        public void Delete(TeacherMemo memo)
        {
            database.Delete(memo);

            Delete(memo.TeacherTrack);

            if (memo.StudentTrack != null)
            {
                Delete(memo.StudentTrack);
            }

            TeacherMemoChanged?.Invoke(memo);
        }

        /// <summary>
        /// Delete the specified recording.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="recording">Recording.</param>
        public void Delete(Recording recording)
        {
            database.Delete(recording);

            Task.Run(() => File.Delete(FileUtil.getAbsolutePath(recording.LocalPath)));

            RecordingChanged?.Invoke(recording);
        }

        /// <summary>
        /// Delete the specified page and its recordings.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="page">Page.</param>
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

        /// <summary>
        /// Delete the specified workbook and on cascade all its pages.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="workbook">Workbook.</param>
        public void Delete(Workbook workbook)
        {
            workbook.Pages.ForEach(p => Delete((Page)p));

            database.Delete(workbook); //On delete cascade used for pages

            WorkbookChanged?.Invoke(workbook);
        }
    }
}