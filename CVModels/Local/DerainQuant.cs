using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Local
{
    public class DerainQuant : IImageProcessingModel
    {
        ILocalSession Session { get; } = new EmbbededModelSession("derain_se_quant.onnx");
        
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
                var stopwatch = Stopwatch.StartNew();
                using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = Session.Instance.Run(inputs);
                stopwatch.Stop();
                Debug.WriteLine($">>> {RequiredHeight}x{RequiredWidth} {stopwatch.ElapsedMilliseconds}ms <<<");

                progress?.Report("Done");

                return TensorUtils.CHWTensorToImage(results.First().AsTensor<float>());
            });
    }
}
