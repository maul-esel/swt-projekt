using Lingvo.Common.Entities;
using Lingvo.MobileApp.Templates;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public partial class DownloadPagesPage : ContentPage
    {
        private Action<object> downloadAction = new Action<object>((o) => { Console.WriteLine(o); });

        public DownloadPagesPage(Workbook workbook)
        {
            Title = workbook.Title;

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = workbook.Pages,
                ItemTemplate = new LingvoDownloadViewCellTemplate("Number", "Description", "Number", downloadAction, "Seite {0}", null),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true,
                IsVisible = workbook.Pages.Count > 0
            };

            listView.ItemTapped += Handle_ItemEvent;
            listView.ItemSelected += ListView_ItemSelected;

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
                    IsVisible = workbook.Pages.Count == 0
                }
                }
            };
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            downloadAction.Invoke(((IPage)e.SelectedItem).Number);
            Handle_ItemEvent(sender, e);
        }

        void Handle_ItemEvent(object sender, EventArgs e)
            => ((ListView)sender).SelectedItem = null;
    }
}
