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
        protected override Task<byte[]> OnPreprocessImage(byte[] image)
        {
            return Task.FromResult(image);
        }
    }
}
