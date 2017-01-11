using System;
using Android.Media;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.MobileApp.Droid.Sound;
using Xamarin.Forms;

[assembly: Dependency(typeof(Player))]
namespace Lingvo.MobileApp.Droid.Sound
{
	public class Player : IPlayer
	{

		private MediaPlayer track;

		public Player()
		{
		}

		public void Continue()
		{
			track.Start();
		}

		public void Pause()
		{
			track.Pause();
		}

		public void Play(Recording recording)
		{
			
			track = new MediaPlayer();
			var file = global::Android.App.Application.Context.Assets.OpenFd(recording.LocalPath);
			track.Prepared += (s, e) =>
			{
				track.Start();
			};
			track.SetDataSource(file.FileDescriptor, file.StartOffset, file.Length);
			track.Prepare();
		}

		public void SeekTo(TimeSpan seek)
		{
			track.SeekTo(seek.Milliseconds);
		}

		public void Stop()
		{
			track.Stop();
		}

	}
}
