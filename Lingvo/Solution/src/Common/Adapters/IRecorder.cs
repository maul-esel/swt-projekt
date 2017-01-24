using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using System;
namespace Lingvo.Common.Adapters
{
	public interface IRecorder
	{
		void Start();
		Recording Stop();
		void Pause();
		void Continue();
		void SeekTo(int seconds);
		RecorderState State { get; }
	}
}
