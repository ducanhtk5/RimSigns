using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Areas.Administration.Models
{
    public class MenuList
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public static void Mapfrom(Navigation entity, ref MenuList model)
        {
            model.ID = entity.Id;
            model.Name = entity.Name;
            model.ParentID = entity.ParentID;
        }
    }

}