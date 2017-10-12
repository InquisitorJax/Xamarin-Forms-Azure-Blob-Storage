using Samples.XamarinForms.AzureBlobStorage.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class FileTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retFileTypeName = null;

            if (value != null && value.GetType() == typeof(FileType))
            {
                var fileType = (FileType)value;

                switch (fileType)
                {
                    case FileType.Document:
                        retFileTypeName = "Documents";
                        break;

                    case FileType.Image:
                        retFileTypeName = "Images";
                        break;
                }
            }

            return retFileTypeName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}