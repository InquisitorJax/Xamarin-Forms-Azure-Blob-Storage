using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Samples.XamarinForms.AzureBlobStorage.Models;
using System.Globalization;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class FileTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retFileTypeName = null;

            if (value != null && value.GetType() == typeof(int))
            {
                int fileType = (int)value;

                switch (fileType)
                {
                    case (int)FileType.Document:
                        retFileTypeName = "Documents";
                        break;

                    case (int)FileType.Image:
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