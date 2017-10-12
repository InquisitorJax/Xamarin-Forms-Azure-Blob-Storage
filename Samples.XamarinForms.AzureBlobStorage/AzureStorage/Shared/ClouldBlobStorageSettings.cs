using System;

namespace Samples.XamarinForms.AzureBlobStorage.AzureStorage
{
    public class CloudStorageSettings
    {
        public string BlobStorageEndpoint { get; set; }

        public string ConnectionString { get; set; }
        public string DocumentStorageContainerName { get; set; }
        public string ImageStorageContainerName { get; set; }
        public string QueueStorageEndpoint { get; set; }

        //NOTE: Some libs (like Xamarin) only auth with a SAS key (doc: https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)
        public string SharedAccessSignature { get; set; }

        public DateTimeOffset SharedAccessSignatureExpires { get; set; }
        public string TableStorageEndpoint { get; set; }
    }
}