using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace Fiskarn.Services
{
    public class ScreenShotService
    {
        private Rectangle _bounds;

        public ScreenShotService()
        {
            _bounds = Screen.GetBounds(Point.Empty);
        }
        public Rectangle CreateRectangleFromCenterPoint(Point center, Size size)
            => new Rectangle(
                center.X - (size.Width / 2),
                center.Y - (size.Height / 8),
                size.Width,
                size.Height);

        public Bitmap CaptureScreenShot()
        {
            try
            {
                Bitmap clone;
                var bounds = Screen.GetBounds(Point.Empty);

                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    clone = (Bitmap)bitmap.Clone();
                }
                return clone;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error");
                Console.WriteLine(ex);
                Thread.Sleep(1000);
                return CaptureScreenShot();
            }
        }

        public Bitmap GetScreenshotFromImage(Bitmap image, Rectangle rect)
        {
            return (Bitmap)image.Clone(rect, image.PixelFormat);
        }
    }
}
