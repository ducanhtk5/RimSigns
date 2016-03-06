using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class SlideModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Images { get; set; }
        public Nullable<int> OrderImage { get; set; }
        public static void Mapfrom(Slide entity, ref SlideModel model)
        {
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Images = entity.Images;
            model.Description = entity.Description;
            model.OrderImage = entity.OrderImage;
        }
        public static void Mapfrom(SlideModel model, ref Slide entity)
        {
            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.Images = model.Images;
            entity.Description = model.Description;
            entity.OrderImage = model.OrderImage;
        }
    }

}