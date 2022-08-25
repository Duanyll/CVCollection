using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Local
{
    public abstract class LocalImageProcessingModelBase : LocalModelBase, IImageProcessingModel
    {
        protected LocalImageProcessingModelBase(string name, string modelName) : base(name, modelName)
        {
        }

        protected abstract Task<byte[]> OnPreprocessImage(byte[] image);
        protected abstract Task<byte[]> OnProcessImage(byte[] image, IProgress<string> progress);

        public async Task<byte[]> PreprocessImageAsync(byte[] image)
        {
            await AwaitLastTaskAsync();
            return await OnPreprocessImage(image);
        }

        public async Task<byte[]> ProcessImageAsync(byte[] prepocessedImage, IProgress<string> progress = null)
        {
            await AwaitLastTaskAsync();
            return await OnProcessImage(prepocessedImage, progress);
        }
    }
}
