using Prism.Commands;
using Samples.XamarinForms.AzureBlobStorage.AzureStorage;
using Samples.XamarinForms.AzureBlobStorage.Events;
using Samples.XamarinForms.AzureBlobStorage.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Wibci.LogicCommand;
using Wibci.Xamarin.Images;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class AddDocumentViewModel : ViewModelBase
    {
        private uint _logoHeight;
        private uint _logoWidth;
        private Invoice _model;

        private int _numberOfInvoiceItems;
        private bool _useCamera;

        private bool _useSimpleTable;

        public AddDocumentViewModel()
        {
            Model = new Invoice();
            NumberOfInvoiceItems = 30;
            Model.GenDefault(NumberOfInvoiceItems);
            GenerateInvoiceCommand = new DelegateCommand(GenerateInvoice);
            SelectPictureCommand = new DelegateCommand(SelectPicture);
            UseSimpleTable = true;
        }

        public ICommand GenerateInvoiceCommand { get; }

        public Invoice Model
        {
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }

        public int NumberOfInvoiceItems
        {
            get { return _numberOfInvoiceItems; }
            set { SetProperty(ref _numberOfInvoiceItems, value); }
        }

        public ICommand SelectPictureCommand { get; }

        public bool UseCamera
        {
            get { return _useCamera; }
            set { SetProperty(ref _useCamera, value); }
        }

        public bool UseSimpleTable
        {
            get { return _useSimpleTable; }
            set { SetProperty(ref _useSimpleTable, value); }
        }

        private ICloudBlobStorageService StorageService
        {
            get { return DependencyService.Get<ICloudBlobStorageService>(); }
        }

        private async void GenerateInvoice()
        {
            var generateCommand = DependencyService.Get<IGenerateInvoiceCommand>();

            Model.GenerateItems(NumberOfInvoiceItems);

            var context = new GenerateInvoiceContext
            {
                FileName = "syncfusionInvoice.pdf",
                Invoice = Model,
                LogoHeight = _logoHeight,
                LogoWidth = _logoWidth,
                SimpleFormat = false, //simple format doesn't generate line items for each invoice item
                SimpleTableItems = UseSimpleTable //when SimpleFormat = false - choose what kind of table to use to generate the items !simple = use pdfGrid, else use SimpleTable
            };

            var result = await generateCommand.ExecuteAsync(context);

            if (!result.IsValid() || result.TaskResult != TaskResult.Success)
            {
                Debug.WriteLine($"Generate Invoice FAILED! {result.Notification.ToString()}");
            }
            else
            {
                await UploadDocumentAsync(result.PdfResult, context.FileName);
            }
        }

        private async void SelectPicture()
        {
            SelectPictureResult pictureResult = null;
            if (UseCamera)
            {
                var takePicture = DependencyService.Get<ITakePictureCommand>();
                var request = new TakePictureRequest { MaxPixelDimension = 130, CameraOption = CameraOption.Back };
                pictureResult = await takePicture.ExecuteAsync(request);
            }
            else
            {
                var choosePicture = DependencyService.Get<IChoosePictureCommand>();
                var request = new ChoosePictureRequest { MaxPixelDimension = 130 };
                pictureResult = await choosePicture.ExecuteAsync(request);
            }

            if (pictureResult.TaskResult == TaskResult.Success)
            {
                var analyseImage = DependencyService.Get<IAnalyseImageCommand>();
                var analyseResult = await analyseImage.ExecuteAsync(new AnalyseImageContext { Image = pictureResult.Image });
                if (analyseResult.IsValid())
                {
                    Model.Logo = pictureResult.Image;
                    _logoWidth = analyseResult.Width;
                    _logoHeight = analyseResult.Height;
                }
                else
                {
                    Model.Logo = pictureResult.Image;
                }
            }
        }

        private async Task UploadDocumentAsync(byte[] document, string documentName)
        {
            if (IsBusy || document == null)
                return;

            IsBusy = true;
            BusyMessage = "upoading document...";

            try
            {
                using (var memoryStream = document.AsMemoryStream())
                {
                    var uploadResult = await StorageService.UploadFileAsync(FileType.Document, memoryStream);

                    if (uploadResult.IsValid())
                    {
                        var file = new StorageDocument()
                        {
                            Name = documentName,
                            RemoteStorageFileId = uploadResult.FileId,
                            //RemoteStorageFileId = "test_id",
                            File = document,
                            FileType = FileType.Document
                        };

                        ModelUpdatedMessageEvent<StorageDocument>.Publish(file, ModelUpdateEvent.Created);
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