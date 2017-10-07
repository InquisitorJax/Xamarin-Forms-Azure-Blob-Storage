using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Samples.XamarinForms.AzureBlobStorage.AzureStorage
{
#if DEBUG

    public class CloudTestStorageSettingsProvider : ICloudBlobStorageSettingsProvider
    {
        //NOTE: This option is not recommended - NEVER store your storage keys for easy access - even in code

        public Task<CloudBlobStorageSettings> FetchSettingsAsync()
        {
            var settings = new CloudBlobStorageSettings
            {
                DocumentStorageContainerName = "document-container",
                ImageStorageContainerName = "image-container",
                ConnectionString = ResolveConnectionString(),
                BlobStorageEndpoint = "https://[STORAGE-NAME].blob.core.windows.net/",
                SharedAccessSignature = null
            };

            return Task.FromResult(settings);
        }

        private string ResolveConnectionString()
        {
            //NOTE: This connection must never be distributed with the app
            return "DefaultEndpointsProtocol=https;AccountName=[ACCOUNT-NAME];AccountKey=[ACCOUNT-KEY];EndpointSuffix=core.windows.net";
        }
    }

    public class LocalBlobStorageSettingsProvider : ICloudBlobStorageSettingsProvider
    {
        //DOC: https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator

        public Task<CloudBlobStorageSettings> FetchSettingsAsync()
        {
            const string localTestingKey = "UseDevelopmentStorage=true";
            var settings = new CloudBlobStorageSettings
            {
                ImageStorageContainerName = "image-container",
                DocumentStorageContainerName = "document-container",
                SharedAccessSignature = null,
                BlobStorageEndpoint = null,
                ConnectionString = localTestingKey
            };

            return Task.FromResult(settings);
        }
    }

#endif

    public class ZCloudBlobStorageSettings : ICloudBlobStorageSettingsProvider
    {
        //DOC: https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1

        private HttpClient _client;
        private CloudBlobStorageSettings _storageSettings;

        public ZCloudBlobStorageSettings()
        {
            _client = new HttpClient();
            _client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<CloudBlobStorageSettings> FetchSettingsAsync()
        {
            const string remoteUrl = "http://localhost:5000/api/storagesettings/";

            var uri = new Uri(string.Format(remoteUrl, string.Empty));

            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _storageSettings = JsonConvert.DeserializeObject<CloudBlobStorageSettings>(content);
            }

            return _storageSettings;
        }
    }
}