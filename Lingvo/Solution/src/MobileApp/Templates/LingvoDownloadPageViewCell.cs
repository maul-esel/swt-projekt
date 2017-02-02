using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Proxies;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadPageViewCell : LingvoPageViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);

        private LingvoRoundImageButton downloadButton;

        public LingvoDownloadPageViewCell() : base()
        {
            downloadButton = new LingvoRoundImageButton()
            {
                Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"),
                HorizontalOptions = LayoutOptions.End,
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = DownloadButtonSize,
                HeightRequest = DownloadButtonSize,
                VerticalOptions = LayoutOptions.Center
            };

            downloadButton.OnClicked += (o, e) => DownloadPage();

            ((StackLayout)View).Children.Add(downloadButton);
        }

        private async void DownloadPage()
        {
            await ((PageProxy)BindingContext).Resolve();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            IPage page = (IPage)BindingContext;

            Workbook localWorkbook = new List<Workbook>(LocalCollection.Instance.Workbooks).Find(w => w.Id.Equals(page.workbookId));
            bool downloaded = localWorkbook?.Pages.Find(p => p.Id.Equals(page.Id)) != null;

            string color = downloaded ? "secondaryColor" : "primaryColor";
            ProgressView.InnerProgressEnabled = false;
            ProgressView.OuterProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = 100;
            ProgressView.Progress = downloaded ? 100 : 0;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Percentual;

            if (ContextActions.Count > 0)
            {
                ContextActions.Clear();
            }

            downloadButton.IsEnabled = !downloaded;
        }

        protected override void Event_PageChanged(Lingvo.Common.Entities.Page p)
        {
            IPage page = (IPage)BindingContext;
            if (p.Id.Equals(page.Id))
            {
                OnBindingContextChanged();
            }
        }
    }
}
