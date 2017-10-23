# Azure Storage

This sample demonstrates how to use Xamarin.Forms to store binary data in Azure Storage, and how to access the data.

also some sample code on using the xamarin pdf component from syncfusion

NOTE: Setting up syncfusion nuget packages in Visual Studio
https://help.syncfusion.com/xamarin/introduction/download-and-installation#configuring-syncfusion-nuget-packages-in-visual-studio

## Using the local storage emulator

- Set the UWP project as startup. 
*NOTE: Android emulator connection to local storage emulator not currently working
- Make sure to create containers "image-container" and "document-container" using the Azure Storage Explorer
- Make sure the the storage emulator is started and running

In the App class of the shared lib:
- Configure ICloudBlobStorageSettingsProvider to resolve to LocalBlobStorageSettingsProvider
- RUN the UWP project

### Using an ad-hoc generated SAS Token instead of a shared key

![logo]

[logo]: https://docs.microsoft.com/en-us/azure/storage/common/media/storage-dotnet-shared-access-signature-part-1/sas-storage-provider-service.png

- Configure ICloudBlobStorageSettingsProvider to resolve to SasTokenBlobStorageSettings
- Run the ASP dotnet core project in Development run configuration (which points to local emulator)
- Run the UWP Project

## Using Azure Storage

- Setup an azure storage account with the 2 containers: "image-container" and "document-container"
- Android App preferred (UWP app has some navigation issues)
- Create a class "YourAzureStorageSettings" that returns CloudStorageSettings with ConnectionString that reflects the azure storage account

## Reference Links
[Create Azure Storage Account](https://docs.microsoft.com/en-us/azure/storage/storage-create-storage-account#create-a-storage-account)

[Azure Storage Emulator for local testing](https://docs.microsoft.com/en-us/azure/storage/storage-use-emulator)

[Azure Storage in ASP NET CORE](https://wildermuth.com/2017/10/14/Using-Azure-Storage-in-ASP-NET-Core)

[Azure Storage: Using shared access signatures (SAS)](https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1)

[Xamain: Storing and Accessing Data in Azure Storage](https://developer.xamarin.com/guides/xamarin-forms/cloud-services/storage/azure-storage/)



