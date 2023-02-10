using System;
using System.Drawing;

namespace Penguin.Images.Extensions
{
    public static class BitmapExtensions
    {
        public static Color GetPixel(this Bitmap b, Point p)
        {
            return b is null ? throw new ArgumentNullException(nameof(b)) : b.GetPixel(p.X, p.Y);
        }

        public static void SetPixel(this Bitmap b, Point p, Color c)
        {
            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            b.SetPixel(p.X, p.Y, c);
        }
    }
}