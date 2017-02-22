using System;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;

namespace Lingvo.Common.Adapters
{
	public interface IPlayer
	{
		/// <summary>
		/// Prepares the teacher track, i.e makes it ready for playback
		/// </summary>
		/// <param name="recording">Recording.</param>
		void PrepareTeacherTrack(Recording recording);

		/// <summary>
		/// Prepares the student track, i.e. makes it ready for playback
		/// </summary>
		/// <param name="recording">Recording.</param>
		void PrepareStudentTrack(Recording recording);

		/// <summary>
		/// Starts or continues the playback
		/// </summary>
		void Play();

		/// <summary>
		/// Stops playback of all tracks.
		/// </summary>
		void Stop();

		/// <summary>
		/// Pauses the playback
		/// </summary>
		void Pause();

		/// <summary>
		/// Jumps back or forth in playback. There is a distinction whether a studenttrack is running or not.
		/// In addition, it is checked whether the duration of the student track is shorter than the one of the teacher
		/// track. If so, the student track is muted once we jump out of its duration.
		/// </summary>
		/// <param name="seconds">Seconds.</param>
		void SeekTo(int seek);

		bool IsStudentTrackMuted { get; set; }

		/// <summary>
		/// The total length of the teacher track, in milliseconds
		/// </summary>
		double TotalLengthOfTrack { get; }

		/// <summary>
		/// The current playback position, in milliseconds
		/// </summary>
		double CurrentProgress { get; }

		/// <summary>
		/// Raised to inform listeners of a change in <see cref="CurrentProgress"/>
		/// </summary>
		event Action<int> Update;

		/// <summary>
		/// Raised when <see cref="State"/> changes.
		/// </summary>
		event Action<PlayerState> StateChange;

		/// <summary>
		/// The current state of the player.
		/// </summary>
		PlayerState State { get; }
	}
}
