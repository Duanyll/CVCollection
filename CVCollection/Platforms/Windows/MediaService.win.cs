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
            var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            await File.WriteAllBytesAsync(fullPath, image);
            App.AlertSvc.ShowAlert("Image Saved", $"Image saved to desktop.");
        }
    }
}