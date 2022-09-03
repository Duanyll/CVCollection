﻿using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Local
{
    internal class ZeroDce : IImageProcessingModel
    {
        ILocalSession Session { get; } = new EmbbededModelSession("zero_dce.onnx");

        const int SizeLimit = 800;

        public Task<byte[]> PreprocessImageAsync(byte[] image)
            => Task.Run(() => SkiaSharpUtils.ZoomToFitSize(image, SizeLimit));
        

        public Task<byte[]> ProcessImageAsync(byte[] image, IProgress<string> progress) => Task.Run(() =>
        {
            Session.Initialize(progress);

            progress?.Report("Loading");

            var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("img", TensorUtils.ImageToCHWTensor(image))
                };

            progress?.Report("Inferencing");

            // Run inference
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = Session.Instance.Run(inputs);

            progress?.Report("Done");

            return TensorUtils.CHWTensorToImage(results.First().AsTensor<float>());
        });
    }
}
