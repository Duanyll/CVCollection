using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Remote
{
    public class Demoire : RemoteImageProcessingModelBase
    {
        public Demoire() : base("Demoire", "demoire") { }
        const int RequiredHeight = 1080;
        const int RequiredWidth = 1920;

        protected override Task<byte[]> OnPreprocessImage(byte[] image)
            => Task.Run(() => SkiaSharpUtils.ResizeToDesiredSize(image, RequiredWidth, RequiredHeight));
    }
}
