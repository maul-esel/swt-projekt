using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    /// <summary>
    /// The page for downloading pages of a workbook from the server.
    /// </summary>
    public partial class DownloadPagesPage : ContentPage
    {
        private Workbook workbook;

        public DownloadPagesPage(Workbook workbook)
        {
            this.workbook = workbook;
            Title = workbook.Title;

            string seite = ((Span)App.Current.Resources["text_seite"]).Text;

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = workbook.Pages,
                ItemTemplate = new DataTemplate(typeof(LingvoDownloadPageViewCell)),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true
            };

            listView.ItemTapped += Handle_ItemEvent;
            listView.ItemSelected += ListView_ItemSelected;

            Label errorLabel = new Label
            {
                Text = ((Span)App.Current.Resources["label_no_pages"]).Text,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalTextAlignment = TextAlignment.Center,
                IsVisible = workbook.Pages.Count == 0
            };

            listView.RefreshCommand = new Command(async () =>
            {
                Workbook[] newWorkbooks = await CloudLibraryProxy.Instance.FetchAllWorkbooks();

                workbook = newWorkbooks?.FirstOrDefault(w => w.Id.Equals(workbook.Id));

                Device.BeginInvokeOnMainThread(() =>
                {
                    errorLabel.IsVisible = workbook == null ? true : workbook.Pages.Count == 0;
                    listView.ItemsSource = workbook?.Pages;
                    listView.IsRefreshing = false;
                });
            });

            RelativeLayout contentLayout = new RelativeLayout();

            contentLayout.Children.Add(new StackLayout() { Children = { errorLabel } },
                            Constraint.RelativeToParent((p) => { return p.X; }),
                            Constraint.RelativeToParent((p) => { return p.Y; }),
                            Constraint.RelativeToParent((p) => { return p.Width; }),
                            Constraint.RelativeToParent((p) => { return p.Height; }));

            contentLayout.Children.Add(listView,
                           Constraint.RelativeToParent((p) => { return p.X; }),
                           Constraint.RelativeToParent((p) => { return p.Y; }),
                           Constraint.RelativeToParent((p) => { return p.Width; }),
                           Constraint.RelativeToParent((p) => { return p.Height; }));

            Content = contentLayout;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            Handle_ItemEvent(sender, e);
        }

        void Handle_ItemEvent(object sender, EventArgs e)
            => ((ListView)sender).SelectedItem = null;
    }
}
