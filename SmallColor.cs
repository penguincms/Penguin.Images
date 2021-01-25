using System;
using System.Runtime.CompilerServices;

namespace Penguin.Images
{
    public struct SmallColor
    {
        public byte A { get; set; }

        public byte B { get; set; }

        public byte G { get; set; }

        public byte R { get; set; }

        public SmallColor(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
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

        public override string ToString()
        {
            return $"#{this.R:X2}{this.G:X2}{this.B:X2}{this.A:X2}";
        }
    }
}