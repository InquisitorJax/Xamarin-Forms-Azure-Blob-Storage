using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class ViewImageViewModel : BindableBase, IViewModel
    {
        private byte[] _image;

        public ViewImageViewModel()
        {
            CloseCommand = new DelegateCommand(Close);
        }

        public ICommand CloseCommand { get; private set; }

        public byte[] Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private INavigationService Navigation
        {
            get { return DependencyService.Get<INavigationService>(); }
        }

        public void SendArgs(object args)
        {
            Image = args as byte[];
        }

        private void Close()
        {
            Navigation.Close();
        }
    }
}