using SkiaSharp;

namespace CVModels;

public static class SkiaSharpUtils
{
    internal static byte[] BitmapToBytes(SKBitmap bitmap)
    {
        using var stream = new MemoryStream();
        using var wstream = new SKManagedWStream(stream);

        bitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
        var bytes = stream.ToArray();

        return bytes;
    }

    /// <summary>
    /// 处理输入图像的旋转元数据，给出无旋转元数据的 PNG 格式编码图像
    /// </summary>
    /// <param name="image">按照 SkiaSharp 支持的方式编码的图像字节流</param>
    /// <returns>无旋转元数据的 JPEG 格式编码图像</returns>
    public static byte[] HandleOrientation(byte[] image)
    {
        using var memoryStream = new MemoryStream(image);
        using var imageData = SKData.Create(memoryStream);
        using var codec = SKCodec.Create(imageData);
        var orientation = codec.EncodedOrigin;

        using var bitmap = SKBitmap.Decode(image);
        using var adjustedBitmap = AdjustBitmapByOrientation(bitmap, orientation);

        return BitmapToBytes(adjustedBitmap);
    }

    static SKBitmap AdjustBitmapByOrientation(SKBitmap bitmap, SKEncodedOrigin orientation)
    {
        switch (orientation)
        {
            case SKEncodedOrigin.BottomRight:

                using (var canvas = new SKCanvas(bitmap))
                {
                    canvas.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                    canvas.DrawBitmap(bitmap.Copy(), 0, 0);
                }

                return bitmap;

            case SKEncodedOrigin.RightTop:

                using (var rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width))
                {
                    using (var canvas = new SKCanvas(rotatedBitmap))
                    {
                        canvas.Translate(rotatedBitmap.Width, 0);
                        canvas.RotateDegrees(90);
                        canvas.DrawBitmap(bitmap, 0, 0);
                    }

                    rotatedBitmap.CopyTo(bitmap);
                    return bitmap;
                }

            case SKEncodedOrigin.LeftBottom:

                using (var rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width))
                {
                    using (var canvas = new SKCanvas(rotatedBitmap))
                    {
                        canvas.Translate(0, rotatedBitmap.Height);
                        canvas.RotateDegrees(270);
                        canvas.DrawBitmap(bitmap, 0, 0);
                    }

                    rotatedBitmap.CopyTo(bitmap);
                    return bitmap;
                }

            default:
                return bitmap;
        }
    }

    public static byte[] StretchToDesiredSize(byte[] image, int width, int height)
    {
        using var bitmap = SKBitmap.Decode(image);
        using var resized = bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
        return BitmapToBytes(resized);
    }

    public static byte[] ZoomToFitSize(byte[] image, int longestEdge)
    {
        using var bitmap = SKBitmap.Decode(image);
        int width = bitmap.Width;
        int height = bitmap.Height;
        if (width < longestEdge && height < longestEdge) return image;
        float ratio = (float)longestEdge / Math.Max(width, height);
        using var resized = bitmap.Resize(new SKImageInfo((int)(width * ratio), (int)(height * ratio)), SKFilterQuality.Medium);
        return BitmapToBytes(resized);
    }
}