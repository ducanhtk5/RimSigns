using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CMS.Common.Helpers
{
    public class EscapeName
    {
        public static string Renamefile(string filename)
        {
            filename = Regex.Replace(filename, @"[^\u0000-\u007F]", String.Empty);
            filename = Regex.Replace(filename, @"\s", "-");
            filename = filename.Replace(" ", "");
            filename = filename.Replace("#", "");
            filename = filename.Replace("@", "");
            filename = filename.Replace("?", "");
            filename = filename.Replace("<", "");
            filename = filename.Replace(">", "");
            filename = filename.Replace("\"", "");
            filename = filename.Replace("'", "");
            filename = filename.Replace("`", "");
            filename = filename.Replace("%", "");
            filename = filename.Replace("!", "");
            filename = filename.Replace("*", "");
            filename = filename.Replace("/", "");
            filename = filename.Replace("$", "");
            filename = filename.Replace("%", "");
            filename = filename.Replace("&", "");
            filename = filename.Replace("^", "");
            filename = filename.Replace("+", "");
            filename = filename.Replace("~", "");
            filename = filename.Replace("{", "");
            filename = filename.Replace("[", "");
            filename = filename.Replace("}", "");
            filename = filename.Replace("]", "");
            filename = filename.Replace("|", "");
            filename = filename.Replace(":", "");
            filename = filename.Replace(";", "");

            return filename;
        }
    }
}
