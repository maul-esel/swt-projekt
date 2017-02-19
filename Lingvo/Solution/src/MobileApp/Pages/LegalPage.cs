using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public class LegalPage : ContentPage
    {
        public LegalPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = ((Span)App.Current.Resources["legal_content"]).Text }
                }
            };
        }
    }
}
