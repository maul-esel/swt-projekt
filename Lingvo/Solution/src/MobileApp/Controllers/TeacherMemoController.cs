using Lingvo.Common.Adapters;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.Entities;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Controllers
{
    /// <summary>
    /// Controller for handling teacher memos.
    /// </summary>
    public class TeacherMemoController
    {
        private static TeacherMemoController instance;
        private IRecorder audioRecorder;

        private Task progressHandler;

        public delegate void OnProgressUpdate(int progress);

        public event OnProgressUpdate Update;

        public TeacherMemo CurrentMemo
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the instance of the teacherMemoController (Singleton Pattern)
        /// </summary>
        /// <value>The instance.</value>
        public static TeacherMemoController Instance => instance ?? (instance = new TeacherMemoController());

        private TeacherMemoController()
        {
            audioRecorder = DependencyService.Get<IRecorder>();
        }

        public void Reset()
        {
            if (CurrentMemo != null)
            {
                App.Database.Delete(CurrentMemo);
                CurrentMemo = null;
            }
        }

        /// <summary>
        /// Starts a new teacher memo.
        /// </summary>
        public void StartTeacherMemo()
        {
            if (audioRecorder.State != RecorderState.PREPARED)
            {
                audioRecorder.PrepareToRecord();
            }

            progressHandler = new Task(async () =>
            {
                DateTime begin = DateTime.Now;
                while (State == RecorderState.RECORDING)
                {
                    int seconds = (int)new TimeSpan(DateTime.Now.Ticks - begin.Ticks).TotalMilliseconds;
                    Update?.Invoke(seconds);
                    await Task.Delay(1000);
                }
            });

            audioRecorder.Start();
            progressHandler.Start();
        }

        /// <summary>
        /// Ends the teacher memo started before with the start method.
        /// </summary>
        public void EndTeacherMemo()
        {
            CurrentMemo = new TeacherMemo() { TeacherTrack = audioRecorder.Stop() };
        }

        /// <summary>
        /// Saves the teacher memo to the local collection.
        /// </summary>
        /// <param name="name">Name.</param>
        public void SaveTeacherMemo(String name)
        {
            CurrentMemo.Name = name;
            LocalCollection.Instance.AddTeacherMemo(CurrentMemo);
            CurrentMemo = null;

            Reset();
        }

        /// <summary>
        /// Returns the state of the recorder.
        /// </summary>
        public RecorderState State
        {
            get { return audioRecorder.State; }
        }
    }
}
