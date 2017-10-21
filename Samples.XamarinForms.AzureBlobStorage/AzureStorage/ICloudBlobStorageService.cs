using Samples.XamarinForms.AzureBlobStorage.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wibci.LogicCommand;

namespace Samples.XamarinForms.AzureBlobStorage.AzureStorage
{
    public interface ICloudBlobStorageService
    {
        //NOTE: This is an opinionated design that assumes a potentially different container per file type
        //see: https://developer.xamarin.com/guides/xamarin-forms/cloud-services/storage/azure-storage/

        Task<bool> DeleteFileAsync(FileType fileType, string name);

        Task<DownloadResult> DownloadFileAsync(FileType fileType, string fileId);

        Task<IList<string>> GetFilesListAsync(FileType fileType);

        Task<UploadResult> UploadFileAsync(FileType fileType, Stream document);
    }

    public interface ICloudBlobStorageSettingsProvider
    {
        Task<CloudStorageSettings> FetchSettingsAsync();
    }

    public class DownloadResult : CommandResult
    {
        public byte[] File { get; set; }
        public string FileId { get; set; }
    }

    public class UploadResult : CommandResult
    {
        public UploadResult(string fileId)
        {
            FileId = fileId;
        }

        public string FileId { get; private set; }
    }
}