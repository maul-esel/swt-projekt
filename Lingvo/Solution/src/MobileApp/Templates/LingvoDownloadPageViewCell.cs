using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Services;
using Lingvo.MobileApp.Services.Progress;
using Lingvo.MobileApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    /// <summary>
    /// The ViewCell for displaying information of and providing a download button for a page on the server.
    /// </summary>
    class LingvoDownloadPageViewCell : LingvoPageViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);

        private static readonly FileImageSource cancelImage = (FileImageSource)ImageSource.FromFile("ic_action_cancel.png");
        private static readonly FileImageSource downloadImage = (FileImageSource)ImageSource.FromFile("ic_action_download.png");

        private LingvoRoundImageButton downloadButton;

        /// <summary>
        /// The <c>CancellationTokenSource</c> for a async download.
        /// </summary>
        private CancellationTokenSource cancellationToken;

        public LingvoDownloadPageViewCell() : base()
        {
            downloadButton = new LingvoRoundImageButton()
            {
                HorizontalOptions = LayoutOptions.End,
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = DownloadButtonSize,
                HeightRequest = DownloadButtonSize,
                VerticalOptions = LayoutOptions.Center
            };

            downloadButton.OnClicked += (o, e) => DownloadButton_Clicked();

            ((StackLayout)View).Children.Add(downloadButton);
        }

        /// <summary>
        /// Occurs when the download or cancel button (which are basically the same button instance) has been clicked.
        /// If the page has no corresponding download progress, a new download is started; if the device is not connected
        /// to Wifi, a warning dialog is shown before, starting the download only with positive result.
        /// If a running download already exists, the download is cancelled.
        /// </summary>
        private async void DownloadButton_Clicked()
        {
            if (!ProgressHolder.Instance.HasPageProgress(((IPage)BindingContext).Id))
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

                await ((PageProxy)BindingContext).Resolve(cancellationToken);
            }
            else
            {
                cancellationToken.Cancel();
            }
            cancellationToken = null;
            Device.BeginInvokeOnMainThread(() => downloadButton.Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"));
        }

        /// <summary>
        /// Called when the view cell appears on screen.
        /// Registers <see cref="OnProgressUpdate(double)"/> as progress listener in <see cref="ProgressHolder"/>.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ProgressHolder.Instance.SetPageProgressListener((IPage)BindingContext, OnProgressUpdate);
        }

        /// <summary>
        /// Called when the view cell disappears on screen.
        /// Unregisters the progress listener <see cref="OnProgressUpdate(double)"/> in <see cref="ProgressHolder"/>.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ProgressHolder.Instance.UnsetPageProgressListener((IPage)BindingContext);
        }

        /// <summary>
        /// Called when the download progress reports a progress update.
        /// Updates the progress of the progress view.
        /// </summary>
        /// <param name="progress">The new progress.</param>
        private void OnProgressUpdate(double progress)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ProgressView.Progress = (int)progress;
            });
        }

        /// <summary>
        /// Binds the views in this view cell to the given page.
        /// Actually, it refreshes the progress view and the download button.
        /// </summary>
        /// <param name="page">The page to bind this view cell to.</param>
        protected override void BindViewCell(IPage page)
        {
            Workbook localWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(page.workbookId));
            IPage localPage = localWorkbook?.Pages.Find(p => p.Id.Equals(page.Id));
            bool downloaded = localPage != null;

            if (downloaded && ((PageProxy)page).NewerVersionExists())
            {
                //A newer version of the page exists on the server, so show the exclamation mark 
                //in progress view and re-enable the download button by setting downloaded to false

                downloaded = false;
                ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Error;
                ProgressView.OuterProgressColor = Color.Red;
                SubtitleLabel.Text = ((Span)App.Current.Resources["label_newer_version"]).Text;
                SubtitleLabel.IsVisible = true;
            }
            else
            {
                string color = downloaded ? "secondaryColor" : "primaryColor";
                ProgressView.OuterProgressColor = (Color)App.Current.Resources[color];
                ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Percentual;
            }

            ProgressView.InnerProgressEnabled = false;
            ProgressView.MaxProgress = 100;

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

            int progress = 0;
            bool hasRunningToken = cancellationToken != null && !cancellationToken.IsCancellationRequested;
            bool hasSubProgress = false;

            if (ProgressHolder.Instance.HasPageProgress(page.Id))
            {
                //A running download progress exists, so bind this cancellationToken to
                //the one of the progress
                PageProgress pageProgress = ProgressHolder.Instance.GetPageProgress(page.Id);
                progress = (int)pageProgress.CurrentProgress;
                cancellationToken = pageProgress.CancellationToken;

                //Additionally, check if the progress is only a sub-progress
                hasSubProgress = pageProgress.IsSubProgress;
            }
            else
            {
                cancellationToken = null;
            }

            hasRunningToken = cancellationToken != null && !cancellationToken.IsCancellationRequested;

            //Set the button image according to the download state
            if (!downloaded && ProgressHolder.Instance.HasPageProgress(page.Id) && hasRunningToken)
            {
                downloadButton.Image = cancelImage;
            }
            else
            {
                downloadButton.Image = downloadImage;
            }

            ProgressView.Progress = downloaded ? 100 : progress;

            //Disable the button if the page is already downloaded or a sub-progress for this page exists, otherwise enable it
            downloadButton.IsEnabled = !downloaded && !hasSubProgress && (!ProgressHolder.Instance.HasPageProgress(page.Id) || hasRunningToken);
        }

        /// <summary>
        /// Occurs when a page has changed.
        /// Refreshes the <c>BindingContext</c> of this view, if the changed page is equal to it.
        /// </summary>
        /// <param name="p">The page which has changed.</param>
        protected override void Event_PageChanged(IPage p)
        {
            IPage page = (IPage)BindingContext;
            if (p.Id.Equals(page.Id))
            {
                OnBindingContextChanged();
            }
        }
    }
}
