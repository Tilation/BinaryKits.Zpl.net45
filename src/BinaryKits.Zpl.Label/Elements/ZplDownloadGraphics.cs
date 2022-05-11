using BinaryKits.Zpl.Label.Helpers;
using BinaryKits.Zpl.Label.ImageConverters;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryKits.Zpl.Label.Elements
{
    /// <summary>
    /// Download Graphics<br/>
    /// The ~DG command downloads an ASCII Hex representation of a graphic image.
    /// If .GRF is not the specified file extension, .GRF is automatically appended.
    /// </summary>
    /// <remarks>
    /// Format:~DGd:o.x,t,w,data
    /// d = device to store image
    /// o = image name
    /// x = extension
    /// t = total number of bytes in graphic
    /// w = number of bytes per row
    /// data = ASCII hexadecimal string defining image
    /// </remarks>
    public class ZplDownloadGraphics : ZplDownload
    {
        public string ImageName { get; private set; }
        private string _extension { get; set; }
        public byte[] ImageData { get; private set; }

        private readonly bool _isCompressionActive;
        private readonly IImageConverter _imageConverter;

        /// <summary>
        /// Zpl Download Graphics
        /// </summary>
        /// <param name="storageDevice"></param>
        /// <param name="imageName"></param>
        /// <param name="imageData"></param>
        /// <param name="isCompressionActive"></param>
        /// <param name="imageConverter"></param>
        public ZplDownloadGraphics(
            char storageDevice,
            string imageName,
            byte[] imageData,
            bool isCompressionActive = true,
            IImageConverter imageConverter = default)
            : base(storageDevice)
        {
            if (imageName.Length > 8)
            {
                new ArgumentException("maximum length of 8 characters exceeded", nameof(imageName));
            }

            _extension = "GRF"; //Fixed

            ImageName = imageName;
            ImageData = imageData;

            if (imageConverter == default)
            {
                imageConverter = new ImageSharpImageConverter();
            }

            _isCompressionActive = isCompressionActive;
            _imageConverter = imageConverter;
        }

        ///<inheritdoc/>
        public override IEnumerable<string> Render(ZplRenderOptions context)
        {
            byte[] objectData;
            using (var image = new MagickImage(ImageData))
            {
                if (context.ScaleFactor != 1)
                {
                    var scaleWidth = (int)Math.Round(image.Width * context.ScaleFactor);
                    var scaleHeight = (int)Math.Round(image.Height * context.ScaleFactor);
                    image.Resize(scaleWidth, scaleHeight);
                }
                objectData = image.ToByteArray();
            }

            var imageResult = _imageConverter.ConvertImage(objectData);

            if (_isCompressionActive)
            {
                imageResult.ZplData = ZebraHexCompressionHelper.Compress(imageResult.ZplData, imageResult.BytesPerRow);
            }

            return new List<string>
            {
                $"~DG{StorageDevice}:{ImageName}.{_extension},{imageResult.BinaryByteCount},{imageResult.BytesPerRow},",
                imageResult.ZplData
            };
        }
    }
}
