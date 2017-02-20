using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public class LegalPage : ContentPage
    {
        public LegalPage()
        {
            Title = ((Span)App.Current.Resources["label_legal"]).Text;

            double fontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));

            Content = new StackLayout
            {
                Padding = new Thickness(15, 15),
                Children = {
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_university"]).Text,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = fontSize
                    },
                    new Label { Text = ((Span)App.Current.Resources["legal_institute"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_link"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Margin = new Thickness(0, 10),
                        Text = ((Span)App.Current.Resources["legal_partners"]).Text,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = fontSize
                    },
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_maiborn"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_tauschring"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Margin = new Thickness(0, 10),
                        Text = ((Span)App.Current.Resources["legal_members"]).Text,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = fontSize
                    },
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_jan"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_dominik"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_philip"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_katja"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Text = ((Span)App.Current.Resources["legal_ralph"]).Text,
                        FontSize = fontSize
                    },
                    new Label {
                        Margin = new Thickness(0, 10),
                        Text = ((Span)App.Current.Resources["legal_elite"]).Text,
                        FontSize = fontSize
                    }
                }
            };
        }
    }
}
