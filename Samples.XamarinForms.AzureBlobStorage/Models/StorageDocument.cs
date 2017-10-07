using Prism.Mvvm;

namespace Samples.XamarinForms.AzureBlobStorage.Models
{
    public enum FileType
    {
        Document,
        Image
    }

    public class StorageDocument : BindableBase
    {
        private byte[] _file;
        private FileType _fileType;
        private string _name;
        private string _remoteStorageFileId;

        public byte[] File
        {
            get { return _file; }
            set { SetProperty(ref _file, value); }
        }

        public FileType FileType
        {
            get { return _fileType; }
            set { SetProperty(ref _fileType, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string RemoteStorageFileId
        {
            get { return _remoteStorageFileId; }
            set { SetProperty(ref _remoteStorageFileId, value); }
        }
    }
}