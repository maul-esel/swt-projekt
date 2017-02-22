using Lingvo.Common.Entities;
using Lingvo.MobileApp.Templates;
using Lingvo.MobileApp.Entities;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Lingvo.MobileApp.Pages
{
    /// <summary>
    /// The page for displaying all locally available workbooks.
    /// </summary>
    public partial class LocalCollectionPage : ContentPage
    {
        private ListView listView;

        public LocalCollectionPage()
        {
            Title = ((Span)App.Current.Resources["page_title_localCollection"]).Text;
			NavigationPage.SetBackButtonTitle(this, "Zurück");
            Icon = (FileImageSource)ImageSource.FromFile("ic_action_book.png");

            listView = new ListView(ListViewCachingStrategy.RecycleElement)
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

            listView.RefreshCommand = new Command(() => Device.BeginInvokeOnMainThread(() =>
            {
                Workbook[] newSource = LocalCollection.Instance.Workbooks.ToArray();
                listView.ItemsSource = newSource;
                errorLabel.IsVisible = newSource.Length == 0;
                listView.IsVisible = newSource.Length > 0;
            }));

            Content = new StackLayout
            {
                Children = {
                listView,
                errorLabel
                }
            };
        }

        /// <summary>
        /// Occurs when a workbook has changed.
        /// Refreshes the list.
        /// </summary>
        /// <param name="workbook">The workbook which changed.</param>
        private void OnWorkbookChanged(Workbook workbook)
        {
            listView.RefreshCommand.Execute(null);
        }

        /// <summary>
        /// Registers all important events and refreshes the list.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LocalCollection.Instance.WorkbookChanged += OnWorkbookChanged;
            listView.RefreshCommand.Execute(null);
        }

        /// <summary>
        /// Unregisters the events registered in <see cref="OnAppearing"/>.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            LocalCollection.Instance.WorkbookChanged -= OnWorkbookChanged;
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            => ((ListView)sender).SelectedItem = null;

        /// <summary>
        /// Opens the selected workbook and shows its pages in a <see cref="WorkbookPagesPage"/>.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The <c>SelectedItemChangedEventArgs</c> of the event.</param>
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
