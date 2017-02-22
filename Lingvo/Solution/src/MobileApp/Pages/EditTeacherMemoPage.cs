using Lingvo.Common.Enums;
using Lingvo.MobileApp.Controllers;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
using Lingvo.MobileApp.Util;

namespace Lingvo.MobileApp.Pages
{
    /// <summary>
    /// The page to record a new teacher memo or edit its name.
    /// </summary>
    class EditTeacherMemoPage : ContentPage
    {

        private static readonly int ControlButtonSize = Device.OnPlatform(iOS: 75, Android: 86, WinPhone: 150);
        private static readonly int EditButtonSize = Device.OnPlatform(iOS: 25, Android: 35, WinPhone: 50);

        private LingvoRoundImageButton RecordButton
        {
            get; set;
        }

        private Label Label
        {
            get; set;
        }

        private Entry Name
        {
            get; set;
        }
        private Label NameLabel
        {
            get; set;
        }

        /// <summary>
        /// The button for editing the name of an existing teacher memo.
        /// </summary>
        private LingvoRoundImageButton EditButton
        {
            get; set;
        }

        /// <summary>
        /// The toolbar item for saving
        /// </summary>
        private ToolbarItem SaveItem
        {
            get; set;
        }

        public EditTeacherMemoPage(TeacherMemo memo) : this()
        {
            NameLabel.IsVisible = true;
            Name.Text = memo.Name;
            Name.IsVisible = false;
            EditButton.IsVisible = true;
            NameLabel.Text = memo.Name;
            ToolbarItems.Clear();
            RecordButton.IsEnabled = false;
            Progress_Update(memo.TeacherTrack.Duration);
            SaveItem.Clicked -= SaveItem_Clicked;
            SaveItem.Clicked += async (o, e) =>
            {
                if (Name.Text.Length > 0 && !await CheckNameExists(Name.Text))
                {
                    memo.Name = Name.Text;
                    App.Database.Save(memo);
                    await Navigation.PopAsync();
                }
            };
        }

        public EditTeacherMemoPage() : base()
        {
            Title = ((Span)App.Current.Resources["page_title_recordTeacherMemo"]).Text;

            TeacherMemoController.Instance.Reset();

            SaveItem = new ToolbarItem
            {
                Text = ((Span)App.Current.Resources["label_save"]).Text,
                Icon = "ic_action_tick.png"
            };

            SaveItem.Clicked += SaveItem_Clicked;

            ToolbarItems.Add(SaveItem);

            RecordButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RecordImage,
                Color = Color.Red,
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                AutomationId = "RecordButton"
            };

            RecordButton.OnClicked += RecordButton_OnClicked;

            Label = new Label()
            {
                Text = "00:00",
                FontSize = Device.OnPlatform(iOS: 80, Android: 100, WinPhone: 200),
                TextColor = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Name = new Entry()
            {
                Placeholder = ((Span)App.Current.Resources["desc_teacherMemoName"]).Text,
                PlaceholderColor = Color.Gray,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Entry)),
                AutomationId = "NameEntry"
            };

