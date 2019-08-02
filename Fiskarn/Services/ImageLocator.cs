using System;
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
                    if (pixel.B < 100 && pixel.G < 100 && pixel.R > 150 && pixel.R < 250)
                    {
                        if (redpoint == null || pixel.R > redpoint.Color.R)
                        {
                            redpoint = new RedPixel
                            {
                                Color = pixel,
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

            Console.WriteLine(redpoint.Color);
            return new Point(redpoint.X, redpoint.Y);
        }

        internal class RedPixel
        {
            public Color Color { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
