using System;
namespace Lingvo.Common
{
	public interface IPlayer
	{
		void Play(Recording recording);
		void Stop();
		void Pause();
		void SeekTo(TimeSpan seek);
	}
}