            NameLabel = new Label()
            {
                TextColor = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Entry)),
                IsVisible = false,
                AutomationId = "NameLabel"
            };

            EditButton = new LingvoRoundImageButton()
            {
                Border = false,
                Image = "ic_edit.png",
                Color = (Color)App.Current.Resources["primaryColor"],
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = EditButtonSize,
                HeightRequest = EditButtonSize,
                IsVisible = false,
                AutomationId = "EditButton"
            };

            EditButton.OnClicked += EditButton_OnClicked;

            Content = new StackLayout()
            {
                Padding = new Thickness(15, 40),
                Children =
                {
                    new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.StartAndExpand,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            Name,
                            NameLabel,
                            EditButton
                        }
                    },
                    Label,
                    RecordButton
                }
            };
        }

        /// <summary>
        /// Registers all important events to actualize the views 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            TeacherMemoController.Instance.Update += Progress_Update;
            TeacherMemoController.Instance.Error += OnError;
        }

        /// <summary>
        /// Unregisters all events registered in <see cref="OnAppearing"/>.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            TeacherMemoController.Instance.Update -= Progress_Update;
            TeacherMemoController.Instance.Error -= OnError;
        }

        /// <summary>
        /// Called after an exception in <see cref="TeacherMemoController"/>.
        /// Updates the views and displays an error message.
        /// </summary>
        private async void OnError()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                RecordButton.Image = LingvoRoundImageButton.RecordImage;
                Progress_Update(0);
            });
            await AlertHelper.DisplayAudioError();
        }

        /// <summary>
        /// Enables text editing of the name.
        /// Occurs after clicking the <see cref="EditButton"/>.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The click <c>EventArgs</c>.</param>
        private void EditButton_OnClicked(object sender, EventArgs e)
        {
            NameLabel.IsVisible = false;
            Name.IsVisible = true;
            EditButton.IsEnabled = false;
            ToolbarItems.Add(SaveItem);
        }

        /// <summary>
        /// Updates the progress view.
        /// </summary>
        /// <param name="progress">The new progress.</param>
        private void Progress_Update(int progress)
        {
            int minutes = progress / 60000;
            int seconds = Math.Abs(progress / 1000) % 60;
            string minuteString = (Math.Abs(minutes) < 10 ? "0" + minutes : "" + minutes);
            string secondString = (seconds < 10 ? "0" + seconds : "" + seconds);
            Device.BeginInvokeOnMainThread(() => Label.Text = minuteString + ":" + secondString);
        }

        /// <summary>
        /// Checks if a teacher memo with the given name already exists.
        /// </summary>
        /// <param name="name">The potential name to be checked.</param>
        /// <returns><c>True</c> if the name already exists, <c>false</c> otherwise.</returns>
        private async Task<bool> CheckNameExists(string name)
        {
            if (LocalCollection.Instance.TeacherMemos.FirstOrDefault(m => m.Name.Equals(Name.Text)) != null)
            {
                await AlertHelper.DisplayInfoTeacherMemoNameExists();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the current teacher memo with the text of <see cref="Name"/>.
        /// If no current teacher memo has been recorded, or the name already exists or is empty,
        /// it shows an error message.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The click <c>EventArgs</c>.</param>
        private async void SaveItem_Clicked(object sender, EventArgs e)
        {
            if (Name.Text.Length > 0)
            {
                if (!await CheckNameExists(Name.Text))
                {
                    if (TeacherMemoController.Instance.CurrentMemo != null)
                    {
                        TeacherMemoController.Instance.SaveTeacherMemo(Name.Text);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await AlertHelper.DisplayNoTeacherMemoRecordingError();
                    }
                }
            }
            else
            {
                await AlertHelper.DisplayNoTeacherMemoNameError();
            }
        }

        /// <summary>
        /// Occurs when the record button or the stop button (which are basically the same button instance) are pressed.
        /// Accordingly, it starts a new recording (after displaying a warning, if a recorded teacher memo already exists) or
        /// stops the recording and sets <see cref="TeacherMemoController.CurrentMemo"/>.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The <c>EventArgs</c> of the click event.</param>
        private async void RecordButton_OnClicked(object sender, EventArgs e)
        {
            RecorderState currentState = TeacherMemoController.Instance.State;
            if (currentState != RecorderState.RECORDING)
            {
                if (TeacherMemoController.Instance.CurrentMemo != null)
                {
                    if (!await AlertHelper.DisplayWarningTeacherMemoExists())
                    {
                        return;
                    }
                }
                TeacherMemoController.Instance.StartTeacherMemo();
                RecordButton.Image = LingvoRoundImageButton.StopImage;
            }
            else
            {
                TeacherMemoController.Instance.EndTeacherMemo();
                Progress_Update(TeacherMemoController.Instance.CurrentMemo.TeacherTrack.Duration);
                RecordButton.Image = LingvoRoundImageButton.RecordImage;
                return;
            }
        }
    }
}
