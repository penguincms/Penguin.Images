using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;

namespace Penguin.Images
{
    public class BitmapReader
    {
        public SmallColor this[int x, int y] => this.Pixels[(y * this.Width) + x];

        private readonly Bitmap backing;

        private readonly object backingLock = new object();

        public int Height { get; private set; }

        public int Width { get; private set; }

        internal SmallColor[] Pixels { get; private set; }

        public BitmapReader(string FileName) : this(new Bitmap(FileName))
        {
        }

        public BitmapReader(Bitmap source)
        {
            this.backing = source ?? throw new ArgumentNullException(nameof(source));

            SmallColor[] npixels = new SmallColor[source.Width * source.Height];

            lock (this.backingLock)
            {
                BitmapData srcData = this.backing.LockBits(new Rectangle(0, 0, this.backing.Width, this.backing.Height), ImageLockMode.ReadOnly, this.backing.PixelFormat);

                unsafe
                {
                    byte* srcPointer = (byte*)srcData.Scan0;

                    byte r;
                    byte g;
                    byte b;
                    byte a;
                    this.Width = source.Width;
                    this.Height = source.Height;

                    int i = 0;

                    for (int x = 0; x < this.Width; x++)
                    {
                        for (int y = 0; y < this.Height; y++)
                        {
                            b = srcPointer[0]; // Blue
                            g = srcPointer[1]; // Green
                            r = srcPointer[2]; // Red
                            a = srcPointer[3]; // Alpha

                            npixels[i++] = new SmallColor(r, g, b, a);

                            srcPointer += 4;
                        }
                    }
                }

                this.backing.UnlockBits(srcData);
            }

            this.Pixels = npixels;
        }

        public BitmapReader(Image source) : this(new Bitmap(source))
        {
        }

        public Bitmap Clone(Rectangle r)
        {
            lock (this.backingLock)
            {
                return this.backing.Clone(r, this.backing.PixelFormat);
            }
        }

        public SmallColor GetPixel(int x, int y)
        {
            return this.Pixels[(y * this.Width) + x];
        }

        public SmallColor GetPixel(Point p)
        {
            return this.Pixels[(p.Y * this.Width) + p.X];
        }

        public ImmutableArray<SmallColor> GetPixels()
        {
            return this.Pixels.ToImmutableArray();
        }
    }
}