using System.IO;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public static class ByteArrayExtensions
    {
        public static MemoryStream AsMemoryStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }
    }
}