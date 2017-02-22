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
    /// <summary>
    /// The ViewCell for displaying information of and providing a download button for a workbook on the server.
    /// </summary>
    class LingvoDownloadWorkbookViewCell : LingvoWorkbookViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);

        private static readonly FileImageSource cancelImage = (FileImageSource)ImageSource.FromFile("ic_action_cancel.png");
        private static readonly FileImageSource downloadImage = (FileImageSource)ImageSource.FromFile("ic_action_download.png");

        private LingvoRoundImageButton downloadButton;

        /// <summary>
        /// The <c>CancellationTokenSource</c> for a async download.
        /// </summary>
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

        /// <summary>
        /// Occurs when a page has changed.
        /// Refreshes the <c>BindingContext</c> of this view, if the workbook contains the changed page.
        /// </summary>
        /// <param name="p">The page which has changed.</param>
        protected override void Event_PageChanged(IPage p)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (p.workbookId.Equals(workbook.Id))
            {
                OnBindingContextChanged();
            };
        }

        /// <summary>
        /// Occurs when a workbook has changed.
        /// Refreshes the <c>BindingContext</c> of this view, if the changed workbook is equal to it.
        /// </summary>
        /// <param name="w">The workbook which has changed.</param>
        protected override void Event_WorkbookChanged(Workbook w)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (w.Id.Equals(workbook.Id))
            {
                OnBindingContextChanged();
            }
        }

        /// <summary>
        /// Called when the view cell appears on screen.
        /// Registers <see cref="OnProgressUpdate(double)"/> as progress listener in <see cref="ProgressHolder"/>.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ProgressHolder.Instance.SetWorkbookProgressListener((Workbook)BindingContext, OnProgressUpdate);
        }

        /// <summary>
        /// Called when the view cell disappears on screen.
        /// Unregisters the progress listener <see cref="OnProgressUpdate(double)"/> in <see cref="ProgressHolder"/>.
        /// </summary>
        protected override void OnDisappearing()
        {
            ProgressHolder.Instance.UnsetWorkbookProgressListener((Workbook)BindingContext);
            base.OnDisappearing();
        }

        /// <summary>
        /// Occurs when the download or cancel button (which are basically the same button instance) has been clicked.
        /// If the workbook has no corresponding download progress, a new download is started; if the device is not connected
        /// to Wifi, a warning dialog is shown before, starting the download only with positive result.
        /// If a running download already exists, the download is cancelled.
        /// </summary>
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

        /// <summary>
        /// Called when the download progress reports a progress update.
        /// Updates the progress of the progress view.
        /// </summary>
        /// <param name="progress">The new progress.</param>
        private void OnProgressUpdate(double progress)
        {
            Device.BeginInvokeOnMainThread(() => ProgressView.Progress = (int)progress);
        }

        /// <summary>
        /// Binds the views in this view cell to the given workbook.
        /// Actually, it refreshes the progress view and the download button.
        /// </summary>
        /// <param name="workbook">The workbook to bind this view cell to.</param>
        protected override void BindViewCell(Workbook workbook)
        {
            bool hasPages = workbook.TotalPages > 0;

            Workbook localWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(workbook.Id));

            int progress = 100;
            string color = "primaryColor";

            if (hasPages)
            {
                //Pages exist, so we can compute a progress
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
                //A newer version of a page in this workbook exists on the server, so show the exclamation mark 
                //in progress view and re-enable the download button by setting progress to 0

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
                //A running download progress exists, so bind this cancellationToken to
                //the one of the progress

                WorkbookProgress workbookProgress = ProgressHolder.Instance.GetWorkbookProgress(workbook.Id);
                progress = (int)workbookProgress.CurrentProgress;
                cancellationToken = workbookProgress.CancellationToken;
            }
            else
            {
                cancellationToken = null;
            }

            bool hasRunningToken = cancellationToken != null && !cancellationToken.IsCancellationRequested;

            //Set the button image according to the download state
            if (ProgressHolder.Instance.HasWorkbookProgress(workbook.Id) && hasRunningToken)
            {
                downloadButton.Image = cancelImage;
            }
            else
            {
                downloadButton.Image = downloadImage;
            }

            ProgressView.Progress = progress;

            //Disable the download button if the workbook has no pages on the server or it's alredy downloaded completely
            downloadButton.IsEnabled = hasPages && progress < 100 && (!ProgressHolder.Instance.HasWorkbookProgress(workbook.Id) || hasRunningToken);
        }

        /// <summary>
        /// Checks if a newer version exists for every page in the workbook.
        /// </summary>
        /// <param name="workbook">The workbook to check.</param>
        /// <returns><c>True</c>, if a newer version exists on the server for at least one page, <c>false</c> otherwise.</returns>
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
