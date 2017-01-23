using System;
using Xamarin.Forms;
using Lingvo.Common.Adapters;
using Lingvo.MobileApp.iOS.Sound;
using Lingvo.Common.Enums;
using Lingvo.Common.Entities;

[assembly: Dependency(typeof(Recorder))]
namespace Lingvo.MobileApp.iOS.Sound
{
	
	public class Recorder :IRecorder
	{
		private RecorderState state;

		public Recorder()
		{
			State = RecorderState.IDLE;
		}

		public RecorderState State
		{
			get
			{
				return state;
			}

			private set
			{
				state = value;
			}
				
		}

		public void Continue()
		{
			State = RecorderState.RECORDING;
		}

		public void Pause()
		{
			if (State == RecorderState.RECORDING)
			{ 
				State = RecorderState.PAUSED;
			}

		}

		public void SeekTo(int seconds)
		{
			
		}

		public void Start()
		{
			State = RecorderState.RECORDING;
		}

		public Recording Stop()
		{
			State = RecorderState.STOPPED;
			return new Recording();
		}
	}
}
