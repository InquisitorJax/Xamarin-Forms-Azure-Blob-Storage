using Prism.Mvvm;
using Samples.XamarinForms.AzureBlobStorage.AppServices;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class ViewModelBase : BindableBase, IViewModel
    {
        private string _busyMessage;
        private bool _isBusy;

        public string BusyMessage
        {
            get { return _busyMessage; }
            set { SetProperty(ref _busyMessage, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        protected IDialogService Dialog
        {
            get { return DependencyService.Get<IDialogService>(); }
        }

        protected INavigationService Navigation
        {
            get { return DependencyService.Get<INavigationService>(); }
        }

        public void AcceptArgs(object args)
        {
            ApplyArgs(args);
        }

        protected virtual void ApplyArgs(object args)
        {
        }
    }
}