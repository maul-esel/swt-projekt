using System;
using System.IO;
using Lingvo.Common.Entities;

namespace Lingvo.Common.Services
{
	public static class FileUtil
	{
		/// <summary>
		/// Calculates the absolute path to the given <paramref name="recording"/>'s MP3 file dynamically.
		/// In iOS, the app's folder path might change due to app updates or internal file system restructuring.
		/// </summary>
		public static String getAbsolutePath(Recording recording)
		{
			return getAbsolutePath(recording.LocalPath);
		}

		/// <summary>
		/// Calculates the absolute path dynamically.
		/// In iOS, the app's folder path might change due to app updates or internal file system restructuring.
		/// </summary>
		/// <param name="localPath">Relative local path.</param>
		public static String getAbsolutePath(String localPath)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), localPath);
		}
	}
}
