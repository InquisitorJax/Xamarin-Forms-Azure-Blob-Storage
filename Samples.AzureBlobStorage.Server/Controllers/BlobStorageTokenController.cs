using Microsoft.AspNetCore.Mvc;
using Samples.AzureBlobStorage.Server.AzureStorage;
using Samples.XamarinForms.AzureBlobStorage.AzureStorage;
using System.Threading.Tasks;

namespace Samples.AzureBlobStorage.Server.Controllers
{
    [Route("api/[controller]")]
    public class BlobStorageTokenController : Controller
    {
        private readonly IFetchBlobStorageSettingsCommand _storageSettingsCommand;

        public BlobStorageTokenController(IFetchBlobStorageSettingsCommand storageSettingsCommand)
        {
            _storageSettingsCommand = storageSettingsCommand;
        }

        // GET api/values
        [HttpGet]
        //[Authorize] //TODO: Must be behind authenticated endpoint!!!
        public async Task<CloudStorageSettings> Get()
        {
            var result = await _storageSettingsCommand.ExecuteAsync(null);

            return result.StorageSettings;
        }
    }
}