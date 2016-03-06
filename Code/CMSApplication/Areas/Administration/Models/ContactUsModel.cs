using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSApplication.Areas.Administration.Models
{
    public class ContactUsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
    }
}