using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<ITakePictureCommand, TakePictureCommand>();
            DependencyService.Register<IChoosePictureCommand, ChoosePictureCommand>();
            DependencyService.Register<IGenerateInvoiceCommand, GenerateInvoiceCommand>();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
    }
}