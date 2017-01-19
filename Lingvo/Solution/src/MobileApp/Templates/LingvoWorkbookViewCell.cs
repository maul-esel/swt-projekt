using Lingvo.Common.Entities;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoWorkbookViewCell : ViewCell
    {
        internal LingvoSingleProgressView ProgressView
        {
            get; private set;
        }

        private Label subtitleLabel;
        public LingvoWorkbookViewCell() :
            base()
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
                Size = Device.OnPlatform(iOS: 80, Android: 120, WinPhone: 240),
                LabelType = LingvoSingleProgressView.LabelTypeValue.NOfM
            };


            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
				HeightRequest = Device.OnPlatform(iOS: 70, Android:72, WinPhone:260),
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

            Workbook workbook = (Workbook)BindingContext;

            int completed = 0;
            workbook.Pages.ForEach((p) => { if (p.StudentTrack != null) completed++; });
            string color = workbook.Pages.Count == completed ? "secondaryColor" : "primaryColor";
            ProgressView.ProgressColor = (Color)App.Current.Resources[color];
            ProgressView.MaxProgress = workbook.Pages.Count;
            ProgressView.Progress = completed;
            ProgressView.LabelType = LingvoSingleProgressView.LabelTypeValue.NOfM;

            subtitleLabel.IsVisible = workbook.Subtitle?.Length > 0;
        }

    }
}
