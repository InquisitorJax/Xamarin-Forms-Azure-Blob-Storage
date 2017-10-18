using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;
using Wibci.LogicCommand;

namespace Samples.AzureBlobStorage.Server.AzureStorage
{
    public interface IGenerateBlobStorageSasTokenCommand : IAsyncLogicCommand<object, StorageTokenResult>
    {
    }

    public class GenerateAzureBlobStorageSasTokenCommand : AsyncLogicCommand<object, StorageTokenResult>, IGenerateBlobStorageSasTokenCommand
    {
        private readonly IConfiguration _config;

        public GenerateAzureBlobStorageSasTokenCommand(IConfiguration config)
        {
            _config = config;
        }

        public override Task<StorageTokenResult> ExecuteAsync(object request)
        {
            var retResult = new StorageTokenResult();

            string connectionString = _config["BlobStorage:ConnectionString"];
            string expiryTimeSpanString = _config["BlobStorage:TokenExpiryTimeSpanInMinutes"];
            double expiryMinutes = double.Parse(expiryTimeSpanString);
            retResult.ExpiryUtc = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString); //throws format exception if not in correct format

                SharedAccessAccountPolicy accessPolicy = new SharedAccessAccountPolicy()
                {
                    Permissions = SharedAccessAccountPermissions.Add | SharedAccessAccountPermissions.Create | SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write,
                    Protocols = SharedAccessProtocol.HttpsOnly,
                    Services = SharedAccessAccountServices.Blob | SharedAccessAccountServices.Blob,
                    ResourceTypes = SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object,
                    SharedAccessStartTime = DateTimeOffset.UtcNow,
                    SharedAccessExpiryTime = retResult.ExpiryUtc
                };

                retResult.Token = storageAccount.GetSharedAccessSignature(accessPolicy);
            }
            catch (FormatException)
            {
                retResult.Notification.Add("Connection String was not in the correct format");
            }

            return Task.FromResult(retResult);
        }
    }

    public class StorageTokenResult : CommandResult
    {
        public DateTimeOffset ExpiryUtc { get; set; }
        public string Token { get; set; }
    }
}