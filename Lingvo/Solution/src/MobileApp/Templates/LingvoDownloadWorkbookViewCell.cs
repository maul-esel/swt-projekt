using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using System;
using Xamarin.Forms;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Entities;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadWorkbookViewCell : LingvoWorkbookViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);


        public LingvoDownloadWorkbookViewCell() : base()
        {
            LingvoRoundImageButton downloadButton = new LingvoRoundImageButton()
            {
                Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"),
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = DownloadButtonSize,
                HeightRequest = DownloadButtonSize,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };

			downloadButton.OnClicked += (o, e) => DownloadWorkbook();

            ((StackLayout)View).Children.Add(downloadButton);
        }

        private async void DownloadWorkbook()
        {
            await CloudLibraryProxy.Instance.DownloadWorkbook(((Workbook)BindingContext).Id);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Workbook workbook = (Workbook)BindingContext;

            string color = workbook.Pages.Count == workbook.TotalPages ? "secondaryColor" : "primaryColor";
            ProgressView.OuterProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = workbook.TotalPages;
            ProgressView.Progress = workbook.Pages.Count;
            ProgressView.InnerProgressEnabled = false;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Percentual;
        }
    }
}
