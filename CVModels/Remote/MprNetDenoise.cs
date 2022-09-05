using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Remote
{
    internal class MprNetDenoise : RemoteImageProcessingModelBase
    {
        public MprNetDenoise() : base("MprNetDenoise", "denoise")
        {
        }

        const int SizeLimit = 800;

        protected override Task<byte[]> OnPreprocessImage(byte[] image)
            => Task.Run(() => SkiaSharpUtils.ZoomToFitSize(image, SizeLimit));
    }
}
