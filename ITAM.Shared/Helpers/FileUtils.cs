using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ITAM.Shared.Helpers
{
    public class FileUtils
    {
        public static ImageSource LoadImage(string filePath)
        {
            var image = new BitmapImage();

            using var stream = File.OpenRead(filePath);

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();

            image.Freeze();

            return image;
        }
    }
}
