using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Penguin.Images.Extensions
{
    public static class BitmapExtensions
    {
        public static Color GetPixel(this Bitmap b, Point p) => b.GetPixel(p.X, p.Y);

        public static void SetPixel(this Bitmap b, Point p, Color c) => b.SetPixel(p.X, p.Y, c);
    }
}