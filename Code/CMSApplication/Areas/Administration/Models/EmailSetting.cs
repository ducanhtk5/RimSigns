using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CMSApplication.Areas.Administration.Models
{
    public class EmailSetting
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string EmailRecieve { get; set; }
        [Required]
        public string Host { get; set; }
        [Required]
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string DisplayName { get; set; }
    }
}