using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples.XamarinForms.AzureBlobStorage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddImagePage : ContentPage
    {
        public AddImagePage()
        {
            InitializeComponent();
            BindingContext = new AddImageViewModel();
        }
    }
}