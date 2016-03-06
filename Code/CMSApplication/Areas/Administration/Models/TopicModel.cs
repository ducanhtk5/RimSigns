using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Areas.Administration.Models
{
    public class TopicModel
    {
        public int Id { get; set; }
        public string SystemName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }

        public static void MapFrom(TopicModel model, ref Topic entity)
        {
            entity.ID = model.Id;
            entity.SystemName = model.SystemName;
            entity.Title = model.Title;
            entity.Body = model.Body;
            entity.MetaDescription = model.MetaDescription;
            entity.MetaTitle = model.MetaTitle;
            entity.MetaKeyword = model.MetaKeywords;
        }

        public static void MapFrom(Topic entity, ref TopicModel model)
        {
            model.Id = entity.ID;
            model.SystemName = entity.SystemName;
            model.Title = entity.Title;
            model.Body = entity.Body;
            model.MetaDescription = entity.MetaDescription;
            model.MetaTitle = entity.MetaTitle;
            model.MetaKeywords = entity.MetaKeyword;
        }

    }
}