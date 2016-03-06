using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Common.Helpers
{
    public class Parameters
    {
        public static string ApplicationName { get { return GetValue("ApplicationName"); } }

        public static string DefaultProfileLogo { get { return GetValue("DefaultProfileLogo"); } }
        public static string DefaultEventLogoStorageDir { get { return GetValue("DefaultEventLogoStorageDir"); } }

        public static string GetValue(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }
            return "CMSApplication";
        }

        public static int GetValueInt(string name, int defaultValue)
        {
            string value = GetValue(name);
            int? intValue = StringUtils.GetValueInt(value);
            return (intValue == null) ? defaultValue : intValue.Value;
        }

        public static bool GetValueBool(string name, bool defaultValue)
        {
            string value = GetValue(name);
            bool? boolValue = StringUtils.GetValueBool(value);
            return (boolValue == null) ? defaultValue : boolValue.Value;
        }
    }
}
