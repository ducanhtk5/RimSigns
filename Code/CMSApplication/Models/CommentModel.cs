using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Contents { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Logo { get; set; }

        public static void ToEntity(CommentModel model, ref Comment entity)
        {
            entity.Id = model.Id;
            entity.OrderId = model.OrderId;
            entity.UserName = model.UserName;
            entity.Contents = model.Contents;
            entity.CreatedTime = model.CreatedTime;
            entity.UserId = model.UserId;
            entity.Logo = model.Logo;
        }

        public static void ToModel(Comment entity, ref CommentModel model)
        {
            model.Id = entity.Id;
            model.OrderId = (int)entity.OrderId;
            model.UserName = entity.UserName;
            model.Contents = entity.Contents;
            model.CreatedTime = (DateTime)entity.CreatedTime;
            model.UserId = (int)entity.UserId;
            model.Logo = entity.Logo;
        }

    }
}