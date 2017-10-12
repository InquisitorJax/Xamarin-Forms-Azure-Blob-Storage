using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public class SfListViewEx : SfListView
    {
        public static BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(SfListViewEx), null);

        public static BindableProperty LeftSwipeCommandProperty = BindableProperty.Create(nameof(LeftSwipeCommand), typeof(ICommand), typeof(SfListViewEx), null);
        public static BindableProperty RightSwipeCommandProperty = BindableProperty.Create(nameof(RightSwipeCommand), typeof(ICommand), typeof(SfListViewEx), null);

        private bool _isSwiping;

        public SfListViewEx()
        {
            ItemTapped += SfListViewEx_ItemTapped;
            SwipeStarted += SfListViewEx_SwipeStarted;
            SwipeEnded += SfListViewEx_SwipeEnded;
        }

        public ICommand ItemTappedCommand
        {
            get { return (ICommand)this.GetValue(ItemTappedCommandProperty); }
            set { this.SetValue(ItemTappedCommandProperty, value); }
        }

        public ICommand LeftSwipeCommand
        {
            get { return (ICommand)this.GetValue(LeftSwipeCommandProperty); }
            set { this.SetValue(LeftSwipeCommandProperty, value); }
        }

        public ICommand RightSwipeCommand
        {
            get { return (ICommand)this.GetValue(RightSwipeCommandProperty); }
            set { this.SetValue(RightSwipeCommandProperty, value); }
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

        private void SfListViewEx_SwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            if (e.ItemData != null && e.SwipeOffset > 180)
            {
                if (e.SwipeDirection == SwipeDirection.Left && RightSwipeCommand != null && RightSwipeCommand.CanExecute(e.ItemData))
                {
                    RightSwipeCommand.Execute(e.ItemData);
                    ResetSwipe();
                    e.SwipeOffset = 0; //BUG in ResetSwipe() see: https://www.syncfusion.com/forums/129974/resetswipe-method-doesn39t-hide-swipetemplate
                }
                else if (e.SwipeDirection == SwipeDirection.Right && LeftSwipeCommand != null && LeftSwipeCommand.CanExecute(e.ItemData))
                {
                    LeftSwipeCommand.Execute(e.ItemData);
                    ResetSwipe();
                    e.SwipeOffset = 0;
                }
            }
            System.Threading.Tasks.Task.Delay(500).ContinueWith((task) =>
            {
                _isSwiping = false;
            });
        }

        private void SfListViewEx_SwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            _isSwiping = true;
        }
    }
}