using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Fiskarn.Services
{
    public class ScreenShotService
    {
        public Bitmap CaptureScreenShot(Point center, Size screenshotSize)
        {
            var screenshotRectangle = new Rectangle(
                center.X - (screenshotSize.Width / 2),
                center.Y - (screenshotSize.Height / 2),
                screenshotSize.Width,
                screenshotSize.Height);

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
