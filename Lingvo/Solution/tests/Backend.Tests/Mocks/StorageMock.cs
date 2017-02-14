using System;
using System.IO;
using System.Threading.Tasks;

namespace Lingvo.Backend.Tests
{
	public class StorageMock: IStorage
	{

		public StorageMock()
		{
		}

		private async Task InitializeAsync()
		{
			await Task.Yield();
		}

		public async Task DeleteAsync(string identifier)
		{
			await InitializeAsync();
		}

		public async Task<string> GetAccessUrlAsync(string identifier)
		{
			await InitializeAsync();
			return "abc";
		}

		public async Task SaveAsync(string identifier, Stream content)
		{
			await InitializeAsync();
		}
	}
}
