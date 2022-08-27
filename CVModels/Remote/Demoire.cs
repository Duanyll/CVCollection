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
        const int SizeLimit = 1920;

        protected override Task<byte[]> OnPreprocessImage(byte[] image)
            => Task.Run(() => SkiaSharpUtils.ZoomToFitSize(image, SizeLimit));
    }
}
