using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoViewCellTemplate : DataTemplate
    {

        public LingvoViewCellTemplate(string textBindingPath) : this(textBindingPath, null, null, null) { }

        public LingvoViewCellTemplate(string textBindingPath, string subtextBindingPath) : this(textBindingPath, subtextBindingPath, null, null) { }

        public LingvoViewCellTemplate(string textBindingPath, string subtextBindingPath, string textFormat, string subtextFormat) :
            base(() =>
            {
                Label titleLabel = new Label()
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };
                titleLabel.SetBinding(Label.TextProperty, textBindingPath, BindingMode.Default, null, textFormat);

                Label subtitleLabel = new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    IsVisible = subtextBindingPath != null
                };

                if (subtextBindingPath != null)
                    subtitleLabel.SetBinding(Label.TextProperty, subtextBindingPath, BindingMode.Default, null, subtextFormat);

                BoxView progressView = new BoxView()
                {
                    Color = (Color)App.Current.Resources["primaryColor"]
                };

                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Padding = new Thickness(5, 5),
                        Orientation = StackOrientation.Horizontal,
                        Children =
                                {
                                    progressView,
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
                    }
                };
            })
        { }
    }
}
