using Lingvo.Common.Entities;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadWorkbookViewCell : ViewCell
    {
        internal LingvoSingleProgressView ProgressView
        {
            get; private set;
        }

        private Label subtitleLabel;
        public LingvoDownloadWorkbookViewCell( Command buttonCommand) : base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            titleLabel.SetBinding(Label.TextProperty, "Title");

            subtitleLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                IsVisible = false
            };
            subtitleLabel.SetBinding(Label.TextProperty, "Subtitle");

            ProgressView = new LingvoSingleProgressView()
            {
                Size = Device.OnPlatform(iOS: 100, Android: 100, WinPhone: 200),
                LabelType = LingvoSingleProgressView.LabelTypeValue.Percentual
            };
           
            Button downloadButton = new Button()
            {
                Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"),
                Command = buttonCommand,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };

            downloadButton.SetBinding(Button.CommandParameterProperty, "Id");


            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
                Orientation = StackOrientation.Horizontal,
                Children =
                                {
                                    ProgressView,
                                    new StackLayout
                                    {
                                        HorizontalOptions = LayoutOptions.StartAndExpand,
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            titleLabel,
                                            subtitleLabel
    }
},
                                    downloadButton
                                }
            };
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

            subtitleLabel.IsVisible = workbook.Subtitle?.Length > 0;
        }
    }
}
