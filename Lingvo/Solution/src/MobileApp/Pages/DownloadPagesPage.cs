using Lingvo.Common.Entities;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Templates;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public partial class DownloadPagesPage : ContentPage
    {
        private Workbook workbook;

        private Command downloadAction;

        public DownloadPagesPage(Workbook workbook)
        {
            this.workbook = workbook;
            Title = workbook.Title;

            downloadAction = new Command(async (param) =>
            {
                PageProxy page = (PageProxy)workbook.Pages.Find((p) => p.Number == (int)param);
                await page?.Resolve();
            });

            string seite = ((Span)App.Current.Resources["text_seite"]).Text;

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = workbook.Pages,
                ItemTemplate = new LingvoDownloadViewCellTemplate("Number", "Description", "Number", downloadAction, seite + " {0}", null),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true,
                IsVisible = workbook.Pages.Count > 0
            };

            listView.ItemTapped += Handle_ItemEvent;
            listView.ItemSelected += ListView_ItemSelected;

            Label errorLabel = new Label
            {
                Text = ((Span)App.Current.Resources["label_sync_error"]).Text,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalTextAlignment = TextAlignment.Center,
                IsVisible = workbook.Pages.Count == 0
            };

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
            downloadAction.Execute(((IPage)e.SelectedItem).Number);
            Handle_ItemEvent(sender, e);
        }

        void Handle_ItemEvent(object sender, EventArgs e)
            => ((ListView)sender).SelectedItem = null;
    }
}
