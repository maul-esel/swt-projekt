﻿using Lingvo.Common.Entities;
using Lingvo.MobileApp.Templates;
using Lingvo.MobileApp.Proxies;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using Lingvo.MobileApp.Entities;

namespace Lingvo.MobileApp.Pages
{
    /// <summary>
    /// The page for downloading whole workbooks from the server.
    /// </summary>
    public partial class DownloadPage : ContentPage
    {
        public DownloadPage()
        {
			NavigationPage.SetBackButtonTitle(this, "Zurück");
            Title = ((Span)App.Current.Resources["page_title_download"]).Text;
            Icon = (FileImageSource)ImageSource.FromFile("ic_action_download.png");

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(LingvoDownloadWorkbookViewCell)),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true
            };

            listView.ItemTapped += Handle_ItemTapped;
            listView.ItemSelected += Handle_ItemSelected;

            RelativeLayout contentLayout = new RelativeLayout();

            Label errorLabel = new Label
            {
                Text = ((Span)App.Current.Resources["label_sync_error"]).Text,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap,
                IsVisible = false
            };

            contentLayout.Children.Add(new StackLayout() { Children = { errorLabel } },
                Constraint.RelativeToParent((p) => { return p.X; }),
                Constraint.RelativeToParent((p) => { return p.Y; }),
                Constraint.RelativeToParent((p) => { return p.Width; }),
                Constraint.RelativeToParent((p) => { return p.Height; }));

            contentLayout.Children.Add(listView, Constraint.RelativeToParent((p) => { return p.X; }),
                           Constraint.RelativeToParent((p) => { return p.Y; }),
                           Constraint.RelativeToParent((p) => { return p.Width; }),
                           Constraint.RelativeToParent((p) => { return p.Height; }));

            Content = contentLayout;

            listView.RefreshCommand = new Command(async () =>
            {
                Device.BeginInvokeOnMainThread(() => listView.IsRefreshing = true);

                Workbook[] newWorkbooks = await CloudLibraryProxy.Instance.FetchAllWorkbooks();

                Device.BeginInvokeOnMainThread(() =>
                {
                    errorLabel.IsVisible = newWorkbooks == null ? true : newWorkbooks.Length == 0;
                    listView.ItemsSource = newWorkbooks;
                    listView.IsRefreshing = false;
                });
            });

            listView.RefreshCommand.Execute(null);
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            => ((ListView)sender).SelectedItem = null;

        /// <summary>
        /// Opens the workbook and shows its pages in a <see cref="DownloadPagesPage"/>.
        /// </summary>
        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await App.Current.MainPage.Navigation.PushAsync(new DownloadPagesPage((Workbook)e.SelectedItem));

            ((ListView)sender).SelectedItem = null;
        }
    }
}
