using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVCollection.Service
{
    public static partial class MediaService
    {
        public static async partial Task SaveImageBytesToGallery(byte[] image, string fileName)
        {
            if (await Permissions.CheckStatusAsync<Permissions.StorageWrite>() != PermissionStatus.Granted)
            {
                if (await Permissions.RequestAsync<Permissions.StorageWrite>() != PermissionStatus.Granted)
                {
                    throw new PermissionException("Write storage permession is not granted.");
                }
            }

            var dir = Path.Combine("/storage/emulated/0/Pictures/CVCollection");
            var path = Path.Combine(dir, fileName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllBytesAsync(path, image);

            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));
            Android.App.Application.Context.SendBroadcast(mediaScanIntent);

            MainThread.BeginInvokeOnMainThread(() => App.AlertSvc.ShowAlert("Image Saved", "Image saved to the gallery."));
        }
    }
}