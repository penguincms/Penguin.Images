using System.Drawing;

namespace Penguin.Images
{
    public struct OverlappingPoint : System.IEquatable<OverlappingPoint>
    {
        public Point Anchor { get; set; }

        public Point Offset { get; set; }

        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }

        public static bool operator ==(OverlappingPoint left, OverlappingPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OverlappingPoint left, OverlappingPoint right)
        {
            return !(left == right);
        }

        public bool Equals(OverlappingPoint other)
        {
            throw new System.NotImplementedException();
        }
    }
}