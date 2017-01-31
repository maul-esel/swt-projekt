using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Templates;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public partial class WorkbookPagesPage : ContentPage
    {
        private Workbook workbook;
        public WorkbookPagesPage(Workbook workbook)
        {
            this.workbook = workbook;

            Title = workbook.Title;

            string seite = ((Span)App.Current.Resources["text_seite"]).Text;

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = workbook.Pages,
                ItemTemplate = new DataTemplate(typeof(LingvoPageViewCell)),
                HasUnevenRows = true,
                IsVisible = workbook.Pages.Count > 0
            };

            listView.ItemTapped += Handle_ItemTapped;
            listView.ItemSelected += Handle_ItemSelected;

            Label errorLabel = new Label
            {
                Text = ((Span)App.Current.Resources["label_no_local_pages"]).Text,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap,
                IsVisible = workbook.Pages.Count == 0

            };

            listView.RefreshCommand = new Command(() => Device.BeginInvokeOnMainThread(() =>
            {
                Workbook newWorkbook = new List<Workbook>(LocalCollection.Instance.Workbooks).Find(w => w.Id.Equals(workbook.Id));
                if (newWorkbook != null)
                {
                    listView.ItemsSource = newWorkbook.Pages;
                    listView.IsVisible = workbook.Pages.Count > 0;
                    errorLabel.IsVisible = workbook.Pages.Count == 0;
                }
            }));

            LocalCollection.Instance.WorkbookChanged += (w) =>
            {
                if (workbook.Id.Equals(w.Id))
                {
                    listView.RefreshCommand.Execute(null);
                }
            };

            LocalCollection.Instance.PageChanged += (p) =>
            {
                if (workbook.Id.Equals(p.workbookId))
                {
                    listView.RefreshCommand.Execute(null);
                }
            };

            Content = new StackLayout
            {
                Children = {
                listView,
                errorLabel
                }
            };
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await App.Current.MainPage.Navigation.PushAsync(new AudioPage((IPage)e.SelectedItem, workbook));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
