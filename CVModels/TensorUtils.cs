using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public static Tensor<float> ImageToCHWTensor(byte[] imageBytes)
        {
            using var image = SKBitmap.Decode(imageBytes);
            Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, image.Height, image.Width });

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

        public static Tensor<float> ImageToCHWTensorPadToMultiples(byte[] imageBytes, int factor, out int origHeight, out int origWidth)
        {
            using var image = SKBitmap.Decode(imageBytes);
            origHeight = image.Height;
            origWidth = image.Width;
            int resHeight = (image.Height + factor - 1) / factor * factor;
            int resWidth = (image.Width + factor - 1) / factor * factor;
            Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, resHeight, resWidth });
            for (int y = 0; y < resHeight; y++)
            {
                for (int x = 0; x < resWidth; x++)
                {
                    int patternX = x % (image.Width * 2);
                    int patternY = y % (image.Height * 2);
                    int flipX = (patternX < image.Width) ? patternX : (image.Width * 2 - patternX - 1);
                    int flipY = (patternY < image.Height) ? patternY : (image.Height * 2 - patternY - 1);
                    var pixel = image.GetPixel(flipX, flipY);
                    input[0, 0, y, x] = pixel.Red / 255f;
                    input[0, 1, y, x] = pixel.Green / 255f;
                    input[0, 2, y, x] = pixel.Blue / 255f;
                }
            }
            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte FloatToByte(float x) => x switch
        {
            < 0 => 0,
            > 1 => 0xFF,
            _ => (byte)(x * 255)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint MakeRgba8888Pixel(float red, float green, float blue) =>
            (uint)((0xFF << 24) | (FloatToByte(blue) << 16) | (FloatToByte(green) << 8) | FloatToByte(red));

        public static byte[] CHWTensorToImage(Tensor<float> tensor, int height, int width)
        {
            using var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);

            unsafe
            {
                uint* pixels = (uint*)bitmap.GetPixels().ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        *pixels++ = MakeRgba8888Pixel(tensor[0, 0, y, x], tensor[0, 1, y, x], tensor[0, 2, y, x]);
                    }
                }
            }

            return SkiaSharpUtils.BitmapToBytes(bitmap);
        }

        public static byte[] CHWTensorToImage(Tensor<float> tensor)
        {
            return CHWTensorToImage(tensor, tensor.Dimensions[2], tensor.Dimensions[3]);
        }
    }
}
