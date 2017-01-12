using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    class AudioPage : ContentPage
    {
        private static readonly int SeekButtonSize = 66;
        private static readonly int ControlButtonSize = 86;

        public IPage Page
        {
            get; set;
        }

        public IPlayer Player
        {
            get; set;
        }

        public IRecorder Recorder
        {
            get; set;
        }

        internal LingvoRoundImageButton RewindButton
        {
            get; set;
        }

        internal LingvoRoundImageButton ForwardButton
        {
            get; set;
        }

        internal LingvoRoundImageButton PlayPauseButton
        {
            get; set;
        }

        internal LingvoRoundImageButton RecordStopButton
        {
            get; set;
        }

        public AudioPage(int numberOfWorkbookPages)
        {
            Grid buttonGrid = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width=new GridLength(SeekButtonSize, GridUnitType.Absolute) },
                    new ColumnDefinition() { Width=new GridLength(ControlButtonSize, GridUnitType.Absolute) },
                    new ColumnDefinition() { Width=new GridLength(ControlButtonSize, GridUnitType.Absolute) },
                    new ColumnDefinition() { Width=new GridLength(SeekButtonSize, GridUnitType.Absolute) }
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height=new GridLength(ControlButtonSize + SeekButtonSize / 2, GridUnitType.Absolute) }
                },
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.End,
                ColumnSpacing = 10
            };

            BoxView progressView = new BoxView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Color = (Color)App.Current.Resources["primaryColor"]
            };

            RewindButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RewindImage,
                Text = "15",
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 2 * SeekButtonSize),
                HeightRequest = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 2 * SeekButtonSize),
                TextOrientation = TextAlignment.End,
                IsEnabled = false,
                VerticalOptions = LayoutOptions.Start
            };

            ForwardButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.ForwardImage,
                Text = "15",
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 2 * SeekButtonSize),
                HeightRequest = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 2 * SeekButtonSize),
                TextOrientation = TextAlignment.Start,
                IsEnabled = false,
                VerticalOptions = LayoutOptions.Start
            };

            PlayPauseButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.PlayImage,
                Color = (Color)App.Current.Resources["primaryColor"],
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.End
            };

            RecordStopButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RecordImage,
                Color = Color.Red,
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.End
            };

            buttonGrid.Children.Add(RewindButton, 0, 0);
            buttonGrid.Children.Add(ForwardButton, 3, 0);
            buttonGrid.Children.Add(PlayPauseButton, 1, 0);
            buttonGrid.Children.Add(RecordStopButton, 2, 0);

            RewindButton.OnClicked += RewindButton_OnClicked;
            ForwardButton.OnClicked += ForwardButton_OnClicked;
            PlayPauseButton.OnClicked += PlayPauseButton_OnClicked;
            RecordStopButton.OnClicked += RecordStopButton_OnClicked;

            Label pageLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BindingContext = this
            };
            string seite = (string)App.Current.Resources["text_seite"];
            pageLabel.SetBinding(Label.TextProperty, "Page.Number", BindingMode.Default, null, seite + " {0} / " + numberOfWorkbookPages);

            Content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(15, 25),
                Children = {
                        pageLabel,
                        progressView,
                        buttonGrid
                }
            };
        }



        private void RecordStopButton_OnClicked(object sender, EventArgs e)
        {
            if (RecordStopButton.Image.Equals(LingvoRoundImageButton.StopImage))
            {
                PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
                ForwardButton.IsEnabled = RewindButton.IsEnabled = false;
                RecordStopButton.Image = LingvoRoundImageButton.RecordImage;
            }
        }

        private void PlayPauseButton_OnClicked(object sender, EventArgs e)
        {
            if (PlayPauseButton.Image.Equals(LingvoRoundImageButton.PlayImage))
            {
                PlayPauseButton.Image = LingvoRoundImageButton.PauseImage;
                RecordStopButton.Image = LingvoRoundImageButton.StopImage;
                ForwardButton.IsEnabled = RewindButton.IsEnabled = true;
            }
            else
            {
                PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
            }
        }

        private void ForwardButton_OnClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RewindButton_OnClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
