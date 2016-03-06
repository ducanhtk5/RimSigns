using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSApplication.Areas.Administration.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Pass { get; set; }
        public bool CheckRemember { get; set; }
    }
}