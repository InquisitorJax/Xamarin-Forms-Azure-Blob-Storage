using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public interface INavigationService
    {
        Task Close();

        Task CloseAll();

        Task Navigate(string destination);
    }

    public class HackedNavigationService : INavigationService
    {
        public async Task Close()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public async Task CloseAll()
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        public async Task Navigate(string destination)
        {
            switch (destination)
            {
                case NavigationPages.AddImagePage:
                    var imagePage = new AddImagePage();
                    await Application.Current.MainPage.Navigation.PushAsync(imagePage);
                    break;

                case NavigationPages.AddDocumentPage:
                    var documentPage = new AddDocumentPage();
                    await Application.Current.MainPage.Navigation.PushAsync(documentPage);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class NavigationPages
    {
        public const string AddDocumentPage = "AddDocumentPage";
        public const string AddImagePage = "AddImagePage";
    }
}