using System;
using System.Threading.Tasks;

namespace Samples.XamarinForms.AzureBlobStorage.AppServices
{
    public interface IDialogService
    {
        Task<string> DisplayActionSheetAsync(String title, String cancel, String destruction, params String[] buttons);

        Task DisplayAlertAsync(String title, String message, String cancel);
    }

    public class DialogService : IDialogService
    {
        public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
        }

        public async Task DisplayAlertAsync(string title, string message, string cancel)
        {
            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}