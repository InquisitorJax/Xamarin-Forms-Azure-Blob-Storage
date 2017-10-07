using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Media;
using Plugin.Permissions;
using Wibci.Xamarin.Images;
using Wibci.Xamarin.Images.Droid;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage.Droid
{
    [Activity(Label = "Samples.XamarinForms.AzureBlobStorage", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.DependencyService.Register<IAnalyseImageCommand, AndroidAnalyseImageCommand>();
            CrossMedia.Current.Initialize();
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            DependencyService.Register<ISaveFileStreamCommand, AndroidSaveFileStreamCommand>();
            DependencyService.Register<IResizeImageCommand, AndroidResizeImageCommand>();
        }
    }
}