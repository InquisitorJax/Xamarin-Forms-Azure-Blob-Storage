using Microsoft.WindowsAzure.Storage;
using Prism.Commands;
using Prism.Events;
using Samples.XamarinForms.AzureBlobStorage.AzureStorage;
using Samples.XamarinForms.AzureBlobStorage.Events;
using Samples.XamarinForms.AzureBlobStorage.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class MainPageViewModel : ViewModelBase
    {
        private ObservableCollection<StorageDocumentContainer> _files;
        private bool _hasFiles;

        private SubscriptionToken _modelUpdateToken;

        public MainPageViewModel()
        {
            HasFiles = true;
            AddDocumentCommand = new DelegateCommand(NavigateAddDocument);
            AddImageCommand = new DelegateCommand(NavigateAddImage);
            DeleteFileCommand = new DelegateCommand<StorageDocumentContainer>(DeleteFile);
            FetchFilesCommand = new DelegateCommand(FetchStorageFiles);
            Files = new ObservableCollection<StorageDocumentContainer>();
            _modelUpdateToken = EventMessenger.GetEvent<ModelUpdatedMessageEvent<StorageDocument>>().Subscribe(OnModelUpdated);
            FetchStorageFiles();
        }

        public ICommand AddDocumentCommand { get; private set; }

        public ICommand AddImageCommand { get; private set; }

        public ICommand DeleteFileCommand { get; private set; }

        public ICommand FetchFilesCommand { get; private set; }

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

        private IEventAggregator EventMessenger
        {
            get { return DependencyService.Get<IEventAggregator>(DependencyFetchTarget.GlobalInstance); }
        }

        private ICloudBlobStorageService StorageService
        {
            get { return DependencyService.Get<ICloudBlobStorageService>(); }
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
                await StorageService.DeleteFileAsync(fileContainer.File.FileType, fileContainer.File.RemoteStorageFileId);
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
                BusyMessage = "fetching images...";

                var imageFiles = await StorageService.GetFilesListAsync(FileType.Image);

                BusyMessage = "fetching documents...";

                var documentFiles = await StorageService.GetFilesListAsync(FileType.Document);

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
            catch (StorageException se)
            {
                //DialogService.DisplayAlert(se.message)
                Debug.WriteLine(se.Message);
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

        private void OnModelUpdated(ModelUpdatedMessageResult<StorageDocument> updateResult)
        {
            if (updateResult.UpdateEvent == ModelUpdateEvent.Created) //only add event support for this example
            {
                var container = new StorageDocumentContainer(updateResult.UpdatedModel.FileType, updateResult.UpdatedModel.RemoteStorageFileId)
                {
                    File = updateResult.UpdatedModel
                };

                Files.Add(container);
                CheckHasFiles();
            }
        }
    }
}