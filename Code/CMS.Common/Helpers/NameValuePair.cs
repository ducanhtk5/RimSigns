using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Common.Helpers
{
    public class NameValuePair
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public NameValuePair() { }

        public NameValuePair(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
