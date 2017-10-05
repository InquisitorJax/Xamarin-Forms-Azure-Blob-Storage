using System.IO;
using Wibci.LogicCommand;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public interface ISaveFileStreamCommand : IAsyncLogicCommand<SaveFileStreamContext, TaskCommandResult>
    {
    }

    public class SaveFileStreamContext
    {
        public SaveFileStreamContext()
        {
            LaunchFile = true;
        }

        public string ContentType { get; set; }
        public string FileName { get; set; }
        public bool LaunchFile { get; set; }
        public MemoryStream Stream { get; set; }
    }
}