using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Fiskarn.Services
{
    public class ScreenShotService
    {
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
                var screenbounds = Screen.PrimaryScreen.Bounds;
                using (var bitmap = new Bitmap(screenbounds.Width, screenbounds.Height))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(
                            0,
                            0,
                            0,
                            0,
                            screenbounds.Size,
                            CopyPixelOperation.SourceCopy);
                    }
                    var image = (Bitmap)bitmap.Clone();
                    return image;
                }
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
