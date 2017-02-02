using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Templates;
using Lingvo.MobileApp.Entities;
using System;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Lingvo.MobileApp.Pages
{
    public partial class TeacherMemosPage : ContentPage
    {
        private ToolbarItem item;

        public TeacherMemosPage(Xamarin.Forms.Page parentPage)
        {
            Title = ((Span)App.Current.Resources["page_title_teacherMemo"]).Text;
            Icon = (FileImageSource)ImageSource.FromFile("ic_action_mic.png");

            item = new ToolbarItem
            {
                Text = "New..",
                Icon = "ic_action_add.png"
            };

            item.Clicked += AddNewClicked;

            ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = LocalCollection.Instance.TeacherMemos,
                ItemTemplate = new DataTemplate(typeof(LingvoTeacherMemoViewCell)),
                HasUnevenRows = true,
                IsVisible = LocalCollection.Instance.TeacherMemos.Count() > 0
            };

            listView.ItemTapped += Handle_ItemTapped;
            listView.ItemSelected += Handle_ItemSelected;

            Label errorLabel = new Label
            {
                Text = ((Span)App.Current.Resources["label_no_teacher_memos"]).Text,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalTextAlignment = TextAlignment.Center,
                IsVisible = LocalCollection.Instance.TeacherMemos.Count() == 0
            };

            listView.RefreshCommand = new Command(() => Device.BeginInvokeOnMainThread(() =>
            {
                List<TeacherMemo> newSource = new List<TeacherMemo>(LocalCollection.Instance.TeacherMemos);
                listView.ItemsSource = newSource;
                errorLabel.IsVisible = newSource.Count == 0;
                listView.IsVisible = newSource.Count > 0;
            }));

            LocalCollection.Instance.TeacherMemoChanged += (t) => listView.RefreshCommand.Execute(null);

            RelativeLayout ContentLayout = new RelativeLayout();
            StackLayout innerLayout = new StackLayout
            {
                Children = {
                listView,
                errorLabel
                }
            };
            ContentLayout.Children.Add(innerLayout, Constraint.RelativeToParent((parent) => { return parent.X; }),
                Constraint.RelativeToParent((parent) => { return parent.Y; }), Constraint.RelativeToParent((parent) => { return parent.Width; }),
                Constraint.RelativeToParent((parent) => { return parent.Height; }));

            if (Device.OS == TargetPlatform.Android)
            {
                LingvoFloatingActionButton fab = new LingvoFloatingActionButton();
                fab.FabClicked += AddNewClicked;
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

        async void AddNewClicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new EditTeacherMemoPage());
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            => ((ListView)sender).SelectedItem = null;

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await App.Current.MainPage.Navigation.PushAsync(new AudioPage((TeacherMemo)e.SelectedItem));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
