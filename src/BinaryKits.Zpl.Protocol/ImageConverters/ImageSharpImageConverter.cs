using ImageMagick;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace BinaryKits.Zpl.Protocol.ImageConverters
{
    /// <summary>
    /// ImageSharp Image Converter
    /// </summary>
    public class ImageSharpImageConverter : IImageConverter
    {
        ///<inheritdoc/>
        public ImageResult ConvertImage(byte[] imageData)
        {
            var zplBuilder = new StringBuilder();

            using (MagickImage image = new MagickImage(imageData))
            {
                var bytesPerRow = image.Width % 8 > 0
                    ? image.Width / 8 + 1
                    : image.Width / 8;

                var binaryByteCount = image.Height * bytesPerRow;

                var colorBits = 0;
                var j = 0;
                var pixels = image.GetPixels();
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var pixel = pixels[x,y];
                        var pixelColor = pixel.ToColor();
                        var isBlackPixel = ((pixelColor.R + pixelColor.G + pixelColor.B) / 3) < 128;
                        if (isBlackPixel)
                        {
                            colorBits |= 1 << (7 - j);
                        }

                        j++;

                        if (j == 8 || x == (image.Width - 1))
                        {
                            zplBuilder.Append(colorBits.ToString("X2"));
                            colorBits = 0;
                            j = 0;
                        }
                    }
                    zplBuilder.Append('\n');
                }

                return new ImageResult
                {
                    ZplData = zplBuilder.ToString(),
                    BinaryByteCount = binaryByteCount,
                    BytesPerRow = bytesPerRow
                };
            }
        }

        private byte Reverse(byte b)
        {
            var reverse = 0;
            for (var i = 0; i < 8; i++)
            {
                if ((b & (1 << i)) != 0)
                {
                    reverse |= 1 << (7 - i);
                }
            }
            return (byte)reverse;
        }

        ///<inheritdoc/>
        public byte[] ConvertImage(byte[] imageData, int bytesPerRow)
        {
            imageData = imageData.Select(b => Reverse(b)).ToArray();

            var imageHeight = imageData.Length / bytesPerRow;
            var imageWidth = bytesPerRow * 8;
            using (var image = new MagickImage(MagickColors.White,imageWidth, imageHeight))
            {
                var pixels = image.GetPixels();
                for (var y = 0; y < image.Height; y++)
                {
                    var bits = new BitArray(imageData.Skip(bytesPerRow * y).Take(bytesPerRow).ToArray());

                    for (var x = 0 ; x < image.Width; x++)
                    {
                        if (bits[x])
                        {
                            var col = pixels[x, y].ToColor();
                            col.A = 255;
                            pixels.SetPixel(x,y,col.ToByteArray());
                        }
                    }
                }

                return image.ToByteArray();
            }
        }
    }
}
