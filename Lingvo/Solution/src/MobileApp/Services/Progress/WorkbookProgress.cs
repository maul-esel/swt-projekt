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

        public event Action<WorkbookProgress> Cancelled;

        public List<PageProgress> CurrentPageProgresses
        {
            get; private set;
        }

        public Workbook Workbook
        {
            get; private set;
        }

        public int PageProgressCount => CurrentPageProgresses.Count;

        public WorkbookProgress(Workbook workbook) : base()
        {
            this.Workbook = workbook;

            CurrentPageProgresses = new List<PageProgress>();

            SubProgressChanged(null, 0);
        }

        protected override void OnReport(double value)
        {
            if (ProgressHolder.Instance.WorkbookListener.ContainsKey(Workbook.Id))
            {
                ProgressHolder.Instance.WorkbookListener[Workbook.Id].Invoke(value);
            }

            base.OnReport(value);
        }

        private void SubProgressChanged(object sender, double p)
        {
            Workbook localWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(Workbook.Id));

            double finishedProgress = 0.0f;
            if (localWorkbook != null)
            {
                finishedProgress = (100.0f * localWorkbook.Pages.Count) / localWorkbook.TotalPages;
            }

            double oldProgress = CurrentProgress;
            CurrentProgress = finishedProgress + p / Workbook.TotalPages;

            if ((int)(CurrentProgress) > (int)(oldProgress))
            {
                OnReport(CurrentProgress);
            }
        }

        private void SubProgressCancelled()
        {
            Cancelled?.Invoke(this);
        }

        public void Cancel()
        {
            Cancelled?.Invoke(this);
        }

        public void RegisterPageProgress(PageProgress progress)
        {
            if (!CurrentPageProgresses.Contains(progress))
            {
                CurrentPageProgresses.Add(progress);
                progress.ProgressChanged += SubProgressChanged;
                progress.Cancelled += SubProgressCancelled;
            }
        }

        public void UnregisterPageProgress(PageProgress progress)
        {
            if (CurrentPageProgresses.Contains(progress))
            {
                CurrentPageProgresses.Remove(progress);
                progress.ProgressChanged -= SubProgressChanged;
                progress.Cancelled -= SubProgressCancelled;
            }
        }
    }
}
