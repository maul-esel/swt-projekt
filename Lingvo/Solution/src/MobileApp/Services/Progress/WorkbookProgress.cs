using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lingvo.MobileApp.Services.Progress
{
    public class WorkbookProgress : Progress<double>
    {
        public double CurrentProgress
        {
            get; private set;
        }

        public CancellationTokenSource CancellationToken
        {
            get; set;
        }

        public List<PageProgress> CurrentPageProgresses
        {
            get; private set;
        }

        public Workbook Workbook
        {
            get; private set;
        }

        public int PageProgressCount => CurrentPageProgresses.Count;

        public WorkbookProgress(Workbook workbook, CancellationTokenSource token) : base()
        {
            Workbook = workbook;

            CancellationToken = token;
            CancellationToken.Token.Register(OnCancel);

            CurrentPageProgresses = new List<PageProgress>();

            SubProgressChanged(null, 0);
        }

        private void OnCancel()
        {
            for (int idx = CurrentPageProgresses.Count - 1; idx >= 0; idx--)
            {
                CurrentPageProgresses[idx].CancellationToken.Cancel();
            }
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

        public void RegisterPageProgress(PageProgress progress)
        {
            if (!CurrentPageProgresses.Contains(progress))
            {
                CurrentPageProgresses.Add(progress);
                progress.ProgressChanged += SubProgressChanged;
            }
        }

        public void UnregisterPageProgress(PageProgress progress)
        {
            if (CurrentPageProgresses.Contains(progress))
            {
                CurrentPageProgresses.Remove(progress);
                progress.ProgressChanged -= SubProgressChanged;
            }
        }
    }
}
