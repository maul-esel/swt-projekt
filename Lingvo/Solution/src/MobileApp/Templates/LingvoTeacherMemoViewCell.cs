using Lingvo.MobileApp.Proxies;
using MobileApp.Entities;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoTeacherMemoViewCell : ViewCell
    {
        internal LingvoSingleProgressView ProgressView
        {
            get; private set;
        }

        public LingvoTeacherMemoViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            string seite = ((Span)App.Current.Resources["text_seite"]).Text;
            titleLabel.SetBinding(Label.TextProperty, "Name");

            ProgressView = new LingvoSingleProgressView()
            {
                Size = 72,
                LabelType = LingvoSingleProgressView.LabelTypeValue.NOfM
            };

            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
                Orientation = StackOrientation.Horizontal,
                Children =
                                {
                                    titleLabel
                                }

            };
        }
    }
}
