using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Penguin.Images
{
    public struct SmallColor
    {
        public byte A;

        public byte B;

        public byte G;

        public byte R;

        public SmallColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Diff255(SmallColor other)
        {
            unchecked
            {
                return Math.Abs(this.R - other.R) +
                       Math.Abs(this.G - other.G) +
                       Math.Abs(this.B - other.B) +
                       Math.Abs(this.A - other.A);
            }
        }

        public override string ToString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";
    }
}