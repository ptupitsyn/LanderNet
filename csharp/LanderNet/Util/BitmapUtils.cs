using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TpsGraphNet;

namespace LanderNet.UI.Util
{
    public static class BitmapUtils
    {
        public static BitmapSource GetResourceImage(string resourcePath)
        {
            var image = new BitmapImage();

            var moduleName = Assembly.GetExecutingAssembly().GetName().Name;
            var resourceLocation = string.Format("pack://application:,,,/{0};component/Resources/Sprites/{1}", moduleName, resourcePath);

            image.BeginInit();
            image.UriSource = new Uri(resourceLocation);

            // Warning: DO NOT USE DecodePixelHeight / DecodePixelWidth here to resize images! It will cause crashes on some systems (Windows 8.1)
            // Microsoft connect bug: https://connect.microsoft.com/VisualStudio/feedback/details/812641/bitmapsource-fails-with-outofmemoryexception-on-8-bit-bitmaps
            // Stackoverflow question: http://stackoverflow.com/questions/20872510/bitmapimage-outofmemoryexception-only-in-windows-8-1

            image.EndInit();
            image.Freeze();

            return image;
        }

        public static Sprite GetResourceSprite(string resourcePath, int decodePixelWidth = 0, int decodePixelHeight = 0)
        {
            var image = ResizeImage(GetResourceImage(resourcePath), decodePixelWidth, decodePixelHeight);
            return new Sprite(image);
        }

        private static BitmapSource ResizeImage(BitmapSource image, int width, int height)
        {
            if (width == 0 || height == 0)
                return image;

            // Convert to 32-bit before resizing! See explanation in GetResourceImage method (same bug)
            var bmp = new FormatConvertedBitmap(image, PixelFormats.Bgr32, null, 0);
            return new TransformedBitmap(bmp, new ScaleTransform((double)width/bmp.PixelWidth, (double)height/bmp.PixelHeight));
        }
    }
}