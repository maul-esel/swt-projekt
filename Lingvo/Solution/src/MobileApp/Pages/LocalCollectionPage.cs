using Lingvo.Common.Entities;
using Lingvo.MobileApp.Templates;
using MobileApp.Entities;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public partial class LocalCollectionPage : ContentPage
    {
        public LocalCollectionPage()
        {
            Title = ((Span)App.Current.Resources["page_title_localCollection"]).Text;
            Icon = (FileImageSource)ImageSource.FromFile("Icon.png");

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = LocalCollection.GetInstance().Workbooks,
                ItemTemplate = new LingvoViewCellTemplate("Title", "Subtitle"),
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
                    Text = ((Span)App.Current.Resources["label_no_local_workbooks"]).Text,
                    TextColor = (Color)App.Current.Resources["primaryColor"],
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    HorizontalOptions=LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    IsVisible=LocalCollection.GetInstance().Workbooks.Count == 0,
                    LineBreakMode = LineBreakMode.WordWrap
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

            await App.Current.MainPage.Navigation.PushAsync(new WorkbookPagesPage((Workbook)e.SelectedItem));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
