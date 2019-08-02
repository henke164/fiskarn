using System.Drawing;

namespace Fiskarn.Services
{
    public static class ColorHelper
    {
        public static bool IsRed(Color color)
        {
            var highestBG = color.B > color.G ? color.B : color.G;

            return color.B < 120 && color.G < 120 && color.R > highestBG;
        }
    }
}
