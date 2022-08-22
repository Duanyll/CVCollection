using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.LocalModels
{
    public class Derain : LocalImageProcessingModelBase
    {
        public Derain() : base("Derain", "derain_se.onnx") { }

        const int RequiredHeight = 320;
        const int RequiredWidth = 480;

        protected override Task<byte[]> OnPreprocessImage(byte[] image)
            => Task.Run(() => SkiaSharpUtils.ResizeToDesiredSize(image, RequiredWidth, RequiredHeight));

        protected override Task<byte[]> OnProcessImage(byte[] image)
            => Task.Run(() =>
            {
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("img", TensorUtils.ImageToCHWTensor(image, RequiredWidth, RequiredHeight))
                };

                // Run inference
                using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = Session.Run(inputs);

                return TensorUtils.CHWTensorToImage(results.First().AsTensor<float>());
            });
    }
}
