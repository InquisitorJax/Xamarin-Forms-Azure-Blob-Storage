using Prism.Commands;
using Prism.Mvvm;
using Samples.XamarinForms.AzureBlobStorage.AzureStorage;
using Samples.XamarinForms.AzureBlobStorage.Events;
using Samples.XamarinForms.AzureBlobStorage.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class AddImageViewModel : BindableBase
    {
        private int _imageBoundSize;

        private byte[] _imageFile;
        private string _imageName;

        private bool _isBusy;

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
            set { SetProperty(ref _imageFile, value); }
        }

        public string ImageName
        {
            get { return _imageName; }
            set { SetProperty(ref _imageName, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public ICommand UploadImageCommand { get; private set; }

        private INavigationService Navigation
        {
            get { return DependencyService.Get<INavigationService>(); }
        }

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
            if (IsBusy || ImageFile == null || string.IsNullOrEmpty(ImageName))
                return;

            try
            {
                //using (var memoryStream = imageFile.AsMemoryStream())
                //{
                //    var uploadResult = await StorageService.UploadImageAsync(memoryStream);

                //    if (uploadResult.IsValid())
                //    {
                var imageDocument = new StorageDocument()
                {
                    Name = ImageName,
                    //RemoteStorageFileId = uploadResult.FileId,
                    RemoteStorageFileId = "test_id",
                    File = imageFile,
                    FileType = FileType.Image
                };
                ModelUpdatedMessageEvent<StorageDocument>.Publish(imageDocument, ModelUpdateEvent.Created);

                //    }
                //    else
                //    {
                //        //TODO: show error message
                //    }
                //}
            }
            finally
            {
                IsBusy = false;
            }
            await Navigation.Close();
        }
    }
}