using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Areas.Administration.Models
{
    public class RolesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public System.DateTime DateCreate { get; set; }
        public bool Detele { get; set; }
        public int Roles_persmission { get; set; }
        public bool checkAddOrUdate { get; set; }
        public string Status { get; set; }
        public string systemnameOld { get; set; }
        public static void MapFrom(RolesModel model, ref Role entity)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.Active = model.Active;
            entity.DateCreate = model.DateCreate;
            entity.Detele = model.Detele;

        }

        public static void MapFrom(Role entity, ref RolesModel model)
        {
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.Active = entity.Active;
            model.DateCreate = entity.DateCreate;
            model.Detele = entity.Detele;
            if (entity.Active) model.Status = "Active";
            else model.Status = "Not Active";
        }

    }
}