using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples.XamarinForms.AzureBlobStorage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(1000);
            _lblLonely.FadeTo(1, 750, Easing.Linear);
            await Task.Delay(1000);
            _btnUploadImage.FadeTo(1, 750, Easing.Linear);
            _btnUploadDocument.FadeTo(1, 750, Easing.Linear);
        }
    }
}