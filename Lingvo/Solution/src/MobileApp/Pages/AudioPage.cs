using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.MobileApp.Controllers;
using Lingvo.MobileApp.Forms;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    class AudioPage : ContentPage
    {
        private static readonly int SeekButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);
        private static readonly int ControlButtonSize = Device.OnPlatform(iOS: 75, Android: 86, WinPhone: 150);

        private static readonly int SeekTimeStep = 15;

        public IPage Page
        {
            get; internal set;
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

        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        public AudioPage(IPage page, int numberOfWorkbookPages)
        {
            Page = page;

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

            ProgressView = new LingvoAudioProgressView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                OuterProgressColor = (Color)App.Current.Resources["primaryColor"],
                InnerProgressColor = (Color)App.Current.Resources["secondaryColor"],
                InnerProgressEnabled = page.StudentTrack != null,
                MuteEnabled = page.StudentTrack != null
            };

            if (page.TeacherTrack != null)
            {
				ProgressView.MaxProgress = page.TeacherTrack.Duration;
            }

            ProgressView.StudentTrackMuted += ProgressView_StudentTrackMuted;
			StudentPageController.Instance.Update += (elapsedTime) => ProgressView.Progress = elapsedTime;

            RewindButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RewindImage,
                Text = "" + SeekTimeStep,
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
                Text = "" + SeekTimeStep,
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
                BindingContext = this,
                Text = ((Span)App.Current.Resources["text_seite"]).Text + " " + Page.Number + " / " + numberOfWorkbookPages
            };

            Content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(15, 25),
                Children = {
                        pageLabel,
                        ProgressView,
                        buttonGrid
                }
            };
        }

        private void ProgressView_StudentTrackMuted(bool muted)
        {
            //StudentPageController.Instance.IsMuted = muted;
        }

        private void RecordStopButton_OnClicked(object sender, EventArgs e)
        {

			if (RecordStopButton.Image.Equals(LingvoRoundImageButton.RecordImage))
			{

				StudentPageController.Instance.StartStudentRecording();
				RecordStopButton.Image = LingvoRoundImageButton.StopImage;
				PlayPauseButton.Image = LingvoRoundImageButton.PauseImage;
		
			}
			else
			{
				StudentPageController.Instance.Stop();
				PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
				RecordStopButton.Image = LingvoRoundImageButton.RecordImage;

			}
            /*ForwardButton.IsEnabled = RewindButton.IsEnabled = PlayPauseButton.Toggled = false;
            if (RecordStopButton.Image.Equals(LingvoRoundImageButton.StopImage))
            {
                //StudentPageController.Instance.Stop();
                PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
                RecordStopButton.Image = LingvoRoundImageButton.RecordImage;

                if (StudentPageController.Instance.SelectedPage.StudentTrack != null false)
                {
                    ProgressView.MuteEnabled = true;
                }
                else
                {
                    ProgressView.InnerProgressEnabled = false;
                    ProgressView.MuteEnabled = false;
                }
            }
            else
            {
                if (StudentPageController.Instance.SelectedPage.StudentTrack != null false)
                {
                    //Dialog
                }
                //StudentPageController.Instance.StartStudentRecording();
                ProgressView.InnerProgressEnabled = true;
                PlayPauseButton.Image = LingvoRoundImageButton.PauseImage;
                RecordStopButton.Image = LingvoRoundImageButton.StopImage;
                ProgressView.MuteEnabled = false;
            }*/
        }

        private void PlayPauseButton_OnClicked(object sender, EventArgs e)
        {

			if (PlayPauseButton.Image.Equals(LingvoRoundImageButton.PlayImage))
			{
				StudentPageController.Instance.PlayPage();
				ForwardButton.IsEnabled = RewindButton.IsEnabled = true;
				PlayPauseButton.Image = LingvoRoundImageButton.PauseImage;
				RecordStopButton.Image = LingvoRoundImageButton.StopImage;
			}
			else //pause player
			{
				StudentPageController.Instance.Pause();
				PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
				RecordStopButton.Image = LingvoRoundImageButton.StopImage;
			}
        }

        private void ForwardButton_OnClicked(object sender, EventArgs e)
        {
            if (/*StudentPageController.Instance.IsPlaying()*/ false)
            {
                //StudentPageController.Instance.SeekTo(TimeSpan.FromSeconds(/*StudentPageController.Instance.CurrentPosition*/ +15));
            }
        }

        private void RewindButton_OnClicked(object sender, EventArgs e)
        {
            if (/*StudentPageController.Instance.IsPlaying()*/ false)
            {
                //StudentPageController.Instance.SeekTo(TimeSpan.FromSeconds(/*StudentPageController.Instance.CurrentPosition*/ -15));
            }
        }
    }
}
