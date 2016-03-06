using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSApplication.Models
{
    public class CommentAndFileUploadModel
    {
        public string OrderID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Contents { get; set; }
        public string UserName { get; set; }
        public int UserID { get; set; }
        public string FileUrl { get; set; }
        public string UserImage { get; set; }
        public int Id { get; set; }
        public bool IsImage { get; set; }
        public string Note { get; set; }
    }
}