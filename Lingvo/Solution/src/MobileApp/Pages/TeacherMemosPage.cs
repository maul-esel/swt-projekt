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
    /// <summary>
    /// The page for displaying all recorded teacher memos.
    /// </summary>
    public partial class TeacherMemosPage : ContentPage
    {
        private ToolbarItem item;
        private ListView listView;

        public TeacherMemosPage(Xamarin.Forms.Page parentPage)
        {
            Title = ((Span)App.Current.Resources["page_title_teacherMemo"]).Text;
			NavigationPage.SetBackButtonTitle(this, "Zurück");
            Icon = (FileImageSource)ImageSource.FromFile("ic_action_mic.png");

            item = new ToolbarItem
            {
                Text = "New...",
                Icon = "ic_action_add.png",
                AutomationId = "New..."
            };

            item.Clicked += AddNewClicked;

            listView = new ListView(ListViewCachingStrategy.RecycleElement)
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
                TeacherMemo[] newSource = LocalCollection.Instance.TeacherMemos.ToArray();
                listView.ItemsSource = newSource;
                errorLabel.IsVisible = newSource.Length == 0;
                listView.IsVisible = newSource.Length > 0;
            }));

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
                LingvoRoundImageButton fab = new LingvoRoundImageButton()
                {
                    Image = "ic_action_add.png",
                    WidthRequest = Device.OnPlatform(iOS: 56, Android: 56, WinPhone: 56),
                    HeightRequest = Device.OnPlatform(iOS: 56, Android: 56, WinPhone: 56),
                    Color = (Color)App.Current.Resources["secondaryColor"],
                    Border = true,
                    Text = "",
                    Filled = true,
                    AutomationId = "New..."
                };
                fab.OnClicked += AddNewClicked;

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

        /// <summary>
        /// Occurs when a teacher memo has changed.
        /// Refreshes the list.
        /// </summary>
        /// <param name="memo">The teacher memo which changed.</param>
        private void OnTeacherMemoChanged(TeacherMemo memo)
        {
            listView.RefreshCommand.Execute(null);
        }

        /// <summary>
        /// Called when the page appears on screen.
        /// Registers all important events and refreshes the list.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LocalCollection.Instance.TeacherMemoChanged += OnTeacherMemoChanged;
            listView.RefreshCommand.Execute(null);
        }

        /// <summary>
        /// Called when the page disappears on screen.
        /// Unregisters the events registered in <see cref="OnAppearing"/>.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            LocalCollection.Instance.TeacherMemoChanged -= OnTeacherMemoChanged;
        }

        /// <summary>
        /// Adds or removes the toolbar item for recording a new teacher memo, according to the given page state.
        /// Is called on iOS only, as Android has a floating action button.
        /// </summary>
        /// <param name="isActive">The state of the page.</param>
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

        /// <summary>
        /// Opens the <see cref="EditTeacherMemoPage"/> to record a new teacher memo.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The clicked <c>EventArgs</c></param>
        async void AddNewClicked(object sender, EventArgs e)
        {

			await App.Current.MainPage.Navigation.PushAsync(new EditTeacherMemoPage());
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            => ((ListView)sender).SelectedItem = null;

        /// <summary>
        /// Opens the selected teacher memo in an <see cref="AudioPage"/>.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The <c>SelectedItemChangedEventArgs</c> of the event.</param>
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
