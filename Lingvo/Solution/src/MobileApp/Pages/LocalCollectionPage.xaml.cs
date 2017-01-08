using Lingvo.Common.Entities;
using MobileApp.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lingvo.MobileApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalCollectionPage : ContentPage
    {
        public LocalCollectionPage()
        {
            InitializeComponent();

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = LocalCollection.GetInstance().Workbooks,
                ItemTemplate = new DataTemplate(() =>
                {
                    Label titleLabel = new Label();
                    titleLabel.FontAttributes = FontAttributes.Bold;
                    titleLabel.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                    titleLabel.SetBinding(Label.TextProperty, "Title");

                    Label subtitleLabel = new Label();
                    subtitleLabel.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                    subtitleLabel.SetBinding(Label.TextProperty, "Subtitle");

                    BoxView progressView = new BoxView() { Color = (Color)App.Current.Resources["primaryColor"] };

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
                }),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true,
                IsVisible = LocalCollection.GetInstance().Workbooks.Count > 0
            };

            listView.ItemTapped += Handle_ItemTapped;
            listView.ItemSelected += Handle_ItemSelected;

            Content = new StackLayout
            {
                Children = {
                listView,
                new Label
                {
                    Text = "Noch keine Arbeitshefte heruntergeladen!",
                    TextColor = (Color)App.Current.Resources["primaryColor"],
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    HorizontalOptions=LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    IsVisible=LocalCollection.GetInstance().Workbooks.Count == 0

                }
                }
            };
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            => ((ListView)sender).SelectedItem = null;

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await DisplayAlert("Selected", ((Workbook)e.SelectedItem).Title, "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
