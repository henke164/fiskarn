using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Fiskarn.Services
{
    public class ScreenShotService
    {
        public Rectangle CreateRectangleFromCenterPoint(Point center, Size size)
            => new Rectangle(
                center.X - (size.Width / 2),
                center.Y - (size.Height / 2),
                size.Width,
                size.Height);

        public Bitmap CaptureScreenShot(Rectangle screenshotRectangle)
        {
            if (screenshotRectangle.X < 0)
            {
                screenshotRectangle.X = 0;
            }

            if (screenshotRectangle.Y < 0)
            {
                screenshotRectangle.Y = 0;
            }

            var bounds = Screen.GetBounds(Point.Empty);

            using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                return bitmap.Clone(screenshotRectangle, PixelFormat.Format24bppRgb);
            }
        }
    }
}
