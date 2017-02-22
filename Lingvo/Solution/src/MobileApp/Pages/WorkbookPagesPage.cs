using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Templates;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    /// <summary>
    /// The page for displaying all locally available pages of a workbook.
    /// </summary>
    public partial class WorkbookPagesPage : ContentPage
    {
        private Workbook workbook;

        private ListView listView;

        public WorkbookPagesPage(Workbook workbook)
        {
            this.workbook = workbook;

            Title = workbook.Title;

            string seite = ((Span)App.Current.Resources["text_seite"]).Text;

            listView = new ListView(ListViewCachingStrategy.RecycleElement)
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

            Content = new StackLayout
            {
                Children = {
                listView
                }
            };

			#if __IOS__
				NavigationPage.SetBackButtonTitle(this, "Zurück");
			#endif
        }

        /// <summary>
        /// Registers all important events and refreshes the list.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            LocalCollection.Instance.WorkbookChanged += OnWorkbookChanged;
            LocalCollection.Instance.PageChanged += OnPageChanged;

            listView.RefreshCommand.Execute(null);
        }

        /// <summary>
        /// Unregisters the events registered in <see cref="OnAppearing"/>.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            LocalCollection.Instance.WorkbookChanged -= OnWorkbookChanged;

            LocalCollection.Instance.PageChanged -= OnPageChanged;
        }

        /// <summary>
        /// Occurs when a workbook has changed.
        /// Refreshes the list, if its the workbook associated with this page.
        /// </summary>
        /// <param name="workbook">The workbook which changed.</param>
        private void OnWorkbookChanged(Workbook w)
        {
            if (workbook.Id.Equals(w.Id))
            {
                listView.RefreshCommand.Execute(null);
            }
        }

        /// <summary>
        /// Occurs when a IPage has changed.
        /// Refreshes the list if the IPage is displayed in this page.
        /// </summary>
        /// <param name="workbook">The workbook which changed.</param>
        private void OnPageChanged(IPage p)
        {
            if (workbook.Id.Equals(p.workbookId))
            {
                listView.RefreshCommand.Execute(null);
            }
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        /// <summary>
        /// Opens the selected IPage in an <see cref="AudioPage"/>.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The <c>SelectedItemChangedEventArgs</c> of the event.</param>
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
