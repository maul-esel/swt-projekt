using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    public sealed partial class LingvoAudioPageTemplate : DataTemplate
    {
        private static readonly int SeekButtonSize = 56;
        private static readonly int ControlButtonSize = 96;
        public LingvoAudioPageTemplate(ButtonCommandsHolder commands, Workbook workbook) : base(() =>
        {
            LingvoRoundImageButton rewindButton = new LingvoRoundImageButton()
            {
                Image = "Icon.png",
                Size = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 72)
            };
            rewindButton.SetBinding(View.BindingContextProperty, ".");
            rewindButton.OnClicked += (o, e) => commands.RewindCommand.Execute(rewindButton.BindingContext);
            
            LingvoRoundImageButton forwardButton = new LingvoRoundImageButton()
            {
                Image = "Icon.png",
                Size = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 72)
            };
            forwardButton.SetBinding(View.BindingContextProperty, ".");
            forwardButton.OnClicked += (o, e) => commands.ForwardCommand.Execute(forwardButton.BindingContext);

            LingvoRoundImageButton playPauseButton = new LingvoRoundImageButton()
            {
                Image = "Icon.png",
                Size = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 144)
            };
            forwardButton.SetBinding(View.BindingContextProperty, ".");
            playPauseButton.OnClicked += (o, e) => commands.PlayPauseCommand.Execute(forwardButton.BindingContext);

            LingvoRoundImageButton recordStopButton = new LingvoRoundImageButton()
            {
                Image = "Icon.png",
                Size = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 144)
            };
            forwardButton.SetBinding(View.BindingContextProperty, ".");
            recordStopButton.OnClicked += (o, e) => commands.RecordStopCommand.Execute(forwardButton.BindingContext);


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
                    new RowDefinition() { Height=new GridLength(SeekButtonSize, GridUnitType.Absolute) },
                    new RowDefinition() { Height=GridLength.Star }
                },
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.End
            };

            buttonGrid.Children.Add(rewindButton, 0, 0);
            buttonGrid.Children.Add(forwardButton, 3, 0);
            buttonGrid.Children.Add(playPauseButton, 1, 1);
            buttonGrid.Children.Add(recordStopButton, 2, 1);

            BoxView progressView = new BoxView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Color = (Color)App.Current.Resources["primaryColor"]
            };

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
                    Padding = new Thickness(15, 15),
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

        public class ButtonCommandsHolder
        {
            public Command<IPage> RewindCommand
            {
                get; set;
            }

            public Command<IPage> ForwardCommand
            {
                get; set;
            }

            public Command<IPage> PlayPauseCommand
            {
                get; set;
            }

            public Command<IPage> RecordStopCommand
            {
                get; set;
            }
        }
    }
}
