using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public interface INavigationService
    {
        Task Close();

        Task CloseAll();

        Task Navigate(string destination, object arg = null);
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

        public async Task Navigate(string destination, object arg = null)
        {
            switch (destination)
            {
                case NavigationPages.AddImagePage:
                    var imagePage = new AddImagePage();
                    await Application.Current.MainPage.Navigation.PushAsync(imagePage);
                    break;

                case NavigationPages.ViewImagePage:
                    var viewImagePage = new ViewImagePage();
                    ((INavigableView)viewImagePage).SendArgs(arg);
                    await Application.Current.MainPage.Navigation.PushAsync(viewImagePage);
                    break;

                case NavigationPages.AddDocumentPage:
                    var documentPage = new AddDocumentPage();
                    await Application.Current.MainPage.Navigation.PushAsync(documentPage);
                    break;

                case NavigationPages.ViewDocumentPage:
                    var viewDocumentPage = new ViewDocumentPage();
                    ((INavigableView)viewDocumentPage).SendArgs(arg);
                    await Application.Current.MainPage.Navigation.PushAsync(viewDocumentPage);
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
        public const string ViewDocumentPage = "ViewDocumentPage";
        public const string ViewImagePage = "ViewImagePage";
    }
}