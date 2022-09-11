using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Local
{
    internal class SadNet : IImageProcessingModel
    {
        ILocalSession Session { get; } = new DownloadedModelSession("sadnet.onnx");

        const int SizeLimit = 800;

        public Task<byte[]> PreprocessImageAsync(byte[] image)
            => Task.Run(() => SkiaSharpUtils.ZoomToFitSize(image, SizeLimit));


        public Task<byte[]> ProcessImageAsync(byte[] image, IProgress<string> progress) => Task.Run(() =>
        {
            Session.Initialize(progress);

            progress?.Report("Loading");

            var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("img", TensorUtils.ImageToCHWTensorPadToMultiples(image, 8, out var origH, out var origW))
                };

            progress?.Report("Inferencing");

            // Run inference
            var stopwatch = Stopwatch.StartNew();
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = Session.Instance.Run(inputs);
            stopwatch.Stop();
            Debug.WriteLine($">>> {origH}x{origW} {stopwatch.ElapsedMilliseconds}ms <<<");

            progress?.Report("Done");

            return TensorUtils.CHWTensorToImage(results.First().AsTensor<float>(), origH, origW);
        });
    }
}
