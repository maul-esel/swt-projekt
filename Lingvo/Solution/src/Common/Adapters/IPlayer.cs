using System;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;

namespace Lingvo.Common.Adapters
{
	public interface IPlayer
	{

		void PrepareTeacherTrack(Recording recording);
		void PrepareStudentTrack(Recording recording);
		void Play();
		void Stop();
		void Pause();
		void SeekTo(int seek);
		bool IsStudentTrackMuted { get; set; }
		double TotalLengthOfTrack { get; }
		double CurrentProgress { get; }
		event Action<int> Update;
		event Action<PlayerState> StateChange;
		PlayerState State { get; }
	}
}
