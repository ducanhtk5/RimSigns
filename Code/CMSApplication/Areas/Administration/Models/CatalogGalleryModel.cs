using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Areas.Administration.Models
{
    public class CatalogGalleryModel
    {
        public int CatalogID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int CountImg { get; set; }
        public int ImagesPerRow { get; set; }
        public int RowPerPage { get; set; }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }

        public static void Mapfrom(CatalogGallery entity, ref CatalogGalleryModel model)
        {
            model.CatalogID = entity.CatalogID;
            model.Name = entity.Name;
            model.Description = entity.Description;
            model.Image = entity.Image;
            model.ImagesPerRow = entity.ImagesPerRow;
            model.RowPerPage = (int)entity.RowPerPage;

            model.MetaDescription = entity.MetaDescription;
            model.MetaTitle = entity.MetaTitle;
            model.MetaKeywords = entity.MetaKeywords;            
        }
        public static void Mapfrom(CatalogGalleryModel model, ref CatalogGallery entity)
        {
            entity.CatalogID = model.CatalogID;
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Image = model.Image;
            entity.ImagesPerRow = model.ImagesPerRow;
            entity.RowPerPage = model.RowPerPage;

            entity.MetaDescription = model.MetaDescription;
            entity.MetaTitle = model.MetaTitle;
            entity.MetaKeywords = model.MetaKeywords;            
        }
    }
}