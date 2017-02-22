using System.IO;
using System.Threading.Tasks;

namespace Lingvo.Backend.Services
{
	/// <summary>
	/// An abstract interface for managing binary files.
	/// This is used to store teacher tracks for all pages.
	/// </summary>
	public interface IStorage
	{
		/// <summary>
		/// Saves the given <paramref name="content"/> stream with the given <paramref name="identifier"/>.
		/// If data was previously saved with the same identifier, it is overwritten.
		/// </summary>
		/// <param name="identifier">A unique name that can be used later to delete or access the stream.</param>
		/// <param name="content">A binary stream containing the information to be stored.</param>
		/// <returns>A <see cref="Task"/> that completes once the save process is complete.</returns>
		Task SaveAsync(string identifier, Stream content);

		/// <summary>
		/// Deletes all data stored under the given <paramref name="identifier"/>.
		/// If no such data exists, does nothing.
		/// </summary>
		/// <returns>A <see cref="Task"/> that completes once the deletion process is complete.</returns>
		Task DeleteAsync(string identifier);

		/// <summary>
		/// Retrieves an URL under which the data for the given <paramref name="identifier"/>
		/// can be downloaded.
		/// </summary>
		/// <returns>A task that eventually completes and returns a string containing the URL.</returns>
		Task<string> GetAccessUrlAsync(string identifier);
	}
}
