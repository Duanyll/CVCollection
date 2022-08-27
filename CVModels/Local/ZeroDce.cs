using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Local
{
    internal class ZeroDce : LocalImageProcessingModelBase
    {
        public ZeroDce() : base("ZeroDce", "zero_dce.onnx")
        {
        }

        const int SizeLimit = 800;

        protected override Task<byte[]> OnPreprocessImage(byte[] image)
            => Task.Run(() => SkiaSharpUtils.ZoomToFitSize(image, SizeLimit));
        

        protected override Task<byte[]> OnProcessImage(byte[] image, IProgress<string> progress) => Task.Run(() =>
        {
            progress?.Report("Loading");

            var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("img", TensorUtils.ImageToCHWTensor(image))
                };

            progress?.Report("Inferencing");

            // Run inference
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = Session.Run(inputs);

            progress?.Report("Done");

            return TensorUtils.CHWTensorToImage(results.First().AsTensor<float>());
        });
    }
}
