using System;
using Android.Media;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.Droid.Sound;
using Xamarin.Forms;
using Android.OS;
using Lingvo.Common.Services;

[assembly: Dependency(typeof(Player))]
namespace Lingvo.MobileApp.Droid.Sound
{
	/// <summary>
	/// Android implementation of <see cref="IPlayer"/>. For documentation on the <see cref="IPlayer"/> methods, see <see cref="IPlayer"/>.
	/// </summary>
    public class Player : IPlayer
    {

        private MediaPlayer teacherTrack;
        private MediaPlayer studentTrack;

        private Recording teacherRecording;
        private Recording studentRecording;

        private bool isStudentTrackMuted;

        private PlayerState state;

        public event Action<int> Update;
        public event Action<PlayerState> StateChange;

        private Handler progressHandler;
        private Action progressUpdate;

        private AudioManager audioManager;

        public Player()
        {
            audioManager = AudioManager.FromContext(Android.App.Application.Context);
            progressHandler = new Handler(Looper.MainLooper);
            progressUpdate = new Action(() =>
            {
                if (State == PlayerState.PLAYING)
                {
                    OnProgress(teacherTrack.CurrentPosition);
                    progressHandler.PostDelayed(progressUpdate, 100);
                }
            });
            State = PlayerState.IDLE;
        }

		/// <summary>
		/// Method, which is called by the timer to initiate a draw of the progress view
		/// </summary>
		/// <param name="elapsedMilliseconds">Elapsed milliseconds.</param>
        private void OnProgress(int elapsedMilliseconds)
        {
            Update?.Invoke(elapsedMilliseconds);

        }

        public bool IsStudentTrackMuted
        {
            get
            {
                return isStudentTrackMuted;
            }

            set
            {
				//As there is no native mute-flag we set the volume to zero
                isStudentTrackMuted = value;
                float volume = isStudentTrackMuted ? 0.0f : 1.0f;
                studentTrack?.SetVolume(volume, volume);
            }
        }

        public double TotalLengthOfTrack
        {
            get
            {
                if (teacherTrack != null)
                {
                    return teacherTrack.Duration;
                }
                return 0.0;
            }
        }

        public double CurrentProgress
        {
            get
            {
                if (teacherTrack != null)
                {
                    return teacherTrack.CurrentPosition;
                }
                return 0.0;
            }
        }

		/// <summary>
		/// State of current player (playing, paused, idle, stopped)
		/// </summary>
		/// <value>The state.</value>
        public PlayerState State
        {
            get
            {
                return state;
            }
            private set
            {
                state = value;
                OnStateChange();
            }
        }

        public void Pause()
        {
            teacherTrack?.Pause();
            studentTrack?.Pause();
            State = PlayerState.PAUSED;
            progressHandler.RemoveCallbacks(progressUpdate);
            Sync();
        }

		/// <summary>
		/// Starts and continues the playback
		/// </summary>
        public void Play()
        {
            teacherTrack?.Start();
            studentTrack?.Start();
            State = PlayerState.PLAYING;
            progressHandler.PostDelayed(progressUpdate, 100); //Each 100 milliseconds the OnProgress method is called
        }

        public void PrepareStudentTrack(Recording recording)
        {
            if (recording == null)
            {
                studentTrack = null;
            }
            else
            {
                studentRecording = recording;
                studentTrack = CreateNewPlayer(recording);
                IsStudentTrackMuted = false;
            }
        }

        public void PrepareTeacherTrack(Recording recording)
        {
            teacherRecording = recording;
            teacherTrack = CreateNewPlayer(recording);
            teacherTrack.Completion += (o, e) =>
            {
				//We subscribe to the native stop event of the player
				//to keep track of the actual playback
                Stop();
                OnProgress(teacherTrack.Duration);
            };
            State = PlayerState.STOPPED;
            OnProgress(teacherTrack.CurrentPosition);
        }

        public void SeekTo(int seconds)
        {
            if (teacherTrack?.CurrentPosition + seconds * 1000 >= teacherTrack?.Duration)
            {
                OnProgress(teacherTrack.Duration);
                Stop();
                return;
            }
            teacherTrack?.SeekTo(teacherTrack.CurrentPosition + seconds * 1000);

            if (teacherTrack?.CurrentPosition >= studentTrack?.Duration)
            {
                studentTrack?.Pause();
            }
            else
            {
                studentTrack?.SeekTo(teacherTrack.CurrentPosition);

                if (studentTrack != null && !studentTrack.IsPlaying && teacherTrack.IsPlaying)
                {
                    studentTrack.Start();
                }
            }

            OnProgress(teacherTrack.CurrentPosition);
        }

		/// <summary>
		/// Stops both audioplayers and reinitializes them for a new session 
		/// </summary>
        public void Stop()
        {
            teacherTrack?.Stop();
            studentTrack?.Stop();
            ReinitializePlayer(teacherTrack, teacherRecording);
            ReinitializePlayer(studentTrack, studentRecording);
            progressHandler.RemoveCallbacks(progressUpdate);
			State = PlayerState.STOPPED;
        }

		/// <summary>
		/// Reinitializes the player after a session has stopped. This is due to the
		/// fact that audio sessions in Android have to be explicitely closed.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="recording">Recording.</param>
        private void ReinitializePlayer(MediaPlayer player, Recording recording)
        {
            if (recording != null && player != null)
            {
                player?.Reset();
                var fileDesriptor = Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(FileUtil.getAbsolutePath(recording)), Android.OS.ParcelFileMode.ReadOnly);
                player?.SetDataSource(fileDesriptor.FileDescriptor);

                player?.Prepare();
            }
        }

        private MediaPlayer CreateNewPlayer(Recording recording)
        {
            var mediaPlayer = new MediaPlayer();

            audioManager.SpeakerphoneOn = !audioManager.WiredHeadsetOn;
            audioManager.Mode = audioManager.WiredHeadsetOn ? Mode.Normal : Mode.InCommunication;

            var fileDesriptor = Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(FileUtil.getAbsolutePath(recording)), Android.OS.ParcelFileMode.ReadOnly);
            mediaPlayer.SetDataSource(fileDesriptor.FileDescriptor);

            mediaPlayer.SetAudioStreamType(audioManager.WiredHeadsetOn ? Stream.Music : Stream.VoiceCall);
            mediaPlayer.SetVolume(1.0f, 1.0f);
            mediaPlayer.Prepare();

            return mediaPlayer;
        }

		/// <summary>
		/// Update method which is called when the state property changes
		/// </summary>
        private void OnStateChange()
        {
            StateChange?.Invoke(state);
        }

		/// <summary>
		/// Syncs the playback positions of teacher and student track
		/// </summary>
        private void Sync()
        {
            studentTrack?.SeekTo(teacherTrack.CurrentPosition);
        }
    }
}
