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

        internal int PageId
        {
            get; private set;
        }

        public PageProgress(int pageId) : base()
        {
            PageId = pageId;
            CurrentProgress = 0;
        }

        protected override void OnReport(double value)
        {
            if (ProgressHolder.Instance.PageListener.ContainsKey(PageId))
            {
                ProgressHolder.Instance.PageListener[PageId].Invoke(value);
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
