using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples.XamarinForms.AzureBlobStorage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDocumentPage : ContentPage
    {
        public AddDocumentPage()
        {
            InitializeComponent();
            BindingContext = new AddDocumentViewModel();
        }
    }
}