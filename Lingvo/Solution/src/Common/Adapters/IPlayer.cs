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
		void SeekTo(TimeSpan seek);
		bool IsStudentTrackMuted { get; set; }
		double TotalLengthOfTrack { get; }
		double CurrentProgress { get; }
		event Action<int> Update;

	}
}
