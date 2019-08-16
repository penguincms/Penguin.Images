using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Penguin.Images
{
    /// <summary>
    /// Wraps an image and allows for some basic operations
    /// </summary>
    public class PenguinImage
    {
        private byte[] backing;

        /// <summary>
        /// Constructs a new instance from a byte array
        /// </summary>
        /// <param name="SourceData">The file data</param>
        public PenguinImage(byte[] SourceData)
        {
            backing = SourceData;
        }

        /// <summary>
        /// Constructs a new instance from a GDI image
        /// </summary>
        /// <param name="image">The image source</param>
        public PenguinImage(System.Drawing.Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                backing = ms.ToArray();
            }
        }

        /// <summary>
        /// Retrieves the altered underlying bytes
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                return backing;
            }
        }

        private void CropRound(int size)
        {
            MemoryStream ms = new MemoryStream();

            using (MagickImage image = new MagickImage(backing))
            {

                image.Format = MagickFormat.Png;

                using (MagickImage mask = new MagickImage(MagickColors.White, image.Width, image.Height))
                {


                    new ImageMagick.Drawables()
                        .FillColor(MagickColors.Black)
                        .StrokeColor(MagickColors.Black)
                        .Polygon(new PointD(0, 0), new PointD(0, size), new PointD(size, 0))
                        .Polygon(new PointD(mask.Width, 0), new PointD(mask.Width, size), new PointD(mask.Width - size, 0))
                        .Polygon(new PointD(0, mask.Height), new PointD(0, mask.Height - size), new PointD(size, mask.Height))
                        .Polygon(new PointD(mask.Width, mask.Height), new PointD(mask.Width, mask.Height - size), new PointD(mask.Width - size, mask.Height))
                        .FillColor(MagickColors.White)
                        .StrokeColor(MagickColors.White)
                        .Circle(size, size, size, 0)
                        .Circle(mask.Width - size, size, mask.Width - size, 0)
                        .Circle(size, mask.Height - size, 0, mask.Height - size)
                        .Circle(mask.Width - size, mask.Height - size, mask.Width - size, mask.Height)
                        .Draw(mask);

                    // This copies the pixels that were already transparent on the mask.
                    using (IMagickImage imageAlpha = image.Clone())
                    {
                        imageAlpha.Alpha(AlphaOption.Extract);
                        imageAlpha.Opaque(MagickColors.White, MagickColors.None);
                        mask.Composite(imageAlpha, CompositeOperator.Over);
                    }

                    mask.HasAlpha = false;
                    image.HasAlpha = false;
                    image.Composite(mask, CompositeOperator.CopyAlpha);
                    image.Write(ms);
                }

            }
            backing = ms.ToArray();

        }

        private void DistortRound()
        {
            MemoryStream ms = new MemoryStream();

            using (MagickImage image = new MagickImage(backing))
            {
                image.Format = MagickFormat.Png;

                image.Alpha(AlphaOption.Set);
                IMagickImage copy = image.Clone();

                copy.Distort(DistortMethod.DePolar, 0);
                copy.VirtualPixelMethod = VirtualPixelMethod.HorizontalTile;
                copy.BackgroundColor = MagickColors.None;
                copy.Distort(DistortMethod.Polar, 0);

                image.Compose = CompositeOperator.DstIn;
                image.Composite(copy, CompositeOperator.CopyAlpha);

                image.Write(ms);
            }
            backing = ms.ToArray();
        }
        /// <summary>
        /// Applies a border radius. 
        /// </summary>
        /// <param name="Radius">The radius to apply. Any amount < 1 is a %, any amount over is a px</param>
        public void Round(float Radius = 0.5f)
        {
            using (MagickImage image = new MagickImage(backing))
            {
                int Effective50 = (int)(Math.Min(image.Width, image.Height) * .01 * 50);
                int r;

                if (Radius < 1)
                {
                    r = (int)(Math.Min(image.Width, image.Height) * .01 * Radius);
                }
                else
                {
                    r = (int)Radius;
                }


                if (r == Effective50)
                {
                    this.DistortRound();
                }
                else
                {
                    this.CropRound(r);
                }

            }
        }

        /// <summary>
        /// The mode to use on the image to ensure it fits within the given dimensions
        /// </summary>
        public enum ResizeMode
        {
            Crop,
            Stretch,
            Fit
        }

        /// <summary>
        /// Resizes the image using the given mode
        /// </summary>
        /// <param name="Width">The new width</param>
        /// <param name="Height">The new height</param>
        /// <param name="Mode">The mode to use</param>
        public void Resize(int Width, int Height, ResizeMode Mode)
        {

            switch (Mode)
            {
                case ResizeMode.Crop:
                    CropResize(Width, Height);
                    break;
                case ResizeMode.Stretch:
                    StretchResize(Width, Height);
                    break;
                case ResizeMode.Fit:
                    FitResize(Width, Height);
                    break;

            }
        }

        private void FitResize(int Width, int Height)
        {

            MemoryStream ms = new MemoryStream();

            using (MagickImage image = new MagickImage(backing))
            {
                MagickGeometry size = new MagickGeometry(Width, Width);
                image.Resize(size);
                // Save the result
                image.Write(ms);
            }

            backing = ms.ToArray();
        }

        private void StretchResize(int Width, int Height)
        {

            MemoryStream ms = new MemoryStream();

            using (MagickImage image = new MagickImage(backing))
            {
                MagickGeometry size = new MagickGeometry(Width, Width);

                size.IgnoreAspectRatio = true;

                image.Resize(size);
                // Save the result
                image.Write(ms);
            }

            backing = ms.ToArray();
        }

        private void CropResize(int width, int height)
        {
            MemoryStream ms = new MemoryStream();
            // FullPath is the new file's path.
            using (MagickImage image = new MagickImage(backing))
            {
                if (image.Height != height || image.Width != width)
                {
                    decimal result_ratio = (decimal)height / (decimal)width;
                    decimal current_ratio = (decimal)image.Height / (decimal)image.Width;

                    Boolean preserve_width = false;
                    if (current_ratio > result_ratio)
                    {
                        preserve_width = true;
                    }
                    int new_width = 0;
                    int new_height = 0;
                    if (preserve_width)
                    {
                        new_width = width;
                        new_height = (int)Math.Round((decimal)(current_ratio * new_width));
                    }
                    else
                    {
                        new_height = height;
                        new_width = (int)Math.Round((decimal)(new_height / current_ratio));
                    }


                    String geomStr = width.ToString() + "x" + height.ToString();
                    String newGeomStr = new_width.ToString() + "x" + new_height.ToString();

                    MagickGeometry intermediate_geo = new MagickGeometry(newGeomStr);
                    MagickGeometry final_geo = new MagickGeometry(geomStr);


                    image.Resize(intermediate_geo);
                    image.Crop(final_geo);

                }

                image.Write(ms);
            }

            backing = ms.ToArray();

        }

    }
}
