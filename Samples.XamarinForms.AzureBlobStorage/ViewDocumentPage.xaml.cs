using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples.XamarinForms.AzureBlobStorage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewDocumentPage : ContentPage, INavigableView
    {
        public ViewDocumentPage()
        {
            InitializeComponent();
            BindingContext = new ViewDocumentViewModel();
        }

        public void SendArgs(object args)
        {
            ((IViewModel)BindingContext).AcceptArgs(args);
        }
    }
}