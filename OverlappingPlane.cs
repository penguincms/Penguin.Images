using System;
using System.Collections.Generic;
using System.Drawing;

namespace Penguin.Images
{
    public class OverlappingPlane
    {
        private const int BYTE_DEPTH = 255 * 4;
        private double? diff;
        public OffsetImage AnchorImage { get; private set; }

        public Point Max { get; private set; }

        public Point Min { get; private set; }

        public OffsetImage OffsetImage { get; private set; }

        public IEnumerable<OverlappingPoint> OverlappingPoints
        {
            get
            {
                for (int x = this.Min.X; x < this.Max.X; x++)
                {
                    for (int y = this.Min.Y; y < this.Max.Y; y++)
                    {
                        yield return new OverlappingPoint
                        {
                            Anchor = new Point(x, y),
                            Offset = new Point(x - (this.OffsetImage.Offset.X), y - (this.OffsetImage.Offset.Y))
                        };
                    }
                }
            }
        }

        public Size Size { get; private set; }

        public enum ClippingStyle
        {
            Ignore,
            Diff
        }

        public OverlappingPlane(BitmapReader anchorImage, BitmapReader offsetImage, Point offset)
        {
            if (anchorImage is null)
            {
                throw new ArgumentNullException(nameof(anchorImage));
            }

            if (offset.X >= anchorImage.Width)
            {
                throw new IndexOutOfRangeException($"Offset X: {offset.X} lies outside of anchor width: {anchorImage.Width}. No overlap possible");
            }

            if (offsetImage is null)
            {
                throw new ArgumentNullException(nameof(offsetImage));
            }

            if (offset.Y >= anchorImage.Height)
            {
                throw new IndexOutOfRangeException($"Offset Y: {offset.Y} lies outside of anchor height: {anchorImage.Height}. No overlap possible");
            }

            if (offset.X <= 0 - offsetImage.Width)
            {
                throw new IndexOutOfRangeException($"Offset X: {offset.X} lies outside of offset image width: {offsetImage.Width}. No overlap possible");
            }

            if (offset.Y <= 0 - offsetImage.Height)
            {
                throw new IndexOutOfRangeException($"Offset Y: {offset.Y} lies outside of offset image height: {offsetImage.Height}. No overlap possible");
            }

            this.AnchorImage = new OffsetImage(anchorImage);

            this.OffsetImage = new OffsetImage(offsetImage)
            {
                Offset = offset
            };

            this.Min = new Point()
            {
                X = Math.Max(0, offset.X),
                Y = Math.Max(0, offset.Y)
            };

            this.Max = new Point()
            {
                X = Math.Min(anchorImage.Width, offset.X + offsetImage.Width) - 1,
                Y = Math.Min(anchorImage.Height, offset.Y + offsetImage.Height) - 1
            };

            this.Size = new Size(this.Max.X - this.Min.X + 1, this.Max.Y - this.Min.Y + 1);
        }

        public OverlappingPlane(Image anchorImage, Image offsetImage, Point offset) : this(new BitmapReader(anchorImage), new BitmapReader(offsetImage), offset)
        {
        }

        public double Diff(Color? key = null, ClippingStyle style = ClippingStyle.Diff)
        {
            unchecked
            {
                if (this.diff.HasValue)
                {
                    return this.diff.Value;
                }

                Color Key = key ?? Color.Black;

                int compC = 0;
                long totalC = 0;

                SmallColor[] aPixels = this.AnchorImage.Image.Pixels;
                int aWidth = this.AnchorImage.Image.Width;

                SmallColor[] oPixels = this.OffsetImage.Image.Pixels;
                int oWidth = this.OffsetImage.Image.Width;

                unsafe
                {
                    int xl = this.Max.X - this.Min.X;

                    for (int y = this.Min.Y; y <= this.Max.Y; y++)
                    {
                        fixed (SmallColor* a = &aPixels[(y * aWidth) + this.Min.X])
                        {
                            fixed (SmallColor* o = &oPixels[((y - this.OffsetImage.Offset.Y) * oWidth) + (this.Min.X - this.OffsetImage.Offset.X)])
                            {
                                SmallColor* aVC = a;
                                SmallColor* oVC = o;

                                for (int x = 0; x <= xl; x++)
                                {
                                    compC++;
                                    totalC += (*aVC).Diff255(*oVC);

                                    aVC++;
                                    oVC++;
                                }
                            }
                        }
                    }
                }

                if (style == ClippingStyle.Diff)
                {
                    int diffAmount = (this.AnchorImage.Image.Height * this.AnchorImage.Image.Width) - compC;

                    totalC += diffAmount * BYTE_DEPTH;

                    compC = (this.AnchorImage.Image.Height * this.AnchorImage.Image.Width);
                }

                this.diff = (totalC / (double)BYTE_DEPTH) / compC;

                return this.diff.Value;
            }
        }

        public Bitmap Extract() => this.Extract(Color.Black);

        public Bitmap Extract(Color? key)
        {
            Bitmap toReturn = new Bitmap(this.Max.X, this.Max.Y);

            if (key.HasValue)
            {
                Graphics g = Graphics.FromImage(toReturn);
                g.Clear(key.Value);
            }

            foreach (OverlappingPoint op in this.OverlappingPoints)
            {
                SmallColor c = this.OffsetImage.Image.GetPixel(op.Offset);

                toReturn.SetPixel(op.Anchor.X, op.Anchor.Y, Color.FromArgb(c.A, c.R, c.G, c.B));
            }

            return toReturn;
        }
    }
}