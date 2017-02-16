using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lingvo.MobileApp.Services.Progress
{
    public class WorkbookProgress : Progress<double>
    {
        public double CurrentProgress
        {
            get; private set;
        }

        private List<PageProgress> currentPageProgresses;
        private Workbook workbook;

        public int PageProgressCount => currentPageProgresses.Count;

        public WorkbookProgress(Workbook workbook) : base()
        {
            this.workbook = workbook;

            currentPageProgresses = new List<PageProgress>();

            SubProgressChanged(null, 0);
        }

        protected override void OnReport(double value)
        {
            if (ProgressHolder.Instance.WorkbookListener.ContainsKey(workbook.Id))
            {
                ProgressHolder.Instance.WorkbookListener[workbook.Id].Invoke(value);
            }

            base.OnReport(value);
        }

        private void SubProgressChanged(object sender, double p)
        {
            Workbook localWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(workbook.Id));

            double finishedProgress = 0.0f;
            if (localWorkbook != null)
            {
                finishedProgress = (100.0f * localWorkbook.Pages.Count) / localWorkbook.TotalPages;
            }

            double oldProgress = CurrentProgress;
            CurrentProgress = finishedProgress + p / workbook.TotalPages;

            if ((int)(CurrentProgress) > (int)(oldProgress))
            {
                OnReport(CurrentProgress);
            }
        }

        public void RegisterPageProgress(PageProgress progress)
        {
            if (!currentPageProgresses.Contains(progress))
            {
                currentPageProgresses.Add(progress);
                progress.ProgressChanged += SubProgressChanged;
            }
        }

        public void UnregisterPageProgress(PageProgress progress)
        {
            if (currentPageProgresses.Contains(progress))
            {
                currentPageProgresses.Remove(progress);
                progress.ProgressChanged -= SubProgressChanged;
            }
        }
    }
}
