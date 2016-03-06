using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Web.Mvc;

namespace CMS.Common.Helpers
{
    public static class WebHelper
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = Convert.ToInt32(e), Name = e.ToString() };

            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static string TrimStringForTitle(string text)
        {
            return text.Replace("\r", "").Replace("\n", "").Trim().Replace("  "," ");
        }
    }
}
