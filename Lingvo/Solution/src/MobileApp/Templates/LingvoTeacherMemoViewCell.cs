using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Entities;
using Xamarin.Forms;
using System.Collections.Generic;
using Lingvo.MobileApp.Pages;
using System.Linq;
using Lingvo.MobileApp.Util;
using System;

namespace Lingvo.MobileApp.Templates
{
    class LingvoTeacherMemoViewCell : ViewCell
    {
        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        private MenuItem deleteAction, editAction, deleteStudentAction;

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

            deleteAction.Clicked += DeleteAction_Clicked;

            deleteStudentAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_delete_studentTrack"]).Text,
                Icon = "ic_mic_off.png"
            };

            deleteStudentAction.Clicked += DeleteStudentAction_Clicked;

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

        private async void DeleteStudentAction_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (await AlertHelper.DisplayWarningDeleteStudentTrack())
                {
                    TeacherMemo page = (TeacherMemo)BindingContext;
                    LocalCollection.Instance.DeleteStudentRecording(page);
                    ContextActions.Remove(deleteStudentAction);
                }
            }
            catch
            {
                Console.WriteLine("Context Actions null");
            }
        }

        private async void DeleteAction_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (await AlertHelper.DisplayWarningDeleteTeacherMemo(((TeacherMemo)BindingContext).StudentTrack != null))
                {
                    TeacherMemo memo = (TeacherMemo)BindingContext;
                    LocalCollection.Instance.DeleteTeacherMemo(memo);
                    ContextActions.Remove(deleteAction);
                }
            }
            catch
            {
                Console.WriteLine("Context Actions null");
            }
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

            if (memo.StudentTrack != null && !ContextActions.Contains(deleteStudentAction))
            {
                ContextActions.Add(deleteStudentAction);
            }
        }

        private void Event_TeacherMemoChanged(TeacherMemo t)
        {
            TeacherMemo memo = (TeacherMemo)BindingContext;
            if (t.Id.Equals(memo.Id))
            {
                TeacherMemo local = LocalCollection.Instance.TeacherMemos.FirstOrDefault(tm => tm.Id.Equals(t.Id));

                BindingContext = local != null ? local : t;
            }
        }
    }
}
