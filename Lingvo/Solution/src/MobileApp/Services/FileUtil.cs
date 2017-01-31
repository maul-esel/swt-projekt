using System;
using System.IO;
using Lingvo.Common.Entities;

namespace Lingvo.Common.Services
{
	public static class FileUtil
	{
		/// <summary>
		/// Calculates the absolute URL dynamically. In iOS, the app's folder path might change due to app updates or internal file system restructuring.
		/// </summary>
		/// <returns>The absolute path.</returns>
		/// <param name="recording">Recording.</param>
		public static String getAbsolutePath(Recording recording)
		{
			return getAbsolutePath(recording.LocalPath);
		}

		/// <summary>
		/// Calculates the absolute URL dynamically. In iOS, the app's folder path might change due to app updates or internal file system restructuring.
		/// </summary>
		/// <returns>The absolute path.</returns>
		/// <param name="localPath">Local path.</param>
		public static String getAbsolutePath(String localPath)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), localPath);
		}
	}
}
