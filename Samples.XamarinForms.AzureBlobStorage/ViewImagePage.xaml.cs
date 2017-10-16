using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples.XamarinForms.AzureBlobStorage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewImagePage : ContentPage, INavigableView
    {
        public ViewImagePage()
        {
            InitializeComponent();
            BindingContext = new ViewImageViewModel();
        }

        public void SendArgs(object args)
        {
            var argViewModel = BindingContext as IViewModel;

            if (argViewModel != null)
            {
                argViewModel.AcceptArgs(args);
            }
        }
    }
}