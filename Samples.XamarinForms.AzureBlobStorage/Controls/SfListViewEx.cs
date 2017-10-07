using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class SfListViewEx : SfListView
    {
        public static BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(SfListViewEx), null);

        public SfListViewEx()
        {
            this.ItemTapped += SfListViewEx_ItemTapped;
        }

        public ICommand ItemTappedCommand
        {
            get { return (ICommand)this.GetValue(ItemTappedCommandProperty); }
            set { this.SetValue(ItemTappedCommandProperty, value); }
        }

        private void SfListViewEx_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            if (e.ItemData != null)
            {
                if (e.ItemData.GetType() != typeof(GroupResult))
                {
                    if (this.ItemTappedCommand != null && this.ItemTappedCommand.CanExecute(e.ItemData))
                    {
                        ItemTappedCommand.Execute(e.ItemData);
                    }
                }
            }
        }
    }
}