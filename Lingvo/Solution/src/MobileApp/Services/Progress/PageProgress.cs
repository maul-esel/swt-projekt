using Lingvo.Common.Entities;
using System;
using System.Threading;

namespace Lingvo.MobileApp.Services.Progress
{
	/// <summary>
	/// Represents the progress when downloading a <see cref="Page"/>
	/// and notifies the appropriate listeners.
	/// </summary>
    public class PageProgress : Progress<double>
    {
		/// <summary>
		/// The current progress in percent.
		/// </summary>
        public double CurrentProgress
        {
            get; private set;
        }

		/// <summary>
		/// Raised when the download is cancelled.
		/// </summary>
        public event Action<PageProgress> Cancelled;

		/// <summary>
		/// The page whose download progress is represented by this instance.
		/// </summary>
        internal IPage Page
        {
            get; private set;
        }

		/// <summary>
		/// The <see cref="CancellationTokenSource"/> used to cancel the download.
		/// </summary>
        public CancellationTokenSource CancellationToken
        {
            get; private set;
        }

		/// <summary>
		/// Set to <c>true</c> if <see cref="Page"/> is downloaded as part of an entire <see cref="Workbook"/>
		/// and the download can thus not be cancelled individually. <c>False</c> otherwise.
		/// </summary>
        public bool IsSubProgress
        {
            get; private set;
        }

        public PageProgress(IPage page, CancellationTokenSource token) : base()
        {
            Page = page;
            CancellationToken = token;
            CancellationToken.Token.Register(() => Cancelled?.Invoke(this));
            CurrentProgress = 0;
            IsSubProgress = false;
        }

		/// <summary>
		/// See <see cref="IsSubProgress"/>.
		/// </summary>
        public void MarkAsSubProgress()
        {
            IsSubProgress = true;
        }

		/// <summary>
		/// Notifies the appropriate listeners (as indicated by the <see cref="ProgressHolder"/>) of download progress.
		/// </summary>
		/// <param name="value">The current download progress, in percent</param>
        protected override void OnReport(double value)
        {
            if (ProgressHolder.Instance.PageListener.ContainsKey(Page.Id))
            {
                ProgressHolder.Instance.PageListener[Page.Id].Invoke(value);
            }

            CurrentProgress = value;

            base.OnReport(value);
        }

		/// <summary>
		/// Informs this instance of its current download progress.
		/// </summary>
		/// <param name="progress">The current download progress, in percent</param>
        public void Report(double progress)
        {
            OnReport(progress);
        }
    }
}
