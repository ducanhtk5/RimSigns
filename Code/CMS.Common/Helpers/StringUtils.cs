using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Common.Helpers
{
    public class StringUtils
    {
        public static bool? GetValueBool(string str)
        {
            bool returnVal = false;
            if (!Boolean.TryParse(str.ToLowerInvariant(), out returnVal))
            {
                return null;
            }
            return returnVal;
        }

        public static int? GetValueInt(string str)
        {
            int returnVal = 0;
            if (!Int32.TryParse(str.ToLowerInvariant(), out returnVal))
            {
                return null;
            }
            return returnVal;
        }
    }
}
