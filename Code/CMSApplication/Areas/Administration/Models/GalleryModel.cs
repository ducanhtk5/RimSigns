using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Areas.Administration.Models
{
    public class GalleryModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int CatalogID { get; set; }
        public Nullable<int> OrderImage { get; set; }
        public string Thumbnail { get; set; }
        public string NameCatalog { get; set; }
        public string ImageCatalog { get; set; }

        public static void Mapfrom(Gallery entity, ref GalleryModel model)
        {
            model.CatalogID = (int)entity.CatalogID;
            model.Name = entity.Name;
            model.Image = entity.Image;
            model.ID = entity.ID;
            model.OrderImage = entity.OrderImage;
            model.Thumbnail = entity.Thumbnail;
            
        }
        public static void Mapfrom(GalleryModel model, ref Gallery entity)
        {
            entity.CatalogID = model.CatalogID;
            entity.Name = model.Name;
            entity.Image = model.Image;
            entity.ID = model.ID;
            entity.OrderImage = model.OrderImage;
            entity.Thumbnail = model.Thumbnail;
            
        }
    }
}