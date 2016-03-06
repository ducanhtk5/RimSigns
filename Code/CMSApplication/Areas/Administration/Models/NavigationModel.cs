using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Areas.Administration.Models
{
    public class NavigationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public int OrderMenu { get; set; }
        public int ParentId { get; set; }
        public int Menutype { get; set; }
        public bool Active { get; set; }
        public static void Mapfrom(Navigation entity, ref NavigationModel model)
        {
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.Link = entity.Link;
            model.OrderMenu = entity.OrderMenu;
            model.ParentId = entity.ParentID;
            model.Menutype = entity.MenuType;
            model.Active = entity.Active;
        }
        public static void Mapfrom(NavigationModel model, ref Navigation entity)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.Link = model.Link;
            entity.OrderMenu = model.OrderMenu;
            entity.ParentID = model.ParentId;
            entity.MenuType = model.Menutype;
            entity.Active = model.Active;
        }
    }
}