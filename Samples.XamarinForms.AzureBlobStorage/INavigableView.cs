using System.ComponentModel;

namespace Samples.XamarinForms.AzureBlobStorage
{
    //HACK for sending navigation args
    public interface INavigableView
    {
        void SendArgs(object args);
    }

    public interface IViewModel : INotifyPropertyChanged
    {
        void SendArgs(object args);
    }
}