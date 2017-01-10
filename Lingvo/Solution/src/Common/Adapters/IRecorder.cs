using Lingvo.Common.Entities;
using System;
namespace Lingvo.Common.Adapters
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
