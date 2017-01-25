using Lingvo.Common.Enums;
using Lingvo.MobileApp.Controllers;
using Lingvo.MobileApp.Forms;
using MobileApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    class RecordTeacherMemoPage : ContentPage
    {

        private static readonly int ControlButtonSize = Device.OnPlatform(iOS: 75, Android: 86, WinPhone: 150);

        private LingvoRoundImageButton RecordButton
        {
            get; set;
        }

        private TeacherMemo CurrentMemo
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
                Text = "Save..",
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

            Name = new Entry()
            {
                Placeholder = "Name der Lehrerspur",
                PlaceholderColor = Color.Gray,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand
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

        private async void SaveItem_Clicked(object sender, EventArgs e)
        {

            if (CurrentMemo != null)
            {
                if (Name.Text.Length > 0 && LocalCollection.GetInstance().TeacherMemos.Find(m => m.Name.Equals(Name.Text)) == null)
                {

                    TeacherMemoController.Instance.SaveTeacherMemo(Name.Text, CurrentMemo);
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Name schon vergeben", "Es existiert bereits eine Lehrerspur mit diesem Namen. Bitte wählen Sie einen anderen.", "Ok");
                }
            }
        }

        private void RecordButton_OnClicked(object sender, EventArgs e)
        {
            //TODO create States in TeacherMemoController
           
            if (RecordButton.Image.Equals(LingvoRoundImageButton.RecordImage))
            {
                TeacherMemoController.Instance.StartTeacherMemo();
                RecordButton.Image = LingvoRoundImageButton.StopImage;
                return;
            }
            if (RecordButton.Image.Equals(LingvoRoundImageButton.StopImage))
            {
                CurrentMemo = TeacherMemoController.Instance.EndTeacherMemo();
                RecordButton.Image = LingvoRoundImageButton.RecordImage;
                return;
            }
        }
    }
}
