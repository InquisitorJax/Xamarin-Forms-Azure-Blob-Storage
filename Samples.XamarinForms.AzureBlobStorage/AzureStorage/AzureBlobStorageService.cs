using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Validation;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage.AzureStorage
{
    public class AzureBlobStorageService : ICloudBlobStorageService
    {
        //DOC: https://developer.xamarin.com/guides/xamarin-forms/cloud-services/storage/azure-storage/

        public AzureBlobStorageService()
        {
        }

        private ICloudBlobStorageSettingsProvider SettingsProvider
        {
            get { return DependencyService.Get<ICloudBlobStorageSettingsProvider>(); }
        }

        public async Task<bool> DeleteFileAsync(string containerName, string connectionString, string name)
        {
            var container = await ResolveContainerAsync(containerName, connectionString);
            var blob = container.GetBlobReference(name);
            return await blob.DeleteIfExistsAsync();
        }

        public async Task<DownloadResult> DownloadDocumentAsync(string fileId)
        {
            var settings = await SettingsProvider.FetchSettingsAsync();

            Requires.NotNull(fileId, nameof(fileId));

            var result = await DownloadFileAsync(settings.DocumentStorageContainerName, settings.ConnectionString, fileId);

            return result;
        }

        public async Task<DownloadResult> DownloadImageAsync(string fileId)
        {
            var settings = await SettingsProvider.FetchSettingsAsync();

            Requires.NotNull(fileId, nameof(fileId));

            var result = await DownloadFileAsync(settings.ImageStorageContainerName, settings.ConnectionString, fileId);

            return result;
        }

        public async Task<IList<string>> GetFilesListAsync(string containerName, string connectionString)
        {
            //var debugTest = new List<string>
            //{
            //    "file1" + containerName,
            //    "file2" + containerName,
            //    "file3" + containerName
            //};

            //return await Task.FromResult(debugTest);

            var container = await ResolveContainerAsync(containerName, connectionString);

            var allBlobsList = new List<string>();
            BlobContinuationToken token = null;

            do
            {
                await container.CreateIfNotExistsAsync();

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

        public async Task<UploadResult> UploadDocumentAsync(Stream document)
        {
            var settings = await SettingsProvider.FetchSettingsAsync();
            return await UploadFileAsync(settings.DocumentStorageContainerName, settings.ConnectionString, document);
        }

        public async Task<UploadResult> UploadImageAsync(Stream image)
        {
            var settings = await SettingsProvider.FetchSettingsAsync();
            return await UploadFileAsync(settings.ImageStorageContainerName, settings.ConnectionString, image);
        }

        private async Task<DownloadResult> DownloadFileAsync(string containerName, string connectionString, string fileId)
        {
            try
            {
                var container = await ResolveContainerAsync(containerName, connectionString);

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
            catch (StorageException se)
            {
                return new DownloadResult()
                {
                    Notification = Notification.Error(se.Message)
                };
            }
        }

        private async Task<CloudBlobContainer> ResolveContainerAsync(string containerName, string connectionString)
        {
            var blobAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = blobAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);

            //var permissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };

            //await container.SetPermissionsAsync(permissions);

            return await Task.FromResult(container);
        }

        private async Task<UploadResult> UploadFileAsync(string containerName, string connectionString, Stream stream)
        {
            try
            {
                var container = await ResolveContainerAsync(containerName, connectionString);
                await container.CreateIfNotExistsAsync();

                var fileId = Guid.NewGuid().ToString();
                var fileBlob = container.GetBlockBlobReference(fileId);
                await fileBlob.UploadFromStreamAsync(stream);

                return new UploadResult(fileId);
            }
            catch (StorageException se)
            {
                return new UploadResult(null)
                {
                    Notification = Notification.Error(se.ToString())
                };
            }
        }
    }
}