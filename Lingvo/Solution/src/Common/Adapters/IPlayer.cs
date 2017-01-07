using Lingvo.Common.Entities;
using System;
namespace Lingvo.Common.Adapters
{
	public interface IPlayer
	{
		void Play(Recording recording);
		void Stop();
		void Pause();
		void Continue();
		void SeekTo(TimeSpan seek);

	}
}
