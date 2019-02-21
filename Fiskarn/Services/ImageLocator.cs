using System.Drawing;

namespace Fiskarn.Services
{
    public class ImageLocator
    {
        public Point FindInImage(Bitmap sourceImage)
        {
            RedPixel redpoint = null;

            for (var x = 0; x < sourceImage.Width; x += 2)
            {
                for (var y = 0; y < sourceImage.Height; y += 2)
                {
                    var pixel = sourceImage.GetPixel(x, y);
                    if (pixel.R > pixel.B && pixel.R > pixel.G && pixel.R > 100 && pixel.R < 250)
                    {
                        if (redpoint == null || pixel.R > redpoint.RedAmount)
                        {
                            redpoint = new RedPixel
                            {
                                RedAmount = pixel.R,
                                X = x,
                                Y = y
                            };
                        }
                    }
                }
            }

            if (redpoint == null)
            {
                return Point.Empty;
            }

            return new Point(redpoint.X, redpoint.Y);
        }

        internal class RedPixel
        {
            public int RedAmount { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
