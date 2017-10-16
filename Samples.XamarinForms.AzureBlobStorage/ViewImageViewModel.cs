using Prism.Commands;
using System.Windows.Input;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class ViewImageViewModel : ViewModelBase
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

        protected override void ApplyArgs(object args)
        {
            Image = args as byte[];
        }

        private void Close()
        {
            Navigation.Close();
        }
    }
}