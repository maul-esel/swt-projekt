using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Templates;
using MobileApp.Entities;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public partial class TeacherMemosPage : ContentPage
    {
        private Command addNewCommand = new Command(new Action(() => { Console.WriteLine("New teachermemo"); }));

        private ToolbarItem item;

        public TeacherMemosPage(Xamarin.Forms.Page parentPage)
        {
            Title = ((Span)App.Current.Resources["page_title_teacherMemo"]).Text;
            Icon = (FileImageSource)ImageSource.FromFile("ic_action_mic.png");

            item = new ToolbarItem
            {
                Text = "New..",
                Icon = "ic_action_add.png",
                Command = addNewCommand
            };

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = LocalCollection.GetInstance().TeacherMemos,
                ItemTemplate = new LingvoViewCellTemplate("Name"),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true,
                IsVisible = LocalCollection.GetInstance().TeacherMemos.Count > 0
            };

            listView.ItemTapped += Handle_ItemTapped;
            listView.ItemSelected += Handle_ItemSelected;

            RelativeLayout ContentLayout = new RelativeLayout();
            StackLayout innerLayout = new StackLayout
            {
                Children = {
                listView,
                new Label
                {
                    Text = ((Span)App.Current.Resources["label_no_teacher_memos"]).Text,
                    TextColor = (Color)App.Current.Resources["primaryColor"],
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    HorizontalOptions=LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    LineBreakMode = LineBreakMode.WordWrap,
                    HorizontalTextAlignment = TextAlignment.Center,
                    IsVisible=LocalCollection.GetInstance().TeacherMemos.Count == 0

                }
                }
            };
            ContentLayout.Children.Add(innerLayout, Constraint.RelativeToParent((parent) => { return parent.X; }),
                Constraint.RelativeToParent((parent) => { return parent.Y; }), Constraint.RelativeToParent((parent) => { return parent.Width; }),
                Constraint.RelativeToParent((parent) => { return parent.Height; }));

            if (Device.OS == TargetPlatform.Android)
            {
                LingvoFloatingActionButton fab = new LingvoFloatingActionButton();
                fab.FabClicked += (o, e) => addNewCommand.Execute(null);
                ContentLayout.Children.Add(fab, Constraint.RelativeToParent((parent) => { return parent.Width - parent.X - 1.5 * fab.WidthRequest; }),
                Constraint.RelativeToParent((parent) => { return parent.Height - parent.Y - 1.5 * fab.HeightRequest; }),
                Constraint.RelativeToParent((parent) => { return fab.WidthRequest; }),
                Constraint.RelativeToParent((parent) => { return fab.HeightRequest; }));
            }
            else if (Device.OS == TargetPlatform.iOS)
            {
                if (parentPage is TabbedPage)
                {
                    ((TabbedPage)parentPage).CurrentPageChanged += (o, e) =>
                    PageStateChanged(((TabbedPage)parentPage).CurrentPage is TeacherMemosPage);
                }
                else
                {
                    PageStateChanged(true);
                }
            }

            Content = ContentLayout;
        }

        public void PageStateChanged(bool isActive)
        {
            if (!isActive)
            {
                ToolbarItems.Remove(item);
            }
            else
            {
                ToolbarItems.Add(item);
            }
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            => ((ListView)sender).SelectedItem = null;

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await DisplayAlert("Selected", ((Workbook)e.SelectedItem).Title, "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
