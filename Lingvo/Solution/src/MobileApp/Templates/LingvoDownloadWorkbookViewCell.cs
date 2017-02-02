using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using System;
using Xamarin.Forms;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Entities;
using System.Collections.Generic;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadWorkbookViewCell : LingvoWorkbookViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);

        private LingvoRoundImageButton downloadButton;

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
            await CloudLibraryProxy.Instance.DownloadWorkbook(((Workbook)BindingContext).Id);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Workbook workbook = (Workbook)BindingContext;

            List<Workbook> currentWorkbooks = new List<Workbook>(LocalCollection.Instance.Workbooks);

            int progress = (currentWorkbooks.Find(w => w.Id.Equals(workbook.Id))?.Pages.Count).GetValueOrDefault(0);
            string color = progress == workbook.TotalPages ? "secondaryColor" : "primaryColor";
            ProgressView.OuterProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = workbook.TotalPages;
            ProgressView.Progress = progress;
            ProgressView.InnerProgressEnabled = false;
			ProgressView.TextSize = 20;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Percentual;

            ContextActions.Clear();

            downloadButton.IsEnabled = progress != workbook.TotalPages;
        }
    }
}
