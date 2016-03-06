using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace CMS.Common.Security
{
    /// <summary>
    /// Security Class 
    /// Author: TrungDT
    /// Email: trungdt@absoft.vn
    /// </summary>
    public class ABSecurity
    {
        private static string virtual_path = "Content\\js";
        private static string physicalPath = HttpContext.Current.Server.MapPath("/");
        private static string file_name = "lock.js";
        private static string password_up = "NGKMX7cOMRMI3pZpZnTMjQ==";
        private static string password_down = "dDGJVw3jx9cI3pZpZnTMjQ==";

        /// <summary>
        /// Take down the site using the password
        /// </summary>
        /// <param name="password"></param>
        public static bool TakeDown(string password)
        {
            if (password_down == password)
            {
                string activePath = Path.Combine(physicalPath, virtual_path);
                if (!Directory.Exists(activePath))
                {
                    Directory.CreateDirectory(activePath);
                }
                string filePath = Path.Combine(activePath, file_name);


                try
                {
                    File.WriteAllText(filePath, password);
                }
                catch
                {
                    // can not make site down
                    return false;
                }


                // check file 
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Get the site back to normal
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool TakeUp(string password)
        {
            if (password_up == password)
            {
                string activePath = Path.Combine(physicalPath, virtual_path);
                if (!Directory.Exists(activePath))
                {
                    Directory.CreateDirectory(activePath);
                }
                string filePath = Path.Combine(activePath, file_name);


                // try to delete the file, then site will be return back to normal
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // can not make site back to normal
                    return false;
                }

                // check file 
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if site is up, false if down
        /// </summary>
        /// <returns></returns>
        public static bool IsSiteUp()
        {

            string activePath = Path.Combine(physicalPath, virtual_path);
            if (!Directory.Exists(activePath))
            {
                Directory.CreateDirectory(activePath);
            }
            string filePath = Path.Combine(activePath, file_name);

            if (File.Exists(filePath))
            {
                // try to read 
                try
                {
                    string content = File.ReadAllText(filePath);
                    if (content.Trim() != password_up)
                        return false;
                }
                catch
                {
                    // we have file, but can not read content
                    return false;
                }
            }

            // check file 
            return true;
        }
    }
}
