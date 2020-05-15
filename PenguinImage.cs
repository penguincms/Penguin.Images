using ImageMagick;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace Penguin.Images
{
    /// <summary>
    /// Wraps an image and allows for some basic operations
    /// </summary>
    public class PenguinImage
    {
        /// <summary>
        /// Retrieves the altered underlying bytes
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
        public byte[] Bytes { get; private set; }

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
        /// Constructs a new instance from a byte array
        /// </summary>
        /// <param name="SourceData">The file data</param>
        public PenguinImage(byte[] SourceData)
        {
            this.Bytes = SourceData;
        }

        /// <summary>
        /// Constructs a new instance from a GDI image
        /// </summary>
        /// <param name="image">The image source</param>
        public PenguinImage(System.Drawing.Image image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                this.Bytes = ms.ToArray();
            }
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
                    this.CropResize(Width, Height);
                    break;

                case ResizeMode.Stretch:
                    this.StretchResize(Width);
                    break;

                case ResizeMode.Fit:
                    this.FitResize(Width);
                    break;
            }
        }

        /// <summary>
        /// Applies a border radius.
        /// </summary>
        /// <param name="Radius">The radius to apply. Any amount &lt; 1 is a %, any amount over is a px</param>
        public void Round(float Radius = 0.5f)
        {
            using (MagickImage image = new MagickImage(this.Bytes))
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

        private void CropResize(int width, int height)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // FullPath is the new file's path.
                using (MagickImage image = new MagickImage(this.Bytes))
                {
                    if (image.Height != height || image.Width != width)
                    {
                        decimal result_ratio = height / (decimal)width;
                        decimal current_ratio = image.Height / (decimal)image.Width;

                        bool preserve_width = false;
                        if (current_ratio > result_ratio)
                        {
                            preserve_width = true;
                        }

                        int new_width;

                        int new_height;
                        if (preserve_width)
                        {
                            new_width = width;
                            new_height = (int)Math.Round(current_ratio * new_width);
                        }
                        else
                        {
                            new_height = height;
                            new_width = (int)Math.Round(new_height / current_ratio);
                        }

                        string geomStr = $"{width}x{height}";
                        string newGeomStr = $"{new_width}x{new_height}";

                        MagickGeometry intermediate_geo = new MagickGeometry(newGeomStr);
                        MagickGeometry final_geo = new MagickGeometry(geomStr);

                        image.Resize(intermediate_geo);
                        image.Crop(final_geo);
                    }

                    image.Write(ms);
                }

                this.Bytes = ms.ToArray();
            }
        }

        private void CropRound(int size)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (MagickImage image = new MagickImage(this.Bytes))
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
                this.Bytes = ms.ToArray();
            }
        }

        private void DistortRound()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (MagickImage image = new MagickImage(this.Bytes))
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
                this.Bytes = ms.ToArray();
            }
        }

        private void FitResize(int Width)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (MagickImage image = new MagickImage(this.Bytes))
                {
                    MagickGeometry size = new MagickGeometry(Width, Width);
                    image.Resize(size);
                    // Save the result
                    image.Write(ms);
                }

                this.Bytes = ms.ToArray();
            }
        }

        private void StretchResize(int Width)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (MagickImage image = new MagickImage(this.Bytes))
                {
                    MagickGeometry size = new MagickGeometry(Width, Width)
                    {
                        IgnoreAspectRatio = true
                    };

                    image.Resize(size);
                    // Save the result
                    image.Write(ms);
                }

                this.Bytes = ms.ToArray();
            }
        }
    }
}