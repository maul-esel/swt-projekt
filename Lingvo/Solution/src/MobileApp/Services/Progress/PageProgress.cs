using Lingvo.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lingvo.MobileApp.Services.Progress
{
    public class PageProgress : Progress<double>
    {
        public double CurrentProgress
        {
            get; private set;
        }

        public event Action<PageProgress> Cancelled;

        internal IPage Page
        {
            get; private set;
        }

        public CancellationTokenSource CancellationToken
        {
            get; private set;
        }

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

        public void MarkAsSubProgress()
        {
            IsSubProgress = true;
        }

        protected override void OnReport(double value)
        {
            if (ProgressHolder.Instance.PageListener.ContainsKey(Page.Id))
            {
                ProgressHolder.Instance.PageListener[Page.Id].Invoke(value);
            }

            CurrentProgress = value;

            base.OnReport(value);
        }

        public void Report(double progress)
        {
            OnReport(progress);
        }
    }
}
