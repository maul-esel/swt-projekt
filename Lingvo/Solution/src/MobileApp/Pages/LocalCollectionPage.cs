using Lingvo.Common.Entities;
using Lingvo.MobileApp.Templates;
using Lingvo.MobileApp.Entities;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Lingvo.MobileApp.Pages
{
    public partial class LocalCollectionPage : ContentPage
    {
        public LocalCollectionPage()
        {
            Title = ((Span)App.Current.Resources["page_title_localCollection"]).Text;
            Icon = (FileImageSource)ImageSource.FromFile("ic_action_book.png");

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = LocalCollection.Instance.Workbooks,
                ItemTemplate = new DataTemplate(typeof(LingvoWorkbookViewCell)),
                HasUnevenRows = true,
                IsVisible = LocalCollection.Instance.Workbooks.Count() > 0
            };

            Label errorLabel = new Label
            {
                Text = ((Span)App.Current.Resources["label_no_local_workbooks"]).Text,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                IsVisible = LocalCollection.Instance.Workbooks.Count() == 0,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap
            };

            listView.ItemTapped += Handle_ItemTapped;
            listView.ItemSelected += Handle_ItemSelected;

            listView.RefreshCommand = new Command(() => Device.BeginInvokeOnMainThread(() => {
                List<Workbook> newSource = new List<Workbook>(LocalCollection.Instance.Workbooks);
                listView.ItemsSource = newSource;
                errorLabel.IsVisible = newSource.Count == 0;
                listView.IsVisible = newSource.Count > 0;
            }));

            LocalCollection.Instance.WorkbooksChanged += () => listView.RefreshCommand.Execute(null);

            Content = new StackLayout
            {
                Children = {
                listView,
                errorLabel
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
