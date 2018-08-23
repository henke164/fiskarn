using AForge.Imaging;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Fiskarn.Services
{
    public class ImageLocator
    {
        private readonly float _sensitivity = 0.9f;

        public ImageLocator()
        {
            var sensitivity = ConfigurationSettings.AppSettings.Get("sensitivity").Replace('.', ',');
            _sensitivity = float.Parse(sensitivity);
            Console.WriteLine($"Sensitivity: {_sensitivity}.");
        }

        public Point FindInImage(Bitmap template, Bitmap sourceImage)
        {
            var tm = new ExhaustiveTemplateMatching(_sensitivity);

            var matchings = tm.ProcessImage(sourceImage, template);

            var data = sourceImage.LockBits(
                 new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                 ImageLockMode.ReadWrite, sourceImage.PixelFormat);

            var matching = matchings.FirstOrDefault();

            sourceImage.UnlockBits(data);

            if (matching == null)
            {
                return Point.Empty;
            }

            return new Point(matching.Rectangle.X, matching.Rectangle.Y);
        }
    }
}
