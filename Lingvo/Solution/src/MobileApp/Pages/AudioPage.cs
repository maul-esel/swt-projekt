using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.Controllers;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Forms;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    class AudioPage : ContentPage
    {
        private static readonly int PageButtonSize = Device.OnPlatform(iOS: 25, Android: 35, WinPhone: 50);
        private static readonly int SeekButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);
        private static readonly int ControlButtonSize = Device.OnPlatform(iOS: 75, Android: 86, WinPhone: 150);

        private static readonly int SeekTimeStep = 5;
        private bool isActive;

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
                return ((Span)App.Current.Resources["text_seite"]).Text + " " + ((IPage)Exercisable).Number + " / " + workbook.TotalPages;
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
            if (state == PlayerState.PAUSED)
            {
                //TODO: @Phlilip: MAAAYBEE, toggle that pause Button for God's sake :P 
                PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
                RecordStopButton.Image = LingvoRoundImageButton.StopImage;
                SetSeekingButtonsAccordingly();
                return;
            }
            if (state == PlayerState.STOPPED || state == PlayerState.IDLE)
            {
                PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
                RecordStopButton.Image = LingvoRoundImageButton.RecordImage;
                ForwardButton.IsEnabled = RewindButton.IsEnabled = false;
                return;
            }
            //no default needed...
        }

        private void SetSeekingButtonsAccordingly()
        {
            RecorderState recorderState = StudentAudioController.Instance.CurrentRecorderState;

            if (recorderState == RecorderState.RECORDING)
            {
                ForwardButton.IsEnabled = RewindButton.IsEnabled = false;
            }
            else
            {
                ForwardButton.IsEnabled = RewindButton.IsEnabled = true;
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
                ColumnSpacing = 10
            };

            ProgressView = new LingvoAudioProgressView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                OuterProgressColor = (Color)App.Current.Resources["primaryColor"],
                InnerProgressColor = Color.Red,
                InnerProgressEnabled = exercisable.StudentTrack != null,
                MuteEnabled = exercisable.StudentTrack != null,
                MaxProgress = 95000
            };

            if (exercisable.TeacherTrack != null)
            {
                ProgressView.MaxProgress = exercisable.TeacherTrack.Duration;
            }

            if (exercisable is IPage)
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

            RewindButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RewindImage,
                Text = "-" + SeekTimeStep,
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
                Text = "+" + SeekTimeStep,
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

            NextPageButton = new LingvoRoundImageButton
            {
                Image = "ic_next.xml",
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
                Image = "ic_previous.xml",
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

        private void Event_TeacherMemoChanged(TeacherMemo t)
        {
            if (exercisable is TeacherMemo && t.Id.Equals(exercisable.Id))
            {
                Refresh();
            }
        }

        private void Event_PageChanged(Lingvo.Common.Entities.Page p)
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
                ProgressView.MuteEnabled = exercisable.StudentTrack != null;
                NextPageButton.IsVisible = exercisable is IPage;
                PreviousPageButton.IsVisible = exercisable is IPage;
                NextPageButton.IsEnabled = workbook != null && NextPageButton.IsVisible && workbook.Pages.IndexOf((IPage)exercisable) < workbook.Pages.Count - 1;
                PreviousPageButton.IsEnabled = workbook != null && PreviousPageButton.IsVisible && workbook.Pages.IndexOf((IPage)exercisable) > 0;
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

            NextPageButton.IsEnabled = nextIndex + 1 < workbook.Pages.Count;
            PreviousPageButton.IsEnabled = nextIndex > 0;
            Exercisable = workbook.Pages[nextIndex];
        }

        private void ProgressView_StudentTrackMuted(bool muted)
        {
            StudentAudioController.Instance.IsMuted = muted;
        }

        private void RecordStopButton_OnClicked(object sender, EventArgs e)
        {
            PlayerState currentState = StudentAudioController.Instance.CurrentPlayerState;

            if (currentState == PlayerState.STOPPED)
            {

                StudentAudioController.Instance.StartStudentRecording();
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