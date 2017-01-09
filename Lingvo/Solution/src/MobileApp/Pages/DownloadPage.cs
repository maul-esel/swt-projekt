using Lingvo.Common.Entities;
using Lingvo.MobileApp.Templates;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public partial class DownloadPage : ContentPage
    {
        public DownloadPage()
        {
            Title = ((Span)App.Current.Resources["page_title_download"]).Text;
            Icon = (FileImageSource)ImageSource.FromFile("Icon.png");

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = new List<Workbook> {
                    new Workbook()
                    {
                        Title = "Thannhauser", Subtitle = "Lloret Ipsum",
                        Pages = { new Common.Entities.Page() { Number = 1 }, new Common.Entities.Page() { Number = 2 } }
                    }
                },
                ItemTemplate = new LingvoDownloadViewCellTemplate("Title", "Subtitle", "Id", new Action<object>(param => Console.WriteLine(param))),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true

            };

            listView.ItemTapped += Handle_ItemTapped;
            listView.ItemSelected += Handle_ItemSelected;

            Content = new StackLayout
            {
                Children = {
                listView,
                new Label
                {
                    Text = ((Span)App.Current.Resources["label_sync_error"]).Text,
                    TextColor = (Color)App.Current.Resources["primaryColor"],
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    HorizontalOptions=LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    LineBreakMode = LineBreakMode.WordWrap,
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

            await App.Current.MainPage.Navigation.PushAsync(new DownloadPagesPage((Workbook)e.SelectedItem));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
