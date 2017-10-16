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

        public Task<CloudStorageSettings> FetchSettingsAsync()
        {
            var settings = new CloudStorageSettings
            {
                DocumentStorageContainerName = "document-container-test",
                ImageStorageContainerName = "image-container-test",
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

        public Task<CloudStorageSettings> FetchSettingsAsync()
        {
            const string localTestingKey = "UseDevelopmentStorage=true";
            var settings = new CloudStorageSettings
            {
                ImageStorageContainerName = "image-container",
                DocumentStorageContainerName = "document-container",
                SharedAccessSignature = null,
                BlobStorageEndpoint = null, //"http://127.0.0.1:10000/development",
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
        private CloudStorageSettings _storageSettings;

        public ZCloudBlobStorageSettings()
        {
            _client = new HttpClient();
            _client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<CloudStorageSettings> FetchSettingsAsync()
        {
            const string remoteUrl = "http://localhost:5000/api/storagesettings/";

            var uri = new Uri(string.Format(remoteUrl, string.Empty));

            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _storageSettings = JsonConvert.DeserializeObject<CloudStorageSettings>(content);
            }

            return _storageSettings;
        }
    }
}