using Lingvo.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lingvo.MobileApp.Services.Progress
{
    public class PageProgress : Progress<double>
    {
        public double CurrentProgress
        {
            get; private set;
        }

        public event Action Cancelled;

        internal IPage Page
        {
            get; private set;
        }

        public PageProgress(IPage page) : base()
        {
            Page = page;
            CurrentProgress = 0;
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

        public void Cancel()
        {
            Cancelled?.Invoke();
        }
    }
}
