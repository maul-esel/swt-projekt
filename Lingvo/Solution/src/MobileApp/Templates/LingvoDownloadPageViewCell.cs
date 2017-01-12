using Lingvo.Common.Entities;
using Lingvo.MobileApp.Proxies;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadPageViewCell : ViewCell
    {
        internal LingvoSingleProgressView ProgressView
        {
            get; private set;
        }

        private Label subtitleLabel;

        public LingvoDownloadPageViewCell(Command action) : base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            string seite = ((Span)App.Current.Resources["text_seite"]).Text;
            titleLabel.SetBinding(Label.TextProperty, "Number", BindingMode.Default, null, seite + " {0}");

            subtitleLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                IsVisible = false
            };
                subtitleLabel.SetBinding(Label.TextProperty, "Description");

            ProgressView = new LingvoSingleProgressView()
            {
                Size = Device.OnPlatform(iOS: 100, Android: 100, WinPhone: 200),
                LabelType = LingvoSingleProgressView.LabelTypeValue.Percentual
            };
            
            Button downloadButton = new Button()
            {
                Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"),
                Command = action,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };

            downloadButton.SetBinding(Button.CommandParameterProperty, "Number");


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

            IPage page = (IPage)BindingContext;

            string color = page.TeacherTrack != null ? "secondaryColor" : "primaryColor";
            ProgressView.ProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = 100;
            ProgressView.Progress = page.TeacherTrack != null ? 100 : 0;
            ProgressView.LabelType = LingvoSingleProgressView.LabelTypeValue.Percentual;
            
            subtitleLabel.IsVisible = page.Description?.Length > 0;
        }
    }
}
