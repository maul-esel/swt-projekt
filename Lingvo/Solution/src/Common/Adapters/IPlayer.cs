using Lingvo.Common.Entities;
using System;
namespace Lingvo.Common.Adapters
{
	public interface IPlayer
	{

		void PrepareTeacherTrack(Recording recording);
		void PrepareStudentTrack(Recording recording);
		void Play();
		void Stop();
		void Pause();
		void Continue();
		void SeekTo(TimeSpan seek);
		bool IsStudentTrackMuted { get; set; }

	}
}
