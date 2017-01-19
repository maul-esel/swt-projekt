using Lingvo.Common.Entities;
using Lingvo.MobileApp.Proxies;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoPageViewCell : ViewCell
    {
        internal LingvoSingleProgressView ProgressView
        {
            get; private set;
        }

        private Label subtitleLabel;
        public LingvoPageViewCell() :
            base()
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
                Size = Device.OnPlatform(iOS: 50, Android: 120, WinPhone: 240),
                LabelType = LingvoSingleProgressView.LabelTypeValue.None
            };

            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
				HeightRequest = Device.OnPlatform(iOS: 60, Android: 130, WinPhone: 260),
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
                                        }
                                }

            };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            IPage page = (IPage)BindingContext;

            string color = page.StudentTrack != null ? "secondaryColor" : "primaryColor";
            ProgressView.ProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = 1;
            ProgressView.Progress = 1;
            ProgressView.LabelType = LingvoSingleProgressView.LabelTypeValue.None;

            subtitleLabel.IsVisible = page.Description?.Length > 0;
        }
    }
}
