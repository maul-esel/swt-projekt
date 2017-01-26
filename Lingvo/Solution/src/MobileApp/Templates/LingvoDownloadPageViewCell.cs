using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Proxies;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadPageViewCell : LingvoPageViewCell
    {
        public delegate void OnDownloadClickedHandler(IPage page);

        public LingvoDownloadPageViewCell(OnDownloadClickedHandler handler) : base()
        {
            LingvoRoundImageButton downloadButton = new LingvoRoundImageButton()
            {
                Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"),
                HorizontalOptions = LayoutOptions.End,
                Color = (Color)App.Current.Resources["primaryColor"],
                VerticalOptions = LayoutOptions.Center
            };

            downloadButton.OnClicked += (o, e) => handler((IPage)BindingContext);

            ((StackLayout)View).Children.Add(downloadButton);            
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            IPage page = (IPage)BindingContext;

            string color = page.TeacherTrack != null ? "secondaryColor" : "primaryColor";
            ProgressView.OuterProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = 100;
            ProgressView.Progress = page.TeacherTrack != null ? 100 : 0;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Percentual;
        }
    }
}
