using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Lingvo.Backend.Services
{
	/// <summary>
	/// An <see cref="IStorage"/> implementing using the Azure blob storage.
	/// To use this, the server environment must be configured with an environment
	/// variable <c>LINGVO_AZURE_CONNSTR</c> containing the connection string.
	/// 
	/// For method documntation, see <see cref="IStorage"/>.
	/// </summary>
	public class AzureStorage : IStorage
	{
		private readonly CloudBlobClient _client;
		private CloudBlobContainer _container;

		private static string _connectionString;

		public AzureStorage()
		{
			if (_connectionString == null)
			{
				_connectionString = System.Environment.GetEnvironmentVariable("LINGVO_AZURE_CONNSTR");
				if (string.IsNullOrEmpty(_connectionString))
					throw new System.Exception("Cannot start server without configuring azure connection");
			}
			_client = CloudStorageAccount.Parse(_connectionString).CreateCloudBlobClient();
		}

		/// <summary>
		/// A helper method that ensures the necessary blob container exists,
		/// and initializes instance fields.
		/// </summary>
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
