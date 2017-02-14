using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Lingvo.Backend.Tests
{
	public class FileMock : IFormFile
	{
		public string ContentDisposition { get; }
		public string ContentType { get; }
		public string FileName { get; }
		public IHeaderDictionary Headers { get; }
		public long Length { get; }
		public string Name { get; }

		public FileMock()
		{
		}

		public void CopyTo(Stream target)
		{
		}

		public async Task CopyToAsync(Stream target, CancellationToken cancellationToken)
		{
			await Task.Yield();
		}

		public Stream OpenReadStream()
		{
			return null;
		}
	}
}
