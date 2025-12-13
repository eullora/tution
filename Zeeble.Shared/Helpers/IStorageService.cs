
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Zeeble.Shared.Helpers
{
    public interface IStorageService
    {
        Task<string> UploadFileToStorage(string fileName, Stream stream);
    }

    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;
        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileToStorage(string fileName, Stream stream)
        {
            var blobStorageConnectionString = _configuration.GetSection("AzureStorage:ConnectionString").Value;
            var container = new BlobContainerClient(blobStorageConnectionString, "zeeble");
            var blob = container.GetBlobClient(fileName);
            var response = await blob.UploadAsync(stream);            

            return "Ok";
        }
    }
}
