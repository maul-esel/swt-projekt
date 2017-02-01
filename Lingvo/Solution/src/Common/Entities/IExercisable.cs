

namespace Lingvo.Common.Entities
{
    public interface IExercisable
    {
        int Id { get; set; }
        /// <summary>
        /// Gets or sets the teacher track.
        /// </summary>
        /// <value>The teacher track.</value>
        Recording TeacherTrack
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the student track.
        /// </summary>
        /// <value>The student track.</value>
        Recording StudentTrack
        {
            get;
            set;
        }

        /// <summary>
        /// Deletes the student recording that is currently attached to this page.
        /// </summary>
        void DeleteStudentRecording();
    }
}