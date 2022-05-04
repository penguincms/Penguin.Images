using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Penguin.Images.Extensions
{
    public static class ColorExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Diff(this Color a, Color b)
        {
            return (Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B) + Math.Abs(a.A - b.A)) / (float)(255 * 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Diff255(this Color a, Color b)
        {
            return Math.Abs(a.R - b.R) +
                   Math.Abs(a.G - b.G) +
                   Math.Abs(a.B - b.B) +
                   Math.Abs(a.A - b.A);
        }
    }
}