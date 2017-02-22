using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.Common.Services;
using System;
using Xamarin.Forms;
using System.IO;
using Lingvo.MobileApp.Entities;
using System.Threading.Tasks;

namespace Lingvo.MobileApp.Controllers
{
    /// <summary>
    /// Controller for working on pages
    /// </summary>
    public class StudentAudioController
    {
        private static StudentAudioController instance;

        private IPlayer audioPlayer;
        private IRecorder recorder;

        private IExercise exercisable;

        /// <summary>
        /// Gets or sets the selected page.
        /// </summary>
        /// <value>The selected page.</value>
        public IExercise Exercisable
        {
            get
            {
                return exercisable;
            }

            set
            {
                exercisable = value;

                try
                {
                    audioPlayer.StateChange += (obj) => StopRecorderIfNecessary();
                    audioPlayer.PrepareTeacherTrack(exercisable.TeacherTrack);

                    audioPlayer.PrepareStudentTrack(exercisable.StudentTrack);
                }
                catch
                {
                    Error?.Invoke();
                }

            }

        }


        /// <summary>
        /// This method delegates all subscribers to the udpate event of the audioplayer
        /// It is intented that the audioView subscribes to this event in order to get the current playback progress.
        /// </summary>
        public event Action<int> Update
        {
            add
            {
                audioPlayer.Update += value;
            }

            remove
            {
                audioPlayer.Update -= value;
            }
        }

        public event Action<PlayerState> StateChange
        {
            add
            {
                audioPlayer.StateChange += value;
            }
            remove
            {
                audioPlayer.StateChange -= value;
            }
        }

        public event Action Error;

        /// <summary>
        /// Gets or sets a value indicating whether the 
        /// <see cref="T:Lingvo.MobileApp.Controllers.StudentPageController"/>'s studentTrack is muted.
        /// </summary>
        /// <value><c>true</c> if is muted; otherwise, <c>false</c>.</value>
        public bool IsMuted
        {
            get
            {
                return audioPlayer.IsStudentTrackMuted;
            }

            set
            {
                audioPlayer.IsStudentTrackMuted = value;
            }


        }

        public PlayerState CurrentPlayerState => audioPlayer.State;
        public RecorderState CurrentRecorderState => recorder.State;

        /// <summary>
        /// Gets the instance of StudentPageController (Singleton Pattern)
        /// </summary>
        /// <returns>The instance.</returns>
        public static StudentAudioController Instance => instance ?? (instance = new StudentAudioController());

        private StudentAudioController()
        {
            audioPlayer = DependencyService.Get<IPlayer>();
            recorder = DependencyService.Get<IRecorder>();
        }

        /// <summary>
        /// Plays the page.
        /// </summary>
        public void PlayPage()
        {
            try
            {
                audioPlayer.Play();
            }
            catch
            {
                Stop();
                StopRecorderIfNecessary();
                Error?.Invoke();
            }
        }

        /// <summary>
        /// Starts the student recording.
        /// </summary>
        public void StartStudentRecording()
        {
            try
            {
                if (recorder.State != RecorderState.PREPARED)
                {
                    recorder.PrepareToRecord();
                }
                //This is to reset the studentRecording as an already existing recording 
                //should not be played while recording a new one
                audioPlayer.PrepareStudentTrack(null);
                recorder.Start();
                PlayPage();
            }
            catch
            {
                Stop();
                StopRecorderIfNecessary();
                Error?.Invoke();
            }

        }

        /// <summary>
        /// Pauses the recording and/or playing.
        /// </summary>
        public void Pause()
        {
            try
            {
                audioPlayer.Pause();
                recorder.Pause();
            }
            catch
            {
                Stop();
                StopRecorderIfNecessary();
                Error?.Invoke();
            }
        }

        /// <summary>
        /// Continues the recording and/or playing.
        /// </summary>
        public void Continue()
        {
            try
            {
                if (recorder.State == RecorderState.PAUSED)
                {
                    recorder.Continue();
                }
                audioPlayer.Play();
            }
            catch
            {
                Stop();
                StopRecorderIfNecessary();
                Error?.Invoke();
            }
        }

        /// <summary>
        /// Stops the recording and/or playing, saves the recorded file as student track
        /// </summary>
        public void Stop()
        {
            try
            {
                audioPlayer.Stop();
            }
            catch
            {
                StopRecorderIfNecessary();
                Error?.Invoke();
            }
        }

        /// <summary>
        /// Seeks the recording and/or playing to the given time.
        /// </summary>
        public void SeekTo(int seconds)
        {
            try
            {
                if (recorder.State != RecorderState.RECORDING)
                {
                    audioPlayer.SeekTo(seconds);
                }
            }
            catch
            {
                Stop();
                StopRecorderIfNecessary();
                Error?.Invoke();
            }
        }

        private async void StopRecorderIfNecessary()
        {
            if (audioPlayer.State == PlayerState.STOPPED && recorder.State == RecorderState.RECORDING)
            {
                //This means an recording-session has come to an end. Thus we are deleting the old studentTrack-Recording
                //if necessary and adding the new one to the selectedPage

                Recording recording = null;

                try
                {
                    //Getting new recording
                    recording = recorder.Stop();

                }
                catch
                {
                    Error.Invoke();
                }

                //deleting the old recording
                if (exercisable.StudentTrack != null)
                {
                    try
                    {
                        await Task.Run(() => File.Delete(FileUtil.getAbsolutePath(exercisable.StudentTrack)));
                    }
                    catch
                    {
                        //Do nothing
                    }
                }

                if (recording != null)
                {
                    Exercisable.StudentTrack = recording;
                    var db = App.Database;
                    db.Save(recording);

                    if (Exercisable is IPage)
                    {
                        db.Save((Lingvo.Common.Entities.Page)Exercisable);
                    }
                    else
                    {
                        db.Save((TeacherMemo)Exercisable);
                    }

                    //Reset the page
                    Exercisable = exercisable;
                }
            }
        }

    }
}
