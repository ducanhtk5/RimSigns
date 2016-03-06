using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class PartiesModels
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime DateAssign { get; set; }
        public int AssignById { get; set; }
        public string AssignByName { get; set; }
        public string EmailAssgined { get; set; }
        
        ///////////// Field not Entity
        public string Name { get; set; }
        public string OrderName { get; set; }

        public static void ToModel(Party entity, ref PartiesModels model)
        {
            model.Id = entity.Id;
            model.OrderId = entity.OrderId;
            model.DateAssign = entity.DateAssign;
            model.AssignById = entity.AssignById;
            model.AssignByName = entity.AssignByName;
            model.EmailAssgined = entity.EmailAssigned;
        }
        public static void ToEntity(PartiesModels model, ref Party entity)
        {
            entity.Id = model.Id;
            entity.OrderId = model.OrderId;
            entity.DateAssign = model.DateAssign;
            entity.AssignById = model.AssignById;
            entity.AssignByName = model.AssignByName;
            entity.EmailAssigned = model.EmailAssgined;
        }
    }
}