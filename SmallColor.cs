using System;
using System.Runtime.CompilerServices;

namespace Penguin.Images
{
    public struct SmallColor : IEquatable<SmallColor>
    {
        public byte A { get; set; }

        public byte B { get; set; }

        public byte G { get; set; }

        public byte R { get; set; }

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
                return Math.Abs(R - other.R) +
                       Math.Abs(G - other.G) +
                       Math.Abs(B - other.B) +
                       Math.Abs(A - other.A);
            }
        }

        public override string ToString()
        {
            return $"#{R:X2}{G:X2}{B:X2}{A:X2}";
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(SmallColor left, SmallColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SmallColor left, SmallColor right)
        {
            return !(left == right);
        }

        public bool Equals(SmallColor other)
        {
            throw new NotImplementedException();
        }
    }
}