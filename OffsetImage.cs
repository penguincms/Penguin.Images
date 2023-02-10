using System.Drawing;

namespace Penguin.Images
{
    public class OffsetImage
    {
        public BitmapReader Image { get; set; }

        public Point Offset { get; set; }

        public OffsetImage(Image source)
        {
            Image = new BitmapReader(source);
        }

        public OffsetImage(BitmapReader source)
        {
            Image = source;
        }

        public SmallColor GetPixel(int x, int y)
        {
            return Image.GetPixel(x - Offset.X, y - Offset.Y);
        }

        public SmallColor GetPixel(Point p)
        {
            return GetPixel(p.X, p.Y);
        }
    }
}