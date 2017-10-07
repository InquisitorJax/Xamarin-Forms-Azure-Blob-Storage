using System;

namespace Samples.XamarinForms.AzureBlobStorage.AzureStorage
{
    public class CloudBlobStorageSettings
    {
        public string BlobStorageEndpoint { get; set; }

        public string ConnectionString { get; set; }

        public string DocumentStorageContainerName { get; set; }
        public string ImageStorageContainerName { get; set; }
        public string SharedAccessSignature { get; set; }

        public DateTimeOffset SharedAccessSignatureExpires { get; set; }
    }
}