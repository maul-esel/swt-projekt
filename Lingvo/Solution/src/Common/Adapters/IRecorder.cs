using Lingvo.Common.Entities;
using Lingvo.Common.Enums;

namespace Lingvo.Common.Adapters
{
	public interface IRecorder
	{
		/// <summary>
		/// Starts the recording
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the recording, saves the created audio file and creates a new recording entity.
		/// </summary>
		/// <returns>The new recording</returns>
		/// <remarks>The recording is not yet saved to the database.</remarks>
		Recording Stop();

		/// <summary>
		/// Pauses the recording.
		/// </summary>
		void Pause();

		/// <summary>
		/// Continues a paused recording.
		/// </summary>
		void Continue();

		/// <summary>
		/// The current state of the recorder.
		/// </summary>
		RecorderState State { get; }

		/// <summary>
		/// Prepares for recording.
		/// </summary>
		/// <returns><c>true</c> if preparations were successful, <c>false otherwise</c></returns>
		bool PrepareToRecord();
	}
}
