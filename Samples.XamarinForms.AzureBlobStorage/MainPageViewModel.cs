using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Samples.XamarinForms.AzureBlobStorage.AzureStorage;
using Samples.XamarinForms.AzureBlobStorage.Events;
using Samples.XamarinForms.AzureBlobStorage.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class MainPageViewModel : BindableBase
    {
        private string _busyMessage;
        private ObservableCollection<StorageDocumentContainer> _files;
        private bool _hasFiles;
        private bool _isBusy;
        private SubscriptionToken _modelUpdateToken;

        public MainPageViewModel()
        {
            AddDocumentCommand = new DelegateCommand(NavigateAddDocument);
            AddImageCommand = new DelegateCommand(NavigateAddImage);
            DeleteFileCommand = new DelegateCommand<StorageDocumentContainer>(DeleteFile);
            Files = new ObservableCollection<StorageDocumentContainer>();
            _modelUpdateToken = EventMessenger.GetEvent<ModelUpdatedMessageEvent<StorageDocumentContainer>>().Subscribe(OnModelUpdated);
            FetchStorageFiles();
        }

        public ICommand AddDocumentCommand { get; private set; }

        public ICommand AddImageCommand { get; private set; }

        public string BusyMessage
        {
            get { return _busyMessage; }
            set { SetProperty(ref _busyMessage, value); }
        }

        public ICommand DeleteFileCommand { get; private set; }

        public ObservableCollection<StorageDocumentContainer> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }
        }

        public bool HasFiles
        {
            get { return _hasFiles; }
            set { SetProperty(ref _hasFiles, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private IEventAggregator EventMessenger
        {
            get { return DependencyService.Get<IEventAggregator>(DependencyFetchTarget.GlobalInstance); }
        }

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

        private void CheckHasFiles()
        {
            HasFiles = Files.Any();
        }

        private async void DeleteFile(StorageDocumentContainer fileContainer)
        {
            IsBusy = true;

            try
            {
                var storageSettings = await StorageSettingsProvider.FetchSettingsAsync();
                string container = fileContainer.File.FileType == FileType.Image ? storageSettings.ImageStorageContainerName : storageSettings.DocumentStorageContainerName;
                bool deleteResult = await StorageService.DeleteFileAsync(container, storageSettings.ConnectionString, fileContainer.File.RemoteStorageFileId);
            }
            finally
            {
                IsBusy = false;
                BusyMessage = null;
            }
        }

        private async void FetchStorageFiles()
        {
            IsBusy = true;

            try
            {
                var storageSettings = await StorageSettingsProvider.FetchSettingsAsync();

                BusyMessage = "fetching images...";

                var imageFiles = await StorageService.GetFilesListAsync(storageSettings.ImageStorageContainerName, storageSettings.ConnectionString);

                BusyMessage = "fetching documents...";

                var documentFiles = await StorageService.GetFilesListAsync(storageSettings.DocumentStorageContainerName, storageSettings.ConnectionString);

                BusyMessage = "generating models...";

                var files = new List<StorageDocumentContainer>();

                foreach (var imageFile in imageFiles)
                {
                    var fileContainer = new StorageDocumentContainer(FileType.Image, imageFile);
                    fileContainer.File.Name = imageFile;
                    files.Add(fileContainer);
                }

                foreach (var documentFile in documentFiles)
                {
                    var fileContainer = new StorageDocumentContainer(FileType.Document, documentFile);
                    fileContainer.File.Name = documentFile;
                    files.Add(fileContainer);
                }

                Files = new ObservableCollection<StorageDocumentContainer>(files);

                CheckHasFiles();
            }
            finally
            {
                IsBusy = false;
                BusyMessage = null;
            }
        }

        private void NavigateAddDocument()
        {
            Navigation.Navigate(NavigationPages.AddDocumentPage);
        }

        private void NavigateAddImage()
        {
            Navigation.Navigate(NavigationPages.AddImagePage);
        }

        private void OnModelUpdated(ModelUpdatedMessageResult<StorageDocumentContainer> updateResult)
        {
            if (updateResult.UpdateEvent == ModelUpdateEvent.Created) //only add event support for this example
            {
                Files.Add(updateResult.UpdatedModel);
                CheckHasFiles();
            }
        }
    }
}