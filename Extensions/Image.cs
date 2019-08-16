using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Penguin.Images.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class ImageExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region Methods

        /// <summary>
        /// Returns the MimeType associated with the image
        /// </summary>
        /// <typeparam name="T">Any Image type</typeparam>
        /// <param name="image">The image to get the mime type for</param>
        /// <returns>The image mime type</returns>
        public static string GetMimeType<T>(this T image) where T : Image
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == image.RawFormat.Guid)?.MimeType;
        }

        /// <summary>
        /// Returns the file extension for the image format (from the encoder)
        /// </summary>
        /// <typeparam name="T">Any image type</typeparam>
        /// <param name="image">Any image</param>
        /// <returns>the file extension (including .)</returns>
        public static string GetFilenameExtension<T>(this T image) where T : Image
        {

            try
            {
                return ImageCodecInfo.GetImageEncoders()
                        .FirstOrDefault(x => x.FormatID == image.RawFormat.Guid)
                        ?.FilenameExtension
                        ?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        ?.FirstOrDefault()
                        ?.Trim('*')
                        ?.ToLower() ?? ".IDFK";
            }
            catch (Exception)
            {
                return ".IDFK";
            }
        }

        /// <summary>
        /// Converts an image to the requested format in-memory and returns the bytes
        /// </summary>
        /// <typeparam name="T">Any image kind</typeparam>
        /// <param name="source">The source to convert</param>
        /// <param name="format">The target format</param>
        /// <returns>The converted bytes</returns>
        public static byte[] Convert<T>(this T source, System.Drawing.Imaging.ImageFormat format) where T : Image
        {
            MemoryStream ms = new MemoryStream();
            source.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        /// <summary>
        /// Just calls resize
        /// </summary>
        /// <param name="img">The image to resize</param>
        /// <param name="thumbSize">The size of the image to return</param>
        /// <returns>A thumbnail for the image</returns>
        public static Bitmap GenerateThumbnail<T>(this T img, Size thumbSize) where T : Image =>
            // return img.PadImage().GetThumbnailImage(thumbSize.Width, thumbSize.Height, () => false, IntPtr.Zero);
            img.Resize(thumbSize);

        /// <summary>
        /// Pads an image out so that it has a 1:1 aspect ratio
        /// </summary>
        /// <param name="originalImage">The image to pad</param>
        /// <returns>The padded image</returns>
        public static Bitmap PadImage<T>(this T originalImage) where T : Image
        {
            int largestDimension = Math.Max(originalImage.Height, originalImage.Width);
            Size squareSize = new Size(largestDimension, largestDimension);
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
            using (Graphics graphics = GetHQGraphics(squareImage))
            {
                graphics.FillRectangle(Brushes.Black, 0, 0, squareSize.Width, squareSize.Height);

                graphics.DrawImage(originalImage, (squareSize.Width / 2) - (originalImage.Width / 2), (squareSize.Height / 2) - (originalImage.Height / 2), originalImage.Width, originalImage.Height);
            }

            return squareImage;
        }

        private static Graphics GetHQGraphics(Image i)
        {
            Graphics g = Graphics.FromImage(i);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            return g;
        }

        /// <summary>
        /// Resizes an image to the given dimensions
        /// </summary>
        /// <param name="original">The original image</param>
        /// <param name="size">A size for the new image</param>
        /// <returns>A resized image</returns>
        public static Bitmap Resize<T>(this T original, Size size) where T : Image
        {
            Bitmap resized = new Bitmap(original, size.Width, original.Height * size.Width / original.Width );         

            if (resized.Height > size.Height)
            {
                Rectangle cropArea = new Rectangle(0, (resized.Height - size.Height) / 2, size.Width, size.Height);
                resized = resized.Clone(cropArea, resized.PixelFormat);
            }

            return resized;
        }

        /// <summary>
        /// Scales an image to fit within the given dimensions while preserving aspect ratio
        /// </summary>
        /// <param name="img">The source image to scale</param>
        /// <param name="maxWidth">The maximum width the new image should have</param>
        /// <param name="maxHeight">The maximum height the new image should have</param>
        /// <returns>A new image that fits within the given dimensions</returns>
        public static Bitmap ScaleImage<T>(this T img, int maxWidth, int maxHeight = int.MaxValue) where T : Image
        {
            double ratioX = (double)maxWidth / img.Width;
            double ratioY = (double)maxHeight / img.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(img.Width * ratio);
            int newHeight = (int)(img.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            newImage.SetResolution(100, 100);

            using (Graphics graphics = GetHQGraphics(newImage))
            {
                graphics.DrawImage(img, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        /// <summary>
        /// Converts an image to a byte array for persistence or transfer
        /// </summary>
        /// <param name="img">the source image to convert</param>
        /// <returns>A byte array representing the data contained within the original image</returns>
        public static byte[] ToByteArray<T>(this T img) where T : Image
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                img.Save(memoryStream, ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Rounds an image corners
        /// </summary>
        /// <param name="StartImage">The image to round</param>
        /// <param name="CornerRadius">The radius of the corners</param>
        /// <param name="BackgroundColor">The color to apply behind the rounded corners</param>
        /// <returns>The image with rounded corners</returns>
        public static Bitmap RoundCorners<T>(this T StartImage, int CornerRadius, Color BackgroundColor) where T : Image
        {

            CornerRadius *= 2;
            Bitmap RoundedImage = new Bitmap(StartImage.Width, StartImage.Height);
            RoundedImage.SetResolution(100, 100);
            using (Graphics g = GetHQGraphics(RoundedImage))
            {
                g.Clear(BackgroundColor);

                using (Brush brush = new TextureBrush(StartImage))
                {
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        gp.AddArc(-1, -1, CornerRadius, CornerRadius, 180, 90);
                        gp.AddArc(0 + RoundedImage.Width - CornerRadius, -1, CornerRadius, CornerRadius, 270, 90);
                        gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                        gp.AddArc(-1, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);

                        g.FillPath(brush, gp);
                    }
                }

                return RoundedImage;
            }
        }

        #endregion Methods
    }
}