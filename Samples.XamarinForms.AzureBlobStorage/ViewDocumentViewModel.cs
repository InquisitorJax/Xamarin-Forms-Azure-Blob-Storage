using Prism.Commands;
using System.IO;
using System.Windows.Input;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class ViewDocumentViewModel : ViewModelBase
    {
        private byte[] _document;

        private Stream _documentFileStream;

        public ViewDocumentViewModel()
        {
            CloseCommand = new DelegateCommand(Close);
        }

        public ICommand CloseCommand { get; private set; }

        public byte[] Document
        {
            get { return _document; }
            set
            {
                SetProperty(ref _document, value);
                DocumentFileStream = _document == null ? null : _document.AsMemoryStream();
            }
        }

        public Stream DocumentFileStream
        {
            get { return _documentFileStream; }
            set { SetProperty(ref _documentFileStream, value); }
        }

        protected override void ApplyArgs(object args)
        {
            Document = args as byte[];
        }

        private void Close()
        {
            Navigation.Close();
        }
    }
}