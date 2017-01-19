using System;
namespace Lingvo.Common.Enums
{
	/// <summary>
	/// Possible states the IPlayer can be in.
	/// </summary>
	public enum PlayerState
	{
		IDLE, //This is in case there is no teacherTrack set yet.
		PLAYING,
		STOPPED,
		PAUSED,
	}
}
