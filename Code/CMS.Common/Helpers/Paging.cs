using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Common.Helpers
{
    public class PagedData<T> where T : class
    {
        public List<T> Data { get; set; }
        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }
        public int First { get; set; }
        public int Last { get; set; }
        public int Next { get; set; }
        public int Prev { get; set; }
    }
}