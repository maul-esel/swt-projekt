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
using Lingvo.MobileApp.Services.Progress;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadWorkbookViewCell : LingvoWorkbookViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);

        private static readonly FileImageSource cancelImage = (FileImageSource)ImageSource.FromFile("ic_action_cancel.png");
        private static readonly FileImageSource downloadImage = (FileImageSource)ImageSource.FromFile("ic_action_download.png");

        private LingvoRoundImageButton downloadButton;
        private CancellationTokenSource cancellationToken;

        public LingvoDownloadWorkbookViewCell() : base()
        {
            downloadButton = new LingvoRoundImageButton()
            {
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

        protected override void Event_PageChanged(IPage p)
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ProgressHolder.Instance.SetWorkbookProgressListener((Workbook)BindingContext, OnProgressUpdate);
        }

        protected override void OnDisappearing()
        {
            ProgressHolder.Instance.UnsetWorkbookProgressListener((Workbook)BindingContext);
            base.OnDisappearing();
        }

        private async void DownloadWorkbook()
        {

            if (!ProgressHolder.Instance.HasWorkbookProgress(((Workbook)BindingContext).Id))
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

                await CloudLibraryProxy.Instance.DownloadWorkbook(((Workbook)BindingContext).Id, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                cancellationToken?.Cancel();
                OnBindingContextChanged();
            }
            cancellationToken = null;
            Device.BeginInvokeOnMainThread(() => downloadButton.Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"));
        }

        private void OnProgressUpdate(double progress)
        {
            Device.BeginInvokeOnMainThread(() => ProgressView.Progress = (int)progress);
        }

        protected override void BindViewCell(Workbook workbook)
        {
            bool hasPages = workbook.TotalPages > 0;

            Workbook localWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(workbook.Id));

            int progress = 100;
            string color = "primaryColor";

            if (hasPages)
            {
                progress = Math.Min(100, 100 * (localWorkbook?.Pages.Count).GetValueOrDefault(0) / workbook.TotalPages);

                if (progress == 100)
                {
                    color = "secondaryColor";
                }

                ProgressView.MaxProgress = 100;
            }
            else
            {
                ProgressView.MaxProgress = 0;
            }

            if (NewerVersionExists(workbook))
            {
                ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Error;
                ProgressView.OuterProgressColor = Color.Red;
                SubtitleLabel.Text = ((Span)App.Current.Resources["label_newer_version"]).Text;
                SubtitleLabel.IsVisible = true;
                progress = 0;
            }
            else
            {
                ProgressView.OuterProgressColor = (Color)App.Current.Resources[color];
                ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Percentual;
            }

            ProgressView.InnerProgressEnabled = false;
            ProgressView.TextSize = 20;

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

            if (ProgressHolder.Instance.HasWorkbookProgress(workbook.Id))
            {
                WorkbookProgress workbookProgress = ProgressHolder.Instance.GetWorkbookProgress(workbook.Id);
                progress = (int)workbookProgress.CurrentProgress;
                cancellationToken = workbookProgress.CancellationToken;
            }
            else
            {
                cancellationToken = null;
            }

            bool hasRunningToken = cancellationToken != null && !cancellationToken.IsCancellationRequested;

            if (ProgressHolder.Instance.HasWorkbookProgress(workbook.Id) && hasRunningToken)
            {
                downloadButton.Image = cancelImage;
            }
            else
            {
                downloadButton.Image = downloadImage;
            }

            ProgressView.Progress = progress;

            downloadButton.IsEnabled = hasPages && progress < 100 && (!ProgressHolder.Instance.HasWorkbookProgress(workbook.Id) || hasRunningToken);
        }

        private bool NewerVersionExists(Workbook workbook)
        {
            foreach (IPage page in workbook.Pages)
            {
                if (((PageProxy)page).NewerVersionExists())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
