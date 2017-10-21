using Prism.Commands;
using Prism.Mvvm;
using Samples.XamarinForms.AzureBlobStorage.AzureStorage;
using System.Threading.Tasks;
using System.Windows.Input;
using Validation;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage.Models
{
    public class StorageDocumentContainer : BindableBase
    {
        private bool _downloadingFile;
        private StorageDocument _file;

        public StorageDocumentContainer(FileType fileType, string fileId)
        {
            Requires.NotNull(fileId, nameof(fileId));

            File = new StorageDocument { FileType = fileType, RemoteStorageFileId = fileId };
            DownloadFileCommand = new DelegateCommand(DownloadFile);
            ViewFileCommand = new DelegateCommand(ViewFile);
        }

        public ICommand DownloadFileCommand { get; }

        public bool DownloadingFile
        {
            get { return _downloadingFile; }
            set { SetProperty(ref _downloadingFile, value); }
        }

        public StorageDocument File
        {
            get { return _file; }
            set { SetProperty(ref _file, value); }
        }

        public bool IsDownloaded
        {
            get
            {
                return File != null && File.File != null;
            }
        }

        public ICommand ViewFileCommand { get; }

        private INavigationService Navigation
        {
            get { return DependencyService.Get<INavigationService>(); }
        }

        private ICloudBlobStorageService StorageService
        {
            get { return DependencyService.Get<ICloudBlobStorageService>(); }
        }

        private ICloudBlobStorageSettingsProvider StorageSettingsProvider
        {
            get { return DependencyService.Get<ICloudBlobStorageSettingsProvider>(); }
        }

        public async Task<Notification> DownloadFileAsync(string fileId)
        {
            DownloadResult fileResult;

            DownloadingFile = true;

            try
            {
                fileResult = await StorageService.DownloadFileAsync(_file.FileType, _file.RemoteStorageFileId);

                if (fileResult.IsValid())
                {
                    _file.File = fileResult.File;
                    RaisePropertyChanged("IsDownloaded");
                }

                return fileResult.Notification;
            }
            finally
            {
                DownloadingFile = false;
            }
        }

        private async void DownloadFile()
        {
            await DownloadFileAsync(File.RemoteStorageFileId);
        }

        private void ViewFile()
        {
            if (_file != null && _file.File != null)
            {
                string page = _file.FileType == FileType.Image ? NavigationPages.ViewImagePage : NavigationPages.ViewDocumentPage;
                Navigation.Navigate(page, _file.File);
            }
        }
    }
}