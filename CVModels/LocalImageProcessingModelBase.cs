using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels
{
    public abstract class LocalImageProcessingModelBase : LocalModelBase, IImageProcessingModel
    {
        protected LocalImageProcessingModelBase(string name, string modelName) : base(name, modelName)
        {
        }

        protected abstract Task<byte[]> OnPreprocessImage(byte[] image);
        protected abstract Task<byte[]> OnProcessImage(byte[] image);

        public async Task<byte[]> PreprocessImageAsync(byte[] image)
        {
            await AwaitLastTaskAsync().ConfigureAwait(false);
            return await OnPreprocessImage(image);
        }

        public async Task<byte[]> ProcessImageAsync(byte[] prepocessedImage)
        {
            await AwaitLastTaskAsync().ConfigureAwait(false);
            return await OnProcessImage(prepocessedImage);
        }
    }
}
