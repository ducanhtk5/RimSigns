using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class OrderActivitiesModels
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string FileLink { get; set; }
        public int OrderUserID { get; set; }
        public string Actions { get; set; }
        public string OrderUserName { get; set; }
        public int UpdateByUserID { get; set; }
        public string UpdateByUserName { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Enable { get; set; }
        // for temporary use when list in dashboard
        public string UserImage { get; set; }

        public static void ToModel(OrderActivity entity, ref OrderActivitiesModels model)
        {
            model.Id = entity.Id;
            model.OrderId = (int)entity.OrderID;
            model.FileLink = entity.FileLink;
            model.OrderUserID = (int)entity.OrderUserID;
            model.Actions = entity.Actions;
            model.OrderUserName = entity.OrderName;
            model.CreatedTime = (DateTime)entity.CreatedTime;
            model.Enable = (int)entity.Enable;
            model.UpdateByUserID = (int)entity.UpdateByUserID;
            model.UpdateByUserName = entity.UpdateByUserName;
        }

        public static void ToEntity(OrderActivitiesModels model, ref  OrderActivity entity)
        {
            entity.Id = model.Id;
            entity.OrderID = model.OrderId;
            entity.FileLink = model.FileLink;
            entity.OrderUserID = model.OrderUserID;
            entity.Actions = model.Actions;
            entity.OrderUserName = model.OrderUserName;
            entity.CreatedTime = (DateTime)model.CreatedTime;
            entity.Enable = model.Enable;
            entity.UpdateByUserID = model.UpdateByUserID;
            entity.UpdateByUserName = model.UpdateByUserName;
        }
    }
}