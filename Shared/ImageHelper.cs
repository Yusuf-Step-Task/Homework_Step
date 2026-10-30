using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace Shared
{
    public class ImageHelper
    {
        private readonly string folderPath;
        public ImageHelper()
        {
            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Images2");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        public string GetImagePath(byte[] buffer)
        {
            ImageConverter ic = new ImageConverter();
            var data = ic.ConvertFrom(buffer);
            Image img = data as Image;
            if (img == null)
            {
                return string.Empty;
            }
            var imagePath = Path.Combine(folderPath, $"image{Guid.NewGuid()}.png");
            new Bitmap(img).Save(imagePath);
            return imagePath;
        }
        public byte[] GetBytesOfImage(string path)
        {
            var image = new Bitmap(path);
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }
        //-----------------------------------------------------------------
        public string TakeScreenShot()
        {
            Bitmap bmp = new Bitmap(1920, 1080);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, new Size(1920, 1080));
            }

            var path = Path.Combine(folderPath, $"screenshot{Guid.NewGuid()}.png");
            bmp.Save(path);
            return path;
        }
       //----------------------------------------------------
        public byte[] TakeScreenShotBytes()
        {
            using Bitmap bmp = new Bitmap(1920, 1080);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, new Size(1920, 1080));
            }

            using Bitmap small = new Bitmap(bmp, new Size(960, 540));
            using MemoryStream ms = new MemoryStream();
            small.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}