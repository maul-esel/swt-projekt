using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lingvo.MobileApp.Services.Progress
{
	/// <summary>
	/// Represents the download progress of an entire workbook (with all its pages)
	/// and notifies appropriate listeners.
	/// </summary>
    public class WorkbookProgress : Progress<double>
	{
		/// <summary>
		/// The current progress in percent.
		/// </summary>
		public double CurrentProgress
        {
            get; private set;
        }

		/// <summary>
		/// The <see cref="CancellationTokenSource"/> used to cancel the download.
		/// </summary>
		public CancellationTokenSource CancellationToken
        {
            get; set;
        }

		/// <summary>
		/// The <see cref="PageProgress"/> instances representing the download progress of <see cref="Workbook"/>.
		/// </summary>
        public List<PageProgress> CurrentPageProgresses
        {
            get; private set;
        }

		/// <summary>
		/// The <see cref="Workbook"/> whose download progress is represented by this instance.
		/// </summary>
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

		/// <summary>
		/// Called when the download is cancelled. Cancels all page downloads.
		/// </summary>
        private void OnCancel()
        {
            for (int idx = CurrentPageProgresses.Count - 1; idx >= 0; idx--)
            {
                CurrentPageProgresses[idx].CancellationToken.Cancel();
            }
        }

		/// <summary>
		/// Notifies the appropriate listeners (as indicated by <see cref="ProgressHolder"/>) of download progress.
		/// </summary>
		/// <param name="value">The current download progress, in percent</param>
        protected override void OnReport(double value)
        {
            if (ProgressHolder.Instance.WorkbookListener.ContainsKey(Workbook.Id))
            {
                ProgressHolder.Instance.WorkbookListener[Workbook.Id].Invoke(value);
            }

            base.OnReport(value);
        }

		/// <summary>
		/// Called when the download progress of a page in <see cref="Workbook"/> changes.
		/// Updates this instances download progress value.
		/// </summary>
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

		/// <summary>
		/// Registers a download progress for a page contained in <see cref="Workbook"/>.
		/// </summary>
        public void RegisterPageProgress(PageProgress progress)
        {
            if (!CurrentPageProgresses.Contains(progress))
            {
                CurrentPageProgresses.Add(progress);
                progress.ProgressChanged += SubProgressChanged;
            }
        }

		/// <summary>
		/// Unregisters a download progress for a page contained in <see cref="Workbook"/>.
		/// </summary>
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
