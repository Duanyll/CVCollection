using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace CVCollection.Service
{
    public static partial class MediaService
    {
        // TODO: The method is not tested.
        public static async partial Task SaveImageBytesToGallery(byte[] image, string fileName)
        {
            if (await Permissions.CheckStatusAsync<Permissions.Photos>() != PermissionStatus.Granted)
            {
                if (await Permissions.RequestAsync<Permissions.Photos>() != PermissionStatus.Granted)
                {
                    throw new PermissionException("Photo permession is not granted.");
                }
            }

            using var imageData = new UIImage(NSData.FromArray(image));
            imageData.SaveToPhotosAlbum((image, error) =>
            {
                //you can retrieve the saved UI Image as well if needed using  
                //var i = image as UIImage;  
                if (error != null)
                {
                    Console.WriteLine(error);
                    MainThread.BeginInvokeOnMainThread(() => App.AlertSvc.ShowAlert("Oops", error.ToString()));
                } 
                else
                {
                    MainThread.BeginInvokeOnMainThread(() => App.AlertSvc.ShowAlert("Image Saved", "Image saved to the gallery."));
                }
            });
        }
    }
}