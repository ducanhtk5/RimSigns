using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class SettingModel
    {
        public int Id { get; set; }
        public string SettingName { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public static void Mapfrom(Setting entity, ref SettingModel model)
        {
            model.Id = entity.Id;
            model.SettingName = entity.SettingName;
            model.Body = entity.Body;
            model.Title = entity.Title;
        }
        public static void Mapfrom(SettingModel model, ref Setting entity)
        {
            entity.Id = model.Id;
            entity.SettingName = model.SettingName;
            entity.Body = model.Body;
            entity.Title = model.Title;
        }
    }
}