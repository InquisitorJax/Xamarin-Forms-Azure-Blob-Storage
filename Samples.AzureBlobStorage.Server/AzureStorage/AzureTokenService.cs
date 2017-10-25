using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Samples.XamarinForms.AzureBlobStorage.AzureStorage;
using System;
using System.Threading.Tasks;
using Wibci.LogicCommand;

namespace Samples.AzureBlobStorage.Server.AzureStorage
{
    public interface IFetchBlobStorageSettingsCommand : IAsyncLogicCommand<object, StorageTokenSettingsResult>
    {
    }

    public class FetchAzureBlobStorageSettingsCommand : AsyncLogicCommand<object, StorageTokenSettingsResult>, IFetchBlobStorageSettingsCommand
    {
        //doc:https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1

        private readonly IConfiguration _config;

        public FetchAzureBlobStorageSettingsCommand(IConfiguration config)
        {
            _config = config;
        }

        public override Task<StorageTokenSettingsResult> ExecuteAsync(object request)
        {
            var retResult = new StorageTokenSettingsResult();

            string connectionString = _config["BlobStorage:ConnectionString"];
            string expiryTimeSpanString = _config["BlobStorage:TokenExpiryTimeSpanInMinutes"];
            double expiryMinutes = double.Parse(expiryTimeSpanString);
            var expires = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString); //throws format exception if not in correct format

                SharedAccessAccountPolicy accessPolicy = new SharedAccessAccountPolicy()
                {
                    //Permissions = SharedAccessAccountPermissions.Add | SharedAccessAccountPermissions.Create | SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write,
                    Permissions = SharedAccessAccountPermissions.Add | SharedAccessAccountPermissions.Create | SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write | SharedAccessAccountPermissions.List,
                    Protocols = SharedAccessProtocol.HttpsOnly,
                    Services = SharedAccessAccountServices.Blob,
                    ResourceTypes = SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object,
                    //SharedAccessStartTime = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(15)), //NOTE: Leaving value blank will make it valid immediately - safter for time zone reasons
                    SharedAccessExpiryTime = expires,
                };

                string token = storageAccount.GetSharedAccessSignature(accessPolicy);

                //NOTE: local storage endpoint will point to: https://<account-name>.blob.core.windows.net/ - this needs to be replaced by http://127.0.0.1:10000/<account-name>/<resource-path>
                var settings = new CloudStorageSettings()
                {
                    SharedAccessSignature = token,
                    SharedAccessSignatureExpires = expires,
                    DocumentStorageContainerName = "document-container-test",
                    ImageStorageContainerName = "image-container-test",
                    AccountName = storageAccount.Credentials.AccountName,
                    ConnectionString = $"SharedAccessSignature={token.TrimStart('?')};BlobEndpoint={storageAccount.BlobEndpoint.AbsoluteUri}",
                    BlobStorageEndpoint = storageAccount.BlobEndpoint.AbsoluteUri
                };

                retResult.StorageSettings = settings;
            }
            catch (FormatException)
            {
                retResult.Notification.Add("Connection String was not in the correct format");
            }

            return Task.FromResult(retResult);
        }
    }

    public class StorageTokenSettingsResult : CommandResult
    {
        public CloudStorageSettings StorageSettings { get; set; }
    }
}