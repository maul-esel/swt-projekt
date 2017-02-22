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
    /// <summary>
    /// The ViewCell for displaying information and providing context actions of a teacher memo.
    /// </summary>
    class LingvoTeacherMemoViewCell : ViewCell
    {
        /// <summary>
        /// The progress view showing if the teacher memo has a student track.
        /// </summary>
        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        /// <summary>
        /// The context menu items for editing and deleting the teacher memo itself or the student track.
        /// </summary>
        private MenuItem deleteAction, editAction, deleteStudentAction;

        public LingvoTeacherMemoViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            ProgressView = new LingvoAudioProgressView()
            {
                Size = Device.OnPlatform(iOS: 50, Android: 72, WinPhone: 240),
                LabelType = LingvoAudioProgressView.LabelTypeValue.None,
                TextSize = 15
            };

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

            editAction.Clicked += EditAction_Clicked;

            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
                HeightRequest = Device.OnPlatform(iOS: 60, Android: 80, WinPhone: 240),
                Orientation = StackOrientation.Horizontal,
                Children =
                                {
                                    ProgressView,
                                    titleLabel
                                }

            };
        }

        /// <summary>
        /// Occurs when the edit context menu item was clicked.
        /// Opens the teacher memo for editing in <see cref="EditTeacherMemoPage"/>.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The clicked <c>EventArgs</c>.</param>
        private async void EditAction_Clicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new EditTeacherMemoPage((TeacherMemo)BindingContext));
        }

        /// <summary>
        /// Called when the view cell appears on screen.
        /// Registers all important events.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LocalCollection.Instance.TeacherMemoChanged += Event_TeacherMemoChanged;

        }

        /// <summary>
        /// Called when the view cell disappears on screen.
        /// Unregisters the events registered in <see cref="OnAppearing"/>.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            LocalCollection.Instance.TeacherMemoChanged -= Event_TeacherMemoChanged;
        }

        /// <summary>
        /// Occurs when the delete student track context menu item was clicked.
        /// Displays a warning dialog and deletes the student track of the teacher memo after positive result.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The clicked <c>EventArgs</c>.</param>
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

        /// <summary>
        /// Occurs when the delete teacher memo context menu item was clicked.
        /// Displays a warning dialog and deletes the teacher memo after positive result.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The clicked <c>EventArgs</c>.</param>
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

        /// <summary>
        /// Occurs when the <c>BindingContext</c> of the view cell has changed.
        /// Refreshes the progress view and the context actions.
        /// </summary>
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

        /// <summary>
        /// Occurs when a teacher memo has changed.
        /// Refreshes the <c>BindingContext</c> of this view, if the changed teacher memo is equal to it.
        /// </summary>
        /// <param name="t">The teacher memo which has changed.</param>
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
