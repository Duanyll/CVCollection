using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels
{
    public interface IImageProcessingModel
    {
        string Name { get; }
        
        Task<byte[]> PreprocessImageAsync(byte[] image);
        Task<byte[]> ProcessImageAsync(byte[] prepocessedImage);
    }
}
