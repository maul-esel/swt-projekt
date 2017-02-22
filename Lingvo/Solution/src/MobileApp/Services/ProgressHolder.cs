using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Services.Progress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lingvo.MobileApp.Services
{
    /// <summary>
    /// This class is responsible for creating and deleting progress objects and (un-)registering of the corresponding actions to invoke.
    /// </summary>
    public class ProgressHolder
    {
        private static ProgressHolder instance;

        private Dictionary<double, PageProgress> pageProgresses;
        private Dictionary<double, WorkbookProgress> workbookProgresses;

        /// <summary>
        /// A dictionary mapping the actions to be invoked on a page id.
        /// </summary>
        internal Dictionary<double, Action<double>> PageListener
        {
            get; private set;
        }

        /// <summary>
        /// A dictionary mapping the actions to be invoked on a workbook id.
        /// </summary>
        internal Dictionary<double, Action<double>> WorkbookListener
        {
            get; private set;
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        public static ProgressHolder Instance => instance ?? (instance = new ProgressHolder());

        private ProgressHolder()
        {
            pageProgresses = new Dictionary<double, PageProgress>();
            workbookProgresses = new Dictionary<double, WorkbookProgress>();
            PageListener = new Dictionary<double, Action<double>>();
            WorkbookListener = new Dictionary<double, Action<double>>();
        }

        /// <summary>
        /// Creates a new <c>PageProgress</c> for a given <paramref name="page"/>.
        /// If no workbook progress for the workbook of the page exists, a new <c>WorkbookProgress</c> is created.
        /// This ensures workbook updates when its page progresses change.
        /// </summary>
        /// <param name="page">The <c>Page</c> for which a progress should be created.</param>
        /// <param name="token">The <c>CancellationTokenSource</c> for cancelling the progress.</param>
        internal void CreatePageProgress(IPage page, CancellationTokenSource token)
        {
            if (!pageProgresses.ContainsKey(page.Id))
            {
                PageProgress pageProgress = new PageProgress(page, token);

                if (!workbookProgresses.ContainsKey(page.workbookId))
                {
                    WorkbookProgress workbookProgress = new WorkbookProgress(page.Workbook, token);
                    workbookProgresses.Add(page.workbookId, workbookProgress);

                    LocalCollection.Instance.OnWorkbookChanged(page.Workbook);
                }

                workbookProgresses[page.workbookId].RegisterPageProgress(pageProgress);

                pageProgress.Cancelled += (p) => DeletePageProgress(p.Page);

                pageProgresses.Add(page.Id, pageProgress);
                LocalCollection.Instance.OnPageChanged(page);
            }
        }

        /// <summary>
        /// Creates a <c>PageProgress</c> via <see cref="CreatePageProgress(IPage, CancellationTokenSource)"/>.
        /// Additionally, marks it as explicit sub-progress of a <c>WorkbookProgress</c>.
        /// </summary>
        /// <param name="page">The <c>Page</c> for which a progress should be created.</param>
        /// <param name="token">The <c>CancellationTokenSource</c> for cancelling the progress.</param>
        internal void CreateSubProgress(IPage page, CancellationTokenSource token)
        {
            CreatePageProgress(page, token);
            pageProgresses[page.Id].MarkAsSubProgress();
        }

        /// <summary>
        /// Deletes a <c>PageProgress</c> for a given page, if existent.
        /// Additionally, if the corresponding <c>WorkbookProgress</c> is removed if no additional corresponding <c>PageProgress</c> objects exist.
        /// </summary>
        /// <param name="page">The <c>Page</c> for which the progress should be deleted.</param>
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

        /// <summary>
        /// Sets the action to be invoked when a progress update for the given page is reported.
        /// </summary>
        /// <param name="page">The <c>Page</c> for which the action should be registered.</param>
        /// <param name="progress">The action which should be invoked on a progress update.</param>
        public void SetPageProgressListener(IPage page, Action<double> progress)
        {
            if (progress != null)
            {
                UnsetPageProgressListener(page);
                PageListener.Add(page.Id, progress);
            }
        }

        /// <summary>
        /// Removes the registered action which is invoked on a progress update for a given page.
        /// </summary>
        /// <param name="page">The <c>Page</c> for which the action should be removed.</param>
        public void UnsetPageProgressListener(IPage page)
        {
            PageListener.Remove(page.Id);
        }

        /// <summary>
        /// Sets the action to be invoked when a progress update for the given workbook is reported.
        /// </summary>
        /// <param name="workbook">The <c>Workbook</c> for which the action should be registered.</param>
        /// <param name="progress">The action which should be invoked on a progress update.</param>
        public void SetWorkbookProgressListener(Workbook workbook, Action<double> progress)
        {
            if (progress != null)
            {
                UnsetWorkbookProgressListener(workbook);
                WorkbookListener.Add(workbook.Id, progress);
            }
        }

        /// <summary>
        /// Removes the registered action which is invoked on a progress update for a given workbook.
        /// </summary>
        /// <param name="workbook">The <c>Workbook</c> for which the action should be removed.</param>
        public void UnsetWorkbookProgressListener(Workbook workbook)
        {
            WorkbookListener.Remove(workbook.Id);
        }

        /// <summary>
        /// Returns the <c>WorkbookProgress</c> for a given workbook id.
        /// </summary>
        /// <param name="workbookId">The id of the workbook.</param>
        /// <returns>The corresponding <c>WorkbookProgress</c>, if existent. Null otherwise.</returns>
        internal WorkbookProgress GetWorkbookProgress(int workbookId)
        {
            if (workbookProgresses.ContainsKey(workbookId))
            {
                return workbookProgresses[workbookId];
            }
            return null;
        }

        /// <summary>
        /// Indicates if a <c>WorkbookProgress</c> for a given workbook id exists.
        /// </summary>
        /// <param name="workbookId">The id of the workbook.</param>
        /// <returns><c>True</c> if a <c>WorkbookProgress</c> exists, <c>false</c> otherwise.</returns>
        public bool HasWorkbookProgress(int workbookId)
        {
            return GetWorkbookProgress(workbookId) != null;
        }

        /// <summary>
        /// Returns the <c>PageProgress</c> for a given page id.
        /// </summary>
        /// <param name="pageId">The id of the page.</param>
        /// <returns>The corresponding <c>PageProgress</c>, if existent. Null otherwise.</returns>
        internal PageProgress GetPageProgress(int pageId)
        {
            if (pageProgresses.ContainsKey(pageId))
            {
                return pageProgresses[pageId];
            }
            return null;
        }

        /// <summary>
        /// Indicates if a <c>PageProgress</c> for a given page id exists.
        /// </summary>
        /// <param name="pageId">The id of the page.</param>
        /// <returns><c>True</c> if a <c>PageProgress</c> exists, <c>false</c> otherwise.</returns>
        public bool HasPageProgress(int pageId)
        {
            return GetPageProgress(pageId) != null;
        }
    }
}
