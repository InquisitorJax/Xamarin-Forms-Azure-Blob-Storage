using Foundation;
using Plugin.Media;
using Syncfusion.SfPdfViewer.XForms.iOS;
using UIKit;
using Wibci.Xamarin.Forms.Converters;
using Wibci.Xamarin.Images;
using Wibci.Xamarin.Images.iOS;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var converter = new BooleanToInvertedBooleanConverter(); //seems assembly cannot be found if type not instantiated before app loads :|

            Xamarin.Forms.DependencyService.Register<IAnalyseImageCommand, iOSAnalyseImageCommand>();
            CrossMedia.Current.Initialize();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            new SfPdfDocumentViewRenderer();

            DependencyService.Register<ISaveFileStreamCommand, iOSSaveFileStreamCommand>();
            DependencyService.Register<IResizeImageCommand, iOSImageResizeCommand>();

            return base.FinishedLaunching(app, options);
        }
    }
}