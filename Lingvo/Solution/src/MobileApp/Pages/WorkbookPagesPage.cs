using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Templates;
using System.Collections.Generic;
using System.Linq;
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
                HasUnevenRows = true
            };

            listView.ItemTapped += Handle_ItemTapped;
            listView.ItemSelected += Handle_ItemSelected;

            listView.RefreshCommand = new Command(() => Device.BeginInvokeOnMainThread(async () =>
             {
                 Workbook newWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(workbook.Id));
                 if (newWorkbook != null && newWorkbook.Pages.Count > 0)
                 {
                     listView.ItemsSource = newWorkbook.Pages;
                 }
                 else
                 {
                     await App.Current.MainPage.Navigation.PopAsync();
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
                listView
                }
            };
			NavigationPage.SetBackButtonTitle(this, "Zurück");
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
