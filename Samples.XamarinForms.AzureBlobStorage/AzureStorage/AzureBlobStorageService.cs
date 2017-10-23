using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Samples.XamarinForms.AzureBlobStorage.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //NOTE: This design is opinionated in that it assumes separate containers for images and documents
        // ...  a cleaner design would be to have cloud storage settings for each kind of storage intent
        //DOC: https://developer.xamarin.com/guides/xamarin-forms/cloud-services/storage/azure-storage/

        public AzureBlobStorageService()
        {
        }

        private ICloudBlobStorageSettingsProvider SettingsProvider
        {
            get { return DependencyService.Get<ICloudBlobStorageSettingsProvider>(); }
        }

        public async Task<bool> DeleteFileAsync(FileType fileType, string name)
        {
            var container = await ResolveContainerAsync(fileType);
            var blob = container.GetBlobReference(name);
            return await blob.DeleteIfExistsAsync();
        }

        public async Task<DownloadResult> DownloadFileAsync(FileType fileType, string fileId)
        {
            Requires.NotNull(fileId, nameof(fileId));

            try
            {
                var container = await ResolveContainerAsync(fileType);

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

        public async Task<IList<string>> GetFilesListAsync(FileType fileType)
        {
            var container = await ResolveContainerAsync(fileType);

            var allBlobsList = new List<string>();
            BlobContinuationToken token = null;

            //TODO: test for scenario where list permissions are not granted

            try
            {
                do
                {
                    //await container.CreateIfNotExistsAsync();

                    var result = await container.ListBlobsSegmentedAsync(token);
                    if (result.Results.Any())
                    {
                        var blobs = result.Results.Cast<CloudBlockBlob>().Select(b => b.Name);
                        allBlobsList.AddRange(blobs);
                    }
                    token = result.ContinuationToken;
                } while (token != null);
            }
            catch (StorageException ex)
            {
                Debug.WriteLine(ex.ToString());
                Debugger.Break();
            }
            return allBlobsList;
        }

        public async Task<UploadResult> UploadFileAsync(FileType fileType, Stream stream)
        {
            try
            {
                var container = await ResolveContainerAsync(fileType);

                //await container.CreateIfNotExistsAsync();

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

        private async Task<CloudBlobContainer> ResolveContainerAsync(FileType fileType)
        {
            var settings = await SettingsProvider.FetchSettingsAsync();
            string containerName = fileType == FileType.Image ? settings.ImageStorageContainerName : settings.DocumentStorageContainerName;

            CloudBlobContainer retContainer;
            CloudStorageAccount blobAccount;
            if (!string.IsNullOrEmpty(settings.SharedAccessSignature))
            {
                string sasToken = settings.SharedAccessSignature;

                StorageCredentials credentials = new StorageCredentials(sasToken);
                blobAccount = new CloudStorageAccount(credentials, settings.AccountName, null, true);

                //retContainer = new CloudBlobContainer(new Uri($"{settings.BlobStorageEndpoint}/{settings.SharedAccessSignature}"));
            }
            else if (!string.IsNullOrEmpty(settings.ConnectionString))
            {
                blobAccount = CloudStorageAccount.Parse(settings.ConnectionString);
            }
            else
            {
                throw new InvalidOperationException("Valid Cloud Storage Settings must have a ConnectionString or SAS property value");
            }

            var blobClient = blobAccount.CreateCloudBlobClient();
            retContainer = blobClient.GetContainerReference(containerName);

            //var permissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
            //await container.SetPermissionsAsync(permissions);

            return retContainer;
        }
    }
}