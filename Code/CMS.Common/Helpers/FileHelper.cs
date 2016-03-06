using System;
using System.IO;
using System.Web;

namespace CMS.Common.Helpers
{
    public class FileHelper
    {
        private string physicalPath = string.Empty;

        public FileHelper(string virtualDir) 
        {
            if (string.IsNullOrEmpty(virtualDir))
            {
                throw new ArgumentNullException("virtualDir");
            }

            physicalPath = HttpContext.Current.Server.MapPath(virtualDir);

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }
        }

        public void DeleteFile(string fileName)
        {
            string filePath = Path.Combine(physicalPath, fileName);

            File.Delete(filePath);
        }

        public string SaveFile(string subDirectory, string extension, Stream inputStream)
        {
            string activePath = physicalPath;
            if (!string.IsNullOrEmpty(subDirectory))
            {
                activePath = Path.Combine(physicalPath, subDirectory);
                if (!Directory.Exists(activePath))
                {
                    Directory.CreateDirectory(activePath);
                }
            }
            string fileName = "Image_"+Guid.NewGuid().ToString("N").Substring(28) + "." + extension;
            string filePath = Path.Combine(activePath, fileName);

            try
            {
                byte[] buffer = new byte[inputStream.Length];
                inputStream.Read(buffer, 0, buffer.Length);
                File.WriteAllBytes(filePath, buffer);
            }
            catch
            {
                fileName = null;
            }

            return fileName;
        }
    }
}
