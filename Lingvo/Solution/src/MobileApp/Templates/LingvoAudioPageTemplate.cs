using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    public sealed partial class LingvoAudioPageTemplate : DataTemplate
    {
        private static readonly int SeekButtonSize = 66;
        private static readonly int ControlButtonSize = 86;
        public LingvoAudioPageTemplate(ButtonClickedEventHandlerHolder handler, Workbook workbook) : base(() =>
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

            LingvoRoundImageButton rewindButton = new LingvoRoundImageButton()
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
            rewindButton.SetBinding(View.BindingContextProperty, ".");
            
            LingvoRoundImageButton forwardButton = new LingvoRoundImageButton()
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
            forwardButton.SetBinding(View.BindingContextProperty, ".");
            
            LingvoRoundImageButton playPauseButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.PlayImage,
                Color = (Color)App.Current.Resources["primaryColor"],
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.End
            };
            forwardButton.SetBinding(View.BindingContextProperty, ".");
            
            LingvoRoundImageButton recordStopButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RecordImage,
                Color = Color.Red,
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.End
            };
            forwardButton.SetBinding(View.BindingContextProperty, ".");
            
            buttonGrid.Children.Add(rewindButton, 0, 0);
            buttonGrid.Children.Add(forwardButton, 3, 0);
            buttonGrid.Children.Add(playPauseButton, 1, 0);
            buttonGrid.Children.Add(recordStopButton, 2, 0);

            ButtonHolder holder = new ButtonHolder()
            {
                RewindButton = rewindButton,
                ForwardButton = forwardButton,
                PlayPauseButton = playPauseButton,
                RecordStopButton = recordStopButton
            };

            rewindButton.OnClicked += (o, e) => handler.RewindHandler?.Invoke(holder, (IPage)rewindButton.BindingContext, progressView);
            forwardButton.OnClicked += (o, e) => handler.ForwardHandler?.Invoke(holder, (IPage)forwardButton.BindingContext, progressView);
            playPauseButton.OnClicked += (o, e) => handler.PlayPauseHandler?.Invoke(holder, (IPage)playPauseButton.BindingContext, progressView);
            recordStopButton.OnClicked += (o, e) => handler.RecordStopHandler?.Invoke(holder, (IPage)recordStopButton.BindingContext, progressView);

            Label pageLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Start
            };
            pageLabel.SetBinding(Label.TextProperty, "Number", BindingMode.Default, null, "Seite {0} / " + workbook.Pages.Count);

            return new ContentPage()
            {
                Content =
                new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(15, 25),
                    Children =
                    {
                        pageLabel,
                        progressView,
                        buttonGrid
                    }
                }
            };
        })
        { }

        public delegate void AudioButtonClickedEventHandler(ButtonHolder buttons, IPage page, BoxView progress);

        public class ButtonHolder
        {
            public LingvoRoundImageButton RewindButton
            {
                get; set;
            }

            public LingvoRoundImageButton ForwardButton
            {
                get; set;
            }

            public LingvoRoundImageButton PlayPauseButton
            {
                get; set;
            }

            public LingvoRoundImageButton RecordStopButton
            {
                get; set;
            }
        }

        public class ButtonClickedEventHandlerHolder
        {
            public AudioButtonClickedEventHandler RewindHandler
            {
                get; set;
            }

            public AudioButtonClickedEventHandler ForwardHandler
            {
                get; set;
            }

            public AudioButtonClickedEventHandler PlayPauseHandler
            {
                get; set;
            }

            public AudioButtonClickedEventHandler RecordStopHandler
            {
                get; set;
            }

            public AudioButtonClickedEventHandler MuteHandler
            {
                get; set;
            }
        }
    }
}
