using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using Samples.XamarinForms.AzureBlobStorage.Models;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class FileListViewBehavior : Behavior<SfListView>
    {
        private SfListView _listView;
        protected override void OnAttachedTo(SfListView bindable)
        {
            base.OnAttachedTo(bindable);
            _listView = bindable;
            _listView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = "FileType",
                KeySelector = (object obj1) =>
                {
                    var item = (obj1 as StorageDocument);
                    return item.FileType;
                },
            });
        }
    }
}