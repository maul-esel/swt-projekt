using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadWorkbookViewCell : LingvoWorkbookViewCell
    {
        public delegate void OnDownloadClickedHandler(Workbook page);

        public LingvoDownloadWorkbookViewCell(OnDownloadClickedHandler handler) : base()
        {
            LingvoRoundImageButton downloadButton = new LingvoRoundImageButton()
            {
                Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"),
                Color = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };

            downloadButton.OnClicked += (o, e) => handler((Workbook)BindingContext);

            ((StackLayout)View).Children.Add(downloadButton);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Workbook workbook = (Workbook)BindingContext;

            string color = workbook.Pages.Count == workbook.TotalPages ? "secondaryColor" : "primaryColor";
            ProgressView.ProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = workbook.TotalPages;
            ProgressView.Progress = workbook.Pages.Count;
            ProgressView.LabelType = LingvoSingleProgressView.LabelTypeValue.Percentual;
        }
    }
}
