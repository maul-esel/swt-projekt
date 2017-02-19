using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Services.Progress;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lingvo.MobileApp.Services
{
    public class ProgressHolder
    {
        private static ProgressHolder instance;

        private Dictionary<double, PageProgress> pageProgresses;
        private Dictionary<double, WorkbookProgress> workbookProgresses;

        internal Dictionary<double, Action<double>> PageListener
        {
            get; private set;
        }

        internal Dictionary<double, Action<double>> WorkbookListener
        {
            get; private set;
        }

        public static ProgressHolder Instance => instance ?? (instance = new ProgressHolder());

        private ProgressHolder()
        {
            pageProgresses = new Dictionary<double, PageProgress>();
            workbookProgresses = new Dictionary<double, WorkbookProgress>();
            PageListener = new Dictionary<double, Action<double>>();
            WorkbookListener = new Dictionary<double, Action<double>>();
        }

        internal void CreatePageProgress(IPage page)
        {
            if (!pageProgresses.ContainsKey(page.Id))
            {
                PageProgress pageProgress = new PageProgress(page);

                if (!workbookProgresses.ContainsKey(page.workbookId))
                {
                    WorkbookProgress workbookProgress = new WorkbookProgress(page.Workbook);
                    workbookProgresses.Add(page.workbookId, workbookProgress);
                    workbookProgress.Cancelled += OnWorkbookProgressCancelled;

                    LocalCollection.Instance.OnWorkbookChanged(page.Workbook);
                }

                workbookProgresses[page.workbookId].RegisterPageProgress(pageProgress);

                pageProgresses.Add(page.Id, pageProgress);
                LocalCollection.Instance.OnPageChanged(page);
            }
        }

        private void OnWorkbookProgressCancelled(WorkbookProgress progress)
        {
            for (int idx = progress.CurrentPageProgresses.Count - 1; idx >= 0; idx--)
            {
                DeletePageProgress(progress.CurrentPageProgresses[idx].Page);
            }

            LocalCollection.Instance.OnWorkbookChanged(progress.Workbook);
        }

        internal void DeletePageProgress(IPage page)
        {
            if (pageProgresses.ContainsKey(page.Id))
            {
                WorkbookProgress workbookProgress = workbookProgresses[page.workbookId];

                workbookProgress.UnregisterPageProgress(pageProgresses[page.Id]);

                pageProgresses.Remove(page.Id);

                if (workbookProgress.PageProgressCount == 0)
                {
                    workbookProgresses.Remove(page.workbookId);
                }
            }
        }

        public void SetPageProgressListener(IPage page, Action<double> progress)
        {
            if (progress != null)
            {
                UnsetPageProgressListener(page);
                PageListener.Add(page.Id, progress);
            }
        }

        public void UnsetPageProgressListener(IPage page)
        {
            PageListener.Remove(page.Id);
        }

        public void SetWorkbookProgressListener(Workbook workbook, Action<double> progress)
        {
            if (progress != null)
            {
                UnsetWorkbookProgressListener(workbook);
                WorkbookListener.Add(workbook.Id, progress);
            }
        }

        public void UnsetWorkbookProgressListener(Workbook workbook)
        {
            WorkbookListener.Remove(workbook.Id);
        }

        internal WorkbookProgress GetWorkbookProgress(int workbookId)
        {
            if (workbookProgresses.ContainsKey(workbookId))
            {
                return workbookProgresses[workbookId];
            }
            return null;
        }

        public bool HasWorkbookProgress(int workbookId)
        {
            return GetWorkbookProgress(workbookId) != null;
        }

        internal PageProgress GetPageProgress(int pageId)
        {
            if (pageProgresses.ContainsKey(pageId))
            {
                return pageProgresses[pageId];
            }
            return null;
        }

        public bool HasPageProgress(int pageId)
        {
            return GetPageProgress(pageId) != null;
        }
    }
}
