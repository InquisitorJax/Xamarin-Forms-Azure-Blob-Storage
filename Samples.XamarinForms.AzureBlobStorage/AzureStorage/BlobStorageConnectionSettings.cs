using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

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
            string localTestingKey = "UseDevelopmentStorage=true";
            string platform = Device.RuntimePlatform;
            if (platform == "Android")
            {
                //BUG: ServiceClient is adding the container name to the storage Uri ?!
                localTestingKey = $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://10.0.2.2:10000/devstoreaccount1"; //Default to android
            }

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

    public class SasTokenBlobStorageSettings : ICloudBlobStorageSettingsProvider
    {
        //DOC: https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1
        //doc: https://blog.lextudio.com/how-to-let-android-emulator-access-iis-express-f6530a02b1d3

        private HttpClient _client;
        private CloudStorageSettings _storageSettings;

        public SasTokenBlobStorageSettings()
        {
            _client = new HttpClient();
            _client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<CloudStorageSettings> FetchSettingsAsync()
        {
            //NOTE: Android Emulator designates 10.0.2.2 as localhost proxy
            string platform = Device.RuntimePlatform;

            string remoteUrl = "http://localhost:53299/api/blobstoragetoken/";
            if (platform == "Android")
            {
                remoteUrl = "http://10.0.2.2:53299/api/blobstoragetoken/"; //Default to android
            }

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