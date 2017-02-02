﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Lingvo.Backend
{
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