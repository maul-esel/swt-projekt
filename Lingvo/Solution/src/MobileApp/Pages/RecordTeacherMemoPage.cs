using Lingvo.Common.Enums;
using Lingvo.MobileApp.Controllers;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections;

namespace Lingvo.MobileApp.Pages
{
    class RecordTeacherMemoPage : ContentPage
    {

        private static readonly int ControlButtonSize = Device.OnPlatform(iOS: 75, Android: 86, WinPhone: 150);

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

        public RecordTeacherMemoPage() : base()
        {
            Title = ((Span)App.Current.Resources["page_title_recordTeacherMemo"]).Text;

            ToolbarItem saveItem = new ToolbarItem
            {
                Text = ((Span)App.Current.Resources["label_save"]).Text,
                Icon = "ic_action_tick.png"
            };

            saveItem.Clicked += SaveItem_Clicked;

            ToolbarItems.Add(saveItem);

            RecordButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RecordImage,
                Color = Color.Red,
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
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

            TeacherMemoController.Instance.Update += Progress_Update;

            Name = new Entry()
            {
                Placeholder = "Name der Lehrerspur",
                PlaceholderColor = Color.Gray,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Entry))
            };

            Content = new StackLayout()
            {
                Padding = new Thickness(15, 40),
                Children =
                {
                Name,
                    Label,
                    RecordButton
                }
            };
        }

        private void Progress_Update(int progress)
        {
            int minutes = progress / 60;
            int seconds = progress % 60;
            string minuteString = (minutes < 10 ? "0" + minutes : "" + minutes);
            string secondString = (seconds < 10 ? "0" + seconds : "" + seconds);
            Device.BeginInvokeOnMainThread(() => Label.Text = minuteString + ":" + secondString);
        }

        private async void SaveItem_Clicked(object sender, EventArgs e)
        {
            if (TeacherMemoController.Instance.CurrentMemo != null && Name.Text.Length > 0)
            {
                List<TeacherMemo> memos = new List<TeacherMemo>(LocalCollection.Instance.TeacherMemos);
                if (memos.Find(m => m.Name.Equals(Name.Text)) != null)
                {
                    string title = ((Span)App.Current.Resources["label_nameAlreadyExists"]).Text;
                    string desc = ((Span)App.Current.Resources["desc_teacherTrackNameAlreadyExists"]).Text;
                    await DisplayAlert(title, desc, "Ok");
                }
                else
                {
                    TeacherMemoController.Instance.SaveTeacherMemo(Name.Text);
                    await Navigation.PopAsync();
                }
            }
        }

        private async void RecordButton_OnClicked(object sender, EventArgs e)
        {
            RecorderState currentState = TeacherMemoController.Instance.State;
            if (currentState != RecorderState.RECORDING)
            {
                if (TeacherMemoController.Instance.CurrentMemo != null)
                {
                    if (!await DisplayAlert("Overwrite", "Overwrite?", "Yeah", "Nope"))
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
                RecordButton.Image = LingvoRoundImageButton.RecordImage;
                return;
            }
        }
    }
}
