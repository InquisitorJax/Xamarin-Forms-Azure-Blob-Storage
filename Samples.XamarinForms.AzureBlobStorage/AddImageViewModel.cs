using Prism.Commands;
using Samples.XamarinForms.AzureBlobStorage.AzureStorage;
using Samples.XamarinForms.AzureBlobStorage.Events;
using Samples.XamarinForms.AzureBlobStorage.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class AddImageViewModel : ViewModelBase
    {
        private int _imageBoundSize;

        private byte[] _imageFile;
        private string _imageName;

        public AddImageViewModel()
        {
            AddImageFromDeviceCommand = new DelegateCommand(AddImageFromDevice);
            AddImageFromCameraCommand = new DelegateCommand(AddImageFromCamera);
            UploadImageCommand = new DelegateCommand(UploadImage);
            ImageBoundSize = 300;
        }

        public ICommand AddImageFromCameraCommand { get; private set; }

        public ICommand AddImageFromDeviceCommand { get; private set; }

        public int ImageBoundSize
        {
            get { return _imageBoundSize; }
            set { SetProperty(ref _imageBoundSize, value); }
        }

        public byte[] ImageFile
        {
            get { return _imageFile; }
            set
            {
                SetProperty(ref _imageFile, value);
                RaisePropertyChanged("ShowUpload");
            }
        }

        public string ImageName
        {
            get { return _imageName; }
            set { SetProperty(ref _imageName, value); }
        }

        public bool ShowUpload
        {
            get { return !IsBusy && ImageFile != null; }
        }

        public ICommand UploadImageCommand { get; private set; }

        private ICloudBlobStorageService StorageService
        {
            get { return DependencyService.Get<ICloudBlobStorageService>(); }
        }

        private void AddImageFromCamera()
        {
            SelectPicture(true);
        }

        private void AddImageFromDevice()
        {
            SelectPicture(false);
        }

        private async void SelectPicture(bool useCamera)
        {
            SelectPictureResult pictureResult = null;
            if (useCamera)
            {
                var takePicture = DependencyService.Get<ITakePictureCommand>();
                var request = new TakePictureRequest { MaxPixelDimension = ImageBoundSize, CameraOption = CameraOption.Back };
                pictureResult = await takePicture.ExecuteAsync(request);
            }
            else
            {
                var choosePicture = DependencyService.Get<IChoosePictureCommand>();
                var request = new ChoosePictureRequest { MaxPixelDimension = ImageBoundSize };
                pictureResult = await choosePicture.ExecuteAsync(request);
            }

            if (pictureResult.TaskResult == TaskResult.Success)
            {
                ImageFile = pictureResult.Image;
            }
        }

        private async void UploadImage()
        {
            await UploadImageAsync(ImageFile);
        }

        private async Task UploadImageAsync(byte[] imageFile)
        {
            if (IsBusy || ImageFile == null)
                return;

            IsBusy = true;
            BusyMessage = "upoading image...";

            try
            {
                using (var memoryStream = imageFile.AsMemoryStream())
                {
                    var uploadResult = await StorageService.UploadImageAsync(memoryStream);

                    if (uploadResult.IsValid())
                    {
                        var imageDocument = new StorageDocument()
                        {
                            Name = ImageName,
                            RemoteStorageFileId = uploadResult.FileId,
                            //RemoteStorageFileId = "test_id",
                            File = imageFile,
                            FileType = FileType.Image
                        };

                        ModelUpdatedMessageEvent<StorageDocument>.Publish(imageDocument, ModelUpdateEvent.Created);
                        await Navigation.Close();
                    }
                    else
                    {
                        await Dialog.DisplayAlertAsync("Error", uploadResult.Notification.ToString(), "close");
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}