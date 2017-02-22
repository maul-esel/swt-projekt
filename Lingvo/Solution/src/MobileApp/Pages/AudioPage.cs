using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.Controllers;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Util;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    class AudioPage : ContentPage
    {
        private static readonly int PageButtonSize = Device.OnPlatform(iOS: 45, Android: 45, WinPhone: 50);
        private static readonly int SeekButtonSize = Device.OnPlatform(iOS: 60, Android: 75, WinPhone: 110);
        private static readonly int ControlButtonSize = Device.OnPlatform(iOS: 75, Android: 86, WinPhone: 150);

        private static readonly int SeekTimeStep = 5;

        private IExercise exercisable;
        private Workbook workbook;
        private Label Label;
        private LingvoRoundImageButton PreviousPageButton;
        private LingvoRoundImageButton NextPageButton;

        public IExercise Exercisable
        {
            get { return exercisable; }
            internal set
            {
                exercisable = value;

                Label.Text = getLabelString();
                StudentAudioController.Instance.Exercisable = exercisable;
                Refresh();
            }
        }

        private string getLabelString()
        {
            if (Exercisable is TeacherMemo)
            {
                return ((TeacherMemo)Exercisable).Name;
            }
            else if (workbook != null)
            {
                return ((Span)App.Current.Resources["text_seite"]).Text + " " + ((IPage)Exercisable).Number;
            }
            return null;
        }

        private void RedrawProgressBar(int elapsedTime)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                ProgressView.Progress = elapsedTime;
            });

        }

        /// <summary>
        /// Sets the state of the buttons according to.
        /// </summary>
        /// <param name="state">State.</param>
        private void SetButtonsAccordingToState(PlayerState state)
        {
            if (state == PlayerState.PLAYING)
            {
                PlayPauseButton.Image = LingvoRoundImageButton.PauseImage;
                RecordStopButton.Image = LingvoRoundImageButton.StopImage;
                SetSeekingButtonsAccordingly();
                return;
            }
            else
            if (state == PlayerState.PAUSED)
            {
                PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
                RecordStopButton.Image = LingvoRoundImageButton.StopImage;
                SetSeekingButtonsAccordingly();
                return;
            }
            else
            if (state == PlayerState.STOPPED || state == PlayerState.IDLE)
            {
                PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
                RecordStopButton.Image = LingvoRoundImageButton.RecordImage;
                ForwardButton.IsEnabled = RewindButton.IsEnabled = false;
                PlayPauseButton.IsEnabled = true;
                return;
            }
        }

        private void SetSeekingButtonsAccordingly()
        {
            RecorderState recorderState = StudentAudioController.Instance.CurrentRecorderState;

            if (recorderState == RecorderState.RECORDING)
            {
                ForwardButton.IsEnabled = RewindButton.IsEnabled = false;

                if (Device.OS == TargetPlatform.Android)
                {
                    PlayPauseButton.IsEnabled = false;
                }
            }
            else
            {
                ForwardButton.IsEnabled = RewindButton.IsEnabled = true;

                if (Device.OS == TargetPlatform.Android)
                {
                    PlayPauseButton.IsEnabled = true;
                }
            }
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

        public AudioPage(TeacherMemo memo) : this((IExercise)memo)
        {
            Title = memo.Name;
            Exercisable = memo;
        }

        public AudioPage(IPage page, Workbook workbook) : this(page)
        {
            Title = workbook.Title;
            this.workbook = workbook;
            Exercisable = page;
        }

        private AudioPage(IExercise exercisable)
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
                ColumnSpacing = 10,
                Padding = new Thickness(-10, 0)
            };

            ProgressView = new LingvoAudioProgressView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                OuterProgressColor = (Color)App.Current.Resources["primaryColor"],
                InnerProgressColor = Color.Red,
                InnerProgressEnabled = exercisable.StudentTrack != null,
                MuteEnabled = exercisable.StudentTrack != null,
                TextSize = 54,
                AutomationId = "ProgressView",
                MaxProgress = 95000
            };

            if (exercisable.TeacherTrack != null)
            {
                ProgressView.MaxProgress = exercisable.TeacherTrack.Duration;
            }



            RewindButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RewindImage,
                Text = " " + SeekTimeStep,
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 2 * SeekButtonSize),
                HeightRequest = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 2 * SeekButtonSize),
                IsEnabled = false,
                VerticalOptions = LayoutOptions.Start,
                AutomationId = "RewindButton"
            };

            ForwardButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.ForwardImage,
                Text = SeekTimeStep + " ",
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 2 * SeekButtonSize),
                HeightRequest = Device.OnPlatform(iOS: SeekButtonSize, Android: SeekButtonSize, WinPhone: 2 * SeekButtonSize),
                IsEnabled = false,
                VerticalOptions = LayoutOptions.Start,
                AutomationId = "ForwardButton"
            };

            PlayPauseButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.PlayImage,
                Color = (Color)App.Current.Resources["primaryColor"],
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.End,
                AutomationId = "PlayPauseButton"
            };

            RecordStopButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RecordImage,
                Color = Color.Red,
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.End,
                AutomationId = "RecordStopButton"
            };

            buttonGrid.Children.Add(RewindButton, 0, 0);
            buttonGrid.Children.Add(ForwardButton, 3, 0);
            buttonGrid.Children.Add(PlayPauseButton, 1, 0);
            buttonGrid.Children.Add(RecordStopButton, 2, 0);

            RewindButton.OnClicked += RewindButton_OnClicked;
            ForwardButton.OnClicked += ForwardButton_OnClicked;
            PlayPauseButton.OnClicked += PlayPauseButton_OnClicked;
            RecordStopButton.OnClicked += RecordStopButton_OnClicked;

            NextPageButton = new LingvoRoundImageButton
            {
                Image = "ic_next.png",
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = Device.OnPlatform(iOS: PageButtonSize, Android: PageButtonSize, WinPhone: 2 * PageButtonSize),
                HeightRequest = Device.OnPlatform(iOS: PageButtonSize, Android: PageButtonSize, WinPhone: 2 * PageButtonSize),
                Color = (Color)App.Current.Resources["primaryColor"],
                Border = true,
                IsEnabled = workbook != null && exercisable is IPage && workbook.Pages.IndexOf((IPage)exercisable) < workbook.Pages.Count - 1
            };

            PreviousPageButton = new LingvoRoundImageButton
            {
                Image = "ic_previous.png",
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = Device.OnPlatform(iOS: PageButtonSize, Android: PageButtonSize, WinPhone: 2 * PageButtonSize),
                HeightRequest = Device.OnPlatform(iOS: PageButtonSize, Android: PageButtonSize, WinPhone: 2 * PageButtonSize),
                Color = (Color)App.Current.Resources["primaryColor"],
                Border = true,
                IsEnabled = workbook != null && exercisable is IPage && workbook.Pages.IndexOf((IPage)exercisable) > 0
            };

            NextPageButton.OnClicked += NextPageButton_OnClicked;
            PreviousPageButton.OnClicked += PreviousPageButton_OnClicked;

            Label = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                BindingContext = this
            };

            Content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(15, 25),
                Children = {
                        new StackLayout()
                        {
                            Padding =  new Thickness(0, -10),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Start,
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                PreviousPageButton,
                                Label,
                                NextPageButton
                            }
                        },
                        ProgressView,
                        buttonGrid
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Exercisable is IPage)
            {
                LocalCollection.Instance.PageChanged += Event_PageChanged;
            }
            else
            {
                LocalCollection.Instance.TeacherMemoChanged += Event_TeacherMemoChanged;
            }

            ProgressView.StudentTrackMuted += ProgressView_StudentTrackMuted;

            StudentAudioController.Instance.Update += RedrawProgressBar;
            StudentAudioController.Instance.StateChange += SetButtonsAccordingToState;
            StudentAudioController.Instance.Error += OnError;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (Exercisable is IPage)
            {
                LocalCollection.Instance.PageChanged -= Event_PageChanged;
            }
            else
            {
                LocalCollection.Instance.TeacherMemoChanged -= Event_TeacherMemoChanged;
            }

            ProgressView.StudentTrackMuted -= ProgressView_StudentTrackMuted;

            StudentAudioController.Instance.Update -= RedrawProgressBar;
            StudentAudioController.Instance.StateChange -= SetButtonsAccordingToState;
            StudentAudioController.Instance.Error -= OnError;

            PlayerState currentState = StudentAudioController.Instance.CurrentPlayerState;
            if (currentState == PlayerState.PAUSED || currentState == PlayerState.PLAYING)
            {
                StudentAudioController.Instance.Stop();
                RedrawProgressBar(0);
            }
        }

        private void Event_TeacherMemoChanged(TeacherMemo t)
        {
            if (exercisable is TeacherMemo && t.Id.Equals(exercisable.Id))
            {
                Refresh();
            }
        }

        private void Event_PageChanged(IPage p)
        {
            if (exercisable is IPage && p.Id.Equals(exercisable.Id))
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ProgressView.InnerProgressEnabled = exercisable.StudentTrack != null;
                ProgressView.MaxProgress = Exercisable.TeacherTrack.Duration;
                ProgressView.MuteEnabled = exercisable.StudentTrack != null;
                NextPageButton.IsVisible = exercisable is IPage;
                PreviousPageButton.IsVisible = exercisable is IPage;

                int index = exercisable is IPage ? workbook.Pages.FindIndex(p => p.Id.Equals(exercisable.Id)) : -1;
                NextPageButton.IsEnabled = workbook != null && NextPageButton.IsVisible && index < workbook.Pages.Count - 1 && index >= -1;
                PreviousPageButton.IsEnabled = workbook != null && PreviousPageButton.IsVisible && index > 0;
            });
        }

        private void PreviousPageButton_OnClicked(object sender, EventArgs e)
        {
            int nextIndex = workbook.Pages.IndexOf((IPage)Exercisable) - 1;
            if (nextIndex >= 0)
            {
                SwitchPage(nextIndex);
            }
        }

        private void NextPageButton_OnClicked(object sender, EventArgs e)
        {
            int nextIndex = workbook.Pages.IndexOf((IPage)Exercisable) + 1;
            if (nextIndex < workbook.Pages.Count)
            {
                SwitchPage(nextIndex);
            }
        }

        private void SwitchPage(int nextIndex)
        {
            PlayerState currentState = StudentAudioController.Instance.CurrentPlayerState;
            if (currentState != PlayerState.STOPPED)
            {
                StudentAudioController.Instance.Stop();
                RedrawProgressBar(0); //Progess & time code be reset if the user triggered it theirselves
            }

            workbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(workbook.Id));

            NextPageButton.IsEnabled = nextIndex + 1 < workbook.Pages.Count;
            PreviousPageButton.IsEnabled = nextIndex > 0;
            Exercisable = workbook.Pages[nextIndex];
        }

        private void ProgressView_StudentTrackMuted(bool muted)
        {
            ProgressView.InnerProgressEnabled = !muted;
            StudentAudioController.Instance.IsMuted = muted;
        }

        private async void RecordStopButton_OnClicked(object sender, EventArgs e)
        {
            PlayerState currentState = StudentAudioController.Instance.CurrentPlayerState;

            if (currentState == PlayerState.STOPPED)
            {
                if (exercisable.StudentTrack != null)
                {
                    if (!await AlertHelper.DisplayWarningStudentTrackExists())
                    {
                        return;
                    }
                }

                StudentAudioController.Instance.StartStudentRecording();
                ProgressView.MuteEnabled = false;
                ProgressView.InnerProgressEnabled = true;

                return;
            }
            if (currentState == PlayerState.PAUSED || currentState == PlayerState.PLAYING)
            {
                StudentAudioController.Instance.Stop();
                RedrawProgressBar(0); //Progess & time code be reset if the user triggered it theirselves
                return;
            }
        }

        private async void OnError()
        {
            RedrawProgressBar(0);
            SetButtonsAccordingToState(PlayerState.STOPPED);
            await AlertHelper.DisplayAudioError();
        }

        private void PlayPauseButton_OnClicked(object sender, EventArgs e)
        {
            PlayerState currentState = StudentAudioController.Instance.CurrentPlayerState;
            RecorderState currentRecorderState = StudentAudioController.Instance.CurrentRecorderState;

            if (currentState == PlayerState.PAUSED || currentState == PlayerState.STOPPED)
            {
                if (currentRecorderState == RecorderState.PAUSED)
                {
                    StudentAudioController.Instance.Continue();
                    return;
                }

                StudentAudioController.Instance.PlayPage();
                return;
            }
            if (currentState == PlayerState.PLAYING)
            {
                StudentAudioController.Instance.Pause();
                return;
            }
        }

        private void ForwardButton_OnClicked(object sender, EventArgs e)
        {
            StudentAudioController.Instance.SeekTo(SeekTimeStep);
        }

        private void RewindButton_OnClicked(object sender, EventArgs e)
        {
            StudentAudioController.Instance.SeekTo(-SeekTimeStep);
        }
    }
}