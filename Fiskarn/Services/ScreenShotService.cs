using System.Drawing;

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

            using (var bitmap = new Bitmap(screenshotRectangle.Width, screenshotRectangle.Height))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(
                        screenshotRectangle.X, 
                        screenshotRectangle.Y,
                        0,
                        0,
                        screenshotRectangle.Size,
                        CopyPixelOperation.SourceCopy);
                }
                var image = (Bitmap)bitmap.Clone();
                return image;
            }
        }
    }
}
