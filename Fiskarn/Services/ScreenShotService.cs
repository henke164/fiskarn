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
            Bitmap clone;

            try
            {
                using (var bitmap = new Bitmap(_bounds.Width, _bounds.Height))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, _bounds.Size);
                    }

                    clone = (Bitmap)bitmap.Clone();
                }
                return clone;
            }
            catch
            {
                return new Bitmap(_bounds.Width, _bounds.Height);
            }
        }

        public Bitmap GetScreenshotFromImage(Bitmap image, Rectangle rect)
        {
            return (Bitmap)image.Clone(rect, image.PixelFormat);
        }
    }
}
