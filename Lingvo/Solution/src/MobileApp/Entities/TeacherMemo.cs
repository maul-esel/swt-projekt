using Lingvo.Common.Entities;
using System;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Lingvo.MobileApp.Entities
{
    /// <summary>
    /// Teacher memo: teachers can record a sentence or word the student can't
	/// pronounce easily. Students can listen to these recordings and record their own voice.
    /// </summary>
	[Table("TeacherMemos")]
    public class TeacherMemo : IExercise
    {
		/// <summary>
		/// References the <see cref="TeacherTrack"/>'s id in the database.
		/// Necessary for ORM.
		/// </summary>
        [ForeignKey(typeof(Recording))]
        public int RecordingId { get; set; }

		/// <summary>
		/// References the <see cref="StudentTrack"/>'s id in the database.
		/// Necessary for ORM.
		/// </summary>
        [ForeignKey(typeof(Recording))]
        public int? StudentTrackId { get; set; }

		/// <summary>
		/// The display name given when the memo was created.
		/// </summary>
        public String Name { get; set; }

		/// <summary>
		/// A unique id used to identify the memo in the database. Necessary for ORM.
		/// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

		/// <summary>
		/// See <see cref="IExercise.TeacherTrack"/>.
		/// </summary>
        [OneToOne]
        public Recording TeacherTrack { get; set; }

		/// <summary>
		/// See <see cref="IExercise.StudentTrack"/>.
		/// </summary>
        [OneToOne]
        public Recording StudentTrack { get; set; }

        public TeacherMemo()
        {
        }

		/// <summary>
		/// See <see cref="IExercise.DeleteStudentRecording"/>.
		/// </summary>
        public void DeleteStudentRecording()
        {
            StudentTrack = null;
            StudentTrackId = 0;
        }
    }
}
