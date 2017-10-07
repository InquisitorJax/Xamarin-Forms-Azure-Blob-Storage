using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Samples.XamarinForms.AzureBlobStorage.Events;
using Samples.XamarinForms.AzureBlobStorage.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class MainPageViewModel : BindableBase
    {
        private ObservableCollection<StorageDocument> _files;
        private SubscriptionToken _modelUpdateToken;

        public MainPageViewModel()
        {
            AddDocumentCommand = new DelegateCommand(NavigateAddDocument);
            AddImageCommand = new DelegateCommand(NavigateAddImage);
            Files = new ObservableCollection<StorageDocument>();
            _modelUpdateToken = EventMessenger.GetEvent<ModelUpdatedMessageEvent<StorageDocument>>().Subscribe(OnModelUpdated);
        }

        public ICommand AddDocumentCommand { get; private set; }

        public ICommand AddImageCommand { get; private set; }

        public ObservableCollection<StorageDocument> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }
        }

        private IEventAggregator EventMessenger
        {
            get { return DependencyService.Get<IEventAggregator>(DependencyFetchTarget.GlobalInstance); }
        }

        private INavigationService Navigation
        {
            get { return DependencyService.Get<INavigationService>(); }
        }

        private void NavigateAddDocument()
        {
            Navigation.Navigate(NavigationPages.AddDocumentPage);
        }

        private void NavigateAddImage()
        {
            Navigation.Navigate(NavigationPages.AddImagePage);
        }

        private void OnModelUpdated(ModelUpdatedMessageResult<StorageDocument> updateResult)
        {
            if (updateResult.UpdateEvent == ModelUpdateEvent.Created) //only add event support for this example
            {
                Files.Add(updateResult.UpdatedModel);
            }
        }
    }
}