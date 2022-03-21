using System.Drawing;

namespace Penguin.Images
{
    public class OffsetImage
    {
        public BitmapReader Image { get; set; }

        public Point Offset { get; set; }

        public OffsetImage(Image source)
        {
            this.Image = new BitmapReader(source);
        }

        public OffsetImage(BitmapReader source)
        {
            this.Image = source;
        }

        public SmallColor GetPixel(int x, int y) => this.Image.GetPixel(x - (this.Offset.X), y - (this.Offset.Y));

        public SmallColor GetPixel(Point p) => this.GetPixel(p.X, p.Y);
    }
}