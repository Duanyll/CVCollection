using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Local
{
    public class Derain : IImageProcessingModel
    {
        ILocalSession Session { get; } = new EmbbededModelSession("derain_se.onnx");
        
        const int RequiredHeight = 320;
        const int RequiredWidth = 480;

        public Task<byte[]> PreprocessImageAsync(byte[] image)
            => Task.Run(() => SkiaSharpUtils.StretchToDesiredSize(image, RequiredWidth, RequiredHeight));

        public Task<byte[]> ProcessImageAsync(byte[] image, IProgress<string> progress)
            => Task.Run(() =>
            {
                Session.Initialize(progress);

                progress?.Report("Loading");

                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("img", TensorUtils.ImageToCHWTensor(image, RequiredWidth, RequiredHeight))
                };

                progress?.Report("Inferencing");

                // Run inference
                using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = Session.Instance.Run(inputs);

                progress?.Report("Done");

                return TensorUtils.CHWTensorToImage(results.First().AsTensor<float>());
            });
    }
}
