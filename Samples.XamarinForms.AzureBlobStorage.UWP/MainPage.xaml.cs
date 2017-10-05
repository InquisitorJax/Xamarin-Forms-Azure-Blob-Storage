using Wibci.Xamarin.Images;
using Wibci.Xamarin.Images.UWP;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Samples.XamarinForms.AzureBlobStorage.App());

            DependencyService.Register<ISaveFileStreamCommand, UWPSaveFileStreamCommand>();
            DependencyService.Register<IResizeImageCommand, UWPResizeImageCommand>();
        }
    }
}