﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wibci.LogicCommand;

namespace Samples.XamarinForms.AzureBlobStorage.AzureStorage
{
    public interface ICloudBlobStorageService
    {
        //see: https://developer.xamarin.com/guides/xamarin-forms/cloud-services/storage/azure-storage/

        Task<bool> DeleteFileAsync(string containerName, string connectionString, string name);

        Task<DownloadResult> DownloadDocumentAsync(string fileId);

        Task<DownloadResult> DownloadImageAsync(string fileId);

        Task<IList<string>> GetFilesListAsync(string containerName, string connectionString);

        Task<UploadResult> UploadDocumentAsync(Stream document);

        Task<UploadResult> UploadImageAsync(Stream image);
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