using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Entities;
using Xamarin.Forms;
using System.Collections.Generic;
using Lingvo.MobileApp.Pages;

namespace Lingvo.MobileApp.Templates
{
    class LingvoTeacherMemoViewCell : ViewCell
    {
        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        private MenuItem deleteAction, editAction;

        public LingvoTeacherMemoViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            ProgressView = new LingvoAudioProgressView()
            {
                Size = Device.OnPlatform(iOS: 50, Android: 120, WinPhone: 240),
                LabelType = LingvoAudioProgressView.LabelTypeValue.None,
                TextSize = 15
            };

            LocalCollection.Instance.TeacherMemoChanged += Event_TeacherMemoChanged;

            deleteAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_delete"]).Text,
                Icon = "ic_delete.png",
                IsDestructive = true
            };

            editAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_edit"]).Text,
                Icon = "ic_edit.png"
            };

            ContextActions.Add(deleteAction);
            ContextActions.Add(editAction);

            titleLabel.SetBinding(Label.TextProperty, "Name");

            deleteAction.Clicked += (o, e) =>
            {
                TeacherMemo memo = (TeacherMemo)BindingContext;
                LocalCollection.Instance.DeleteTeacherMemo(memo);
            };

            editAction.Clicked += async (o, e) =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new EditTeacherMemoPage((TeacherMemo)BindingContext));
            };

            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
                HeightRequest = Device.OnPlatform(iOS: 60, Android: 72, WinPhone: 240),
                Orientation = StackOrientation.Horizontal,
                Children =
                                {
                                    ProgressView,
                                    titleLabel
                                }

            };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            TeacherMemo memo = (TeacherMemo)BindingContext;

            ProgressView.OuterProgressColor = (Color)App.Current.Resources["primaryColor"];
            ProgressView.InnerProgressEnabled = memo.StudentTrack != null;
            ProgressView.InnerProgressColor = Color.Red;
            ProgressView.Progress = 1;
            ProgressView.MaxProgress = 1;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.None;
            ProgressView.MuteEnabled = false;
        }

        private void Event_TeacherMemoChanged(TeacherMemo t)
        {
            TeacherMemo memo = (TeacherMemo)BindingContext;
            if (t.Id.Equals(memo.Id))
            {
                TeacherMemo local = new List<TeacherMemo>(LocalCollection.Instance.TeacherMemos).Find(tm => tm.Id.Equals(t.Id));

                BindingContext = local != null ? local : t;
            }
        }
    }
}
