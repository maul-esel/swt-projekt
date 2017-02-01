using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Lingvo.Backend
{
	public class AzureStorage : IStorage
	{
		private readonly CloudBlobClient _client;
		private CloudBlobContainer _container;

		public AzureStorage()
		{
			var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=lingvostorage;AccountKey=D7fHdT4yIweuYMNzSawJWPvD5m8zwe9zSmZk9dNZVoAnmRSmG2MX3DLXD5vfLUTqPLUSJGY+Pmp7f79SeZR3wA=="); // TODO
			_client = storageAccount.CreateCloudBlobClient();
		}

		private async Task InitializeAsync()
		{
			if (_container != null)
				return;

			_container = _client.GetContainerReference("teachertracks");
			await _container.CreateIfNotExistsAsync();
			await _container.SetPermissionsAsync(
				new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob }
			);
		}

		public async Task DeleteAsync(string identifier)
		{
			await InitializeAsync();
			await _container.GetBlockBlobReference(identifier).DeleteAsync();
		}

		public async Task<string> GetAccessUrlAsync(string identifier)
		{
			await InitializeAsync();
			return _container.GetBlockBlobReference(identifier).Uri.ToString();
		}

		public async Task SaveAsync(string identifier, Stream content)
		{
			await InitializeAsync();
			await _container.GetBlockBlobReference(identifier).UploadFromStreamAsync(content);
		}
	}
}
