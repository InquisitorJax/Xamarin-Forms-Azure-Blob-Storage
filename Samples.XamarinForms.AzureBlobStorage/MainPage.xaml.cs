using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }
    }
}