using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Validation;
using Wibci.LogicCommand;

namespace Samples.XamarinForms.AzureBlobStorage.AzureStorage
{
    public class AzureBlobStorageService : ICloudBlobStorageService
    {
        //DOC: https://developer.xamarin.com/guides/xamarin-forms/cloud-services/storage/azure-storage/

        private readonly ICloudBlobStorageSettingsProvider _storageSettings;

        public AzureBlobStorageService(ICloudBlobStorageSettingsProvider settingsProvider)
        {
            Requires.NotNull(settingsProvider, nameof(settingsProvider));

            _storageSettings = settingsProvider;
        }

        public async Task<bool> DeleteFileAsync(string containerName, string connectionString, string name)
        {
            var container = ResolveContainer(containerName, connectionString);
            var blob = container.GetBlobReference(name);
            return await blob.DeleteIfExistsAsync();
        }

        public async Task<DownloadResult> DownloadDocumentAsync(string fileId)
        {
            var settings = await _storageSettings.FetchSettingsAsync();

            Requires.NotNull(fileId, nameof(fileId));

            var result = await DownloadFileAsync(settings.DocumentStorageContainerName, settings.ConnectionString, fileId);

            return result;
        }

        public async Task<DownloadResult> DownloadImageAsync(string fileId)
        {
            var settings = await _storageSettings.FetchSettingsAsync();

            Requires.NotNull(fileId, nameof(fileId));

            var result = await DownloadFileAsync(settings.ImageStorageContainerName, settings.ConnectionString, fileId);

            return result;
        }

        public async Task<UploadResult> UploadDocumentAsync(Stream document)
        {
            var settings = await _storageSettings.FetchSettingsAsync();
            string fileId = await UploadFileAsync(settings.DocumentStorageContainerName, settings.ConnectionString, document);

            return new UploadResult(fileId);
        }

        public async Task<UploadResult> UploadImageAsync(Stream image)
        {
            var settings = await _storageSettings.FetchSettingsAsync();
            string fileId = await UploadFileAsync(settings.ImageStorageContainerName, settings.ConnectionString, image);

            return new UploadResult(fileId);
        }

        private async Task<DownloadResult> DownloadFileAsync(string containerName, string connectionString, string fileId)
        {
            var container = ResolveContainer(containerName, connectionString);

            var blob = container.GetBlobReference(fileId);
            if (await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                byte[] blobBytes = new byte[blob.Properties.Length];

                await blob.DownloadToByteArrayAsync(blobBytes, 0);

                return new DownloadResult { File = blobBytes, FileId = fileId };
            }
            else
            {
                return new DownloadResult { Notification = Notification.Error("The file your are looking for does not exist.") };
            }
        }

        private async Task<IList<string>> GetFilesListAsync(string containerName, string connectionString)
        {
            var container = ResolveContainer(containerName, connectionString);

            var allBlobsList = new List<string>();
            BlobContinuationToken token = null;

            do
            {
                var result = await container.ListBlobsSegmentedAsync(token);
                if (result.Results.Any())
                {
                    var blobs = result.Results.Cast<CloudBlockBlob>().Select(b => b.Name);
                    allBlobsList.AddRange(blobs);
                }
                token = result.ContinuationToken;
            } while (token != null);

            return allBlobsList;
        }

        private CloudBlobContainer ResolveContainer(string containerName, string connectionString)
        {
            var blobAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = blobAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }

        private async Task<string> UploadFileAsync(string containerName, string connectionString, Stream stream)
        {
            var container = ResolveContainer(containerName, connectionString);
            await container.CreateIfNotExistsAsync();

            var fileId = Guid.NewGuid().ToString();
            var fileBlob = container.GetBlockBlobReference(fileId);
            await fileBlob.UploadFromStreamAsync(stream);

            return fileId;
        }
    }
}