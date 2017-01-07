using System;
namespace Lingvo.Common
{
	public interface IRecorder
	{
		void Start();
		Recording Stop();
		void Pause();
		void Continue();
		void SeekTo(TimeSpan seek);
	}
}
