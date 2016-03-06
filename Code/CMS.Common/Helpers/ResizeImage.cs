using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Web;
using System.IO;

namespace CMS.Common.Helpers
{
    public class ResizeImage
    {
        public static Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            return result;
        }

        public static string ResizeByMaxWidth(string dir, HttpPostedFileBase file, int max_width, int max_height)
        {
            string filename = file.FileName;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string tmp = Path.GetRandomFileName().Substring(0, 3);
            filename = EscapeName.Renamefile(tmp+"-"+ filename );
            System.Drawing.Image bm = System.Drawing.Image.FromStream(file.InputStream);
            // calculate new width and height
            int new_width = bm.Width;
            int new_height = bm.Height;
            if (new_width > max_width)
            {
                float ratio = (float)max_width / (float)new_width;
                new_width = (int)(new_width* ratio);
                new_height = (int) (new_height * ratio);
            }
            else if (new_height > max_height)
            {
                float ratio = (float)max_height / (float)new_height;
                new_width = (int)(new_width * ratio);
                new_height = (int)(new_height * ratio);
            }
            bm = ResizeBitmap((Bitmap)bm, new_width, new_height);
            bm.Save(Path.Combine(dir, filename));
            return filename;
        }

        public static void Resize(string path, string subfolder, HttpPostedFileBase file, string filenamefinal, int nWidth, int nHeight)
        {
            string dir = string.Empty;
            string filename = file.FileName;
            if (!String.IsNullOrEmpty(subfolder))
            {
                dir = path + "\\" + subfolder;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            if (!String.IsNullOrEmpty(filenamefinal))
            {
                filename = filenamefinal;
            }
            System.Drawing.Image bm = System.Drawing.Image.FromStream(file.InputStream);
            bm = ResizeBitmap((Bitmap)bm, nWidth, nHeight);
            bm.Save(Path.Combine(dir, filename));
        }
    }
}
