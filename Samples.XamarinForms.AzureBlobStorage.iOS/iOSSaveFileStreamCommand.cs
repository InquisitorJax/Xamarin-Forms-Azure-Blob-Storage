﻿using QuickLook;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using Wibci.LogicCommand;

namespace Samples.XamarinForms.AzureBlobStorage.iOS
{
    public class iOSSaveFileStreamCommand : AsyncLogicCommand<SaveFileStreamContext, TaskCommandResult>, ISaveFileStreamCommand
    {
        public override Task<TaskCommandResult> ExecuteAsync(SaveFileStreamContext request)
        {
            //from: https://help.syncfusion.com/file-formats/pdf/working-with-xamarin (broken??)
            var retResult = new TaskCommandResult();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var stream = request.Stream;

            string filePath = Path.Combine(path, request.FileName);

            try
            {
                FileStream fileStream = File.Open(filePath, FileMode.Create);
                stream.Position = 0;
                stream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            catch (Exception e)

            {
            }

            UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            while (currentController.PresentedViewController != null)

                currentController = currentController.PresentedViewController;

            UIView currentView = currentController.View;

            QLPreviewController qlPreview = new QLPreviewController();

            QLPreviewItem item = new QLPreviewItemBundle(request.FileName, filePath);

            qlPreview.DataSource = new PreviewControllerDS(item);

            currentController.PresentViewController(qlPreview, true, null);

            return Task.FromResult(retResult);
        }
    }
}