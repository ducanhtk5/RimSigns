using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSApplication.Models
{
    public class ContactUsModel
    {
        public string Email { get; set; }
        public string Phone { get;set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string State { get; set; }
        public string CaptchaValue { get; set; }
    }
}