using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Penguin.Images
{
    public class BitmapReader
    {
        private Bitmap backing;

        private object backingLock = new object();

        public int Height { get; private set; }

        public int Width { get; private set; }

        internal SmallColor[] Pixels { get; private set; }

        public BitmapReader(string FileName) : this(new Bitmap(FileName))
        {
        }

        public BitmapReader(Bitmap source)
        {
            backing = source ?? throw new ArgumentNullException(nameof(source));

            SmallColor[] npixels = new SmallColor[source.Width * source.Height];

            lock (backingLock)
            {
                BitmapData srcData = backing.LockBits(new Rectangle(0, 0, backing.Width, backing.Height), ImageLockMode.ReadOnly, backing.PixelFormat);

                unsafe
                {
                    byte* srcPointer = (byte*)srcData.Scan0;

                    byte r;
                    byte g;
                    byte b;
                    byte a;
                    Width = source.Width;
                    Height = source.Height;

                    int i = 0;

                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            b = srcPointer[0]; // Blue
                            g = srcPointer[1]; // Green
                            r = srcPointer[2]; // Red
                            a = srcPointer[3]; // Alpha

                            npixels[i++] = new SmallColor(r, g, b, a);
                        }
                    }
                }

                backing.UnlockBits(srcData);
            }

            Pixels = npixels;
        }

        public BitmapReader(Image source) : this(new Bitmap(source))
        {
        }

        public SmallColor this[int x, int y]
        {
            get
            {
                return Pixels[(y * Width) + x];
            }
        }

        public Bitmap Clone(Rectangle r)
        {
            lock (backingLock)
            {
                return backing.Clone(r, backing.PixelFormat);
            }
        }

        public SmallColor GetPixel(int x, int y)
        {
            return Pixels[(y * Width) + x];
        }

        public SmallColor GetPixel(Point p)
        {
            return Pixels[(p.Y * Width) + p.X];
        }

        public ImmutableArray<SmallColor> GetPixels()
        {
            return Pixels.ToImmutableArray();
        }
    }
}