using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using System;
using Xamarin.Forms;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Lingvo.MobileApp.Services;
using Lingvo.MobileApp.Util;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadWorkbookViewCell : LingvoWorkbookViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);

        private LingvoRoundImageButton downloadButton;
        private CancellationTokenSource cancellationToken;

        public LingvoDownloadWorkbookViewCell() : base()
        {
            downloadButton = new LingvoRoundImageButton()
            {
                Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"),
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = DownloadButtonSize,
                MinimumWidthRequest = DownloadButtonSize,
                HeightRequest = DownloadButtonSize,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
            };

            downloadButton.OnClicked += (o, e) => DownloadWorkbook();

            ((Grid)View).Children.Add(downloadButton, 1, 0);
        }

        protected override void Event_PageChanged(Lingvo.Common.Entities.Page p)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (p.workbookId.Equals(workbook.Id))
            {
                OnBindingContextChanged();
            };
        }

        protected override void Event_WorkbookChanged(Workbook w)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (w.Id.Equals(workbook.Id))
            {
                OnBindingContextChanged();
            }
        }

        private async void DownloadWorkbook()
        {

            if (cancellationToken == null)
            {
                if (!await AlertHelper.DisplayWarningIfNotWifiConnected())
                {
                    return;
                }
                cancellationToken = new CancellationTokenSource();
                cancellationToken.Token.Register(() => Device.BeginInvokeOnMainThread(() =>
                {
                    downloadButton.Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png");
                    OnBindingContextChanged();
                }));
                downloadButton.Image = (FileImageSource)ImageSource.FromFile("ic_action_cancel.png");

                await CloudLibraryProxy.Instance.DownloadWorkbook(((Workbook)BindingContext).Id, new Progress<double>(OnProgressUpdate), cancellationToken.Token);
            }
            else
            {
                cancellationToken.Cancel();
                OnBindingContextChanged();
            }
            cancellationToken = null;
            Device.BeginInvokeOnMainThread(() => downloadButton.Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"));
        }

        private void OnProgressUpdate(double progress)
        {
            Device.BeginInvokeOnMainThread(() => ProgressView.Progress = (int)progress);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Workbook workbook = (Workbook)BindingContext;

            Workbook localWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(workbook.Id));

            int progress = Math.Min(100, 100 * (localWorkbook?.Pages.Count).GetValueOrDefault(0) / workbook.TotalPages);
            string color = progress == 100 ? "secondaryColor" : "primaryColor";
            ProgressView.OuterProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = 100;
            ProgressView.Progress = progress;
            ProgressView.InnerProgressEnabled = false;
            ProgressView.TextSize = 20;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Percentual;

            try
            {
                if (ContextActions.Count > 0)
                {
                    ContextActions.Clear();
                }
            }
            catch
            {
                Console.WriteLine("Context Actions null");
            }

            downloadButton.IsEnabled = progress < 100;
        }
    }
}
