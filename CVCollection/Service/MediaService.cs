using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVCollection.Service
{
    public static partial class MediaService
    {
        public static partial Task SaveImageBytesToGallery(byte[] image, string fileName);
    }
}
