using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;

namespace CVModels
{
    static internal class TensorUtils
    {
        public static Tensor<float> ImageToCHWTensor(byte[] imageBytes, int width, int height)
        {
            using var image = SKBitmap.Decode(imageBytes);
            if (image.Width != width || image.Height != height)
            {
                throw new ArgumentException($"Image size expected to be ({width}, {height}), got ({image.Width}, {image.Height}) instead.");
            }
            Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, height, width });

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    input[0, 0, y, x] = pixel.Red / 255f;
                    input[0, 1, y, x] = pixel.Green / 255f;
                    input[0, 2, y, x] = pixel.Blue / 255f;
                }
            }

            return input;
        }

        public static byte[] CHWTensorToImage(Tensor<float> tensor)
        {
            int height = tensor.Dimensions[2];
            int width = tensor.Dimensions[3];

            using var bitmap = new SKBitmap(width, height);
            byte clamp(float x) => (byte)(Math.Min(Math.Max(x, 0), 1) * 255);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var col = new SKColor(
                        clamp(tensor[0, 0, y, x]), clamp(tensor[0, 1, y, x]), clamp(tensor[0, 2, y, x]));
                    bitmap.SetPixel(x, y, col);
                }
            }

            return SkiaSharpUtils.BitmapToBytes(bitmap);
        }
    }
}
