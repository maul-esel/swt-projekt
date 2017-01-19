using Lingvo.MobileApp.Proxies;
using MobileApp.Entities;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoTeacherMemoViewCell : ViewCell
    {
        public LingvoTeacherMemoViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            titleLabel.SetBinding(Label.TextProperty, "Name");

            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
                HeightRequest = Device.OnPlatform(iOS: 60, Android: 72, WinPhone: 240),
                Orientation = StackOrientation.Horizontal,
                Children =
                                {
                                    titleLabel
                                }

            };
        }
    }
}
