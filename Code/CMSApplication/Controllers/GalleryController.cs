using CMS.Service.Repository;
using CMSApplication.Areas.Administration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSApplication.Controllers
{
    public class GalleryController : Controller
    {
        //
        // GET: /Gallery/

        private GalleryRepository _gallery_srv = new GalleryRepository();
        private CatalogGalleryRepository _catalog_gallery = new CatalogGalleryRepository();

        public ActionResult Index()
        {
            List<CatalogGalleryModel> model = new List<CatalogGalleryModel>();
            var c = _catalog_gallery.List();
            foreach (var item in c)
            {
                var m = _gallery_srv.GetTableListById(item.CatalogID).ToList();
                CatalogGalleryModel ml = new CatalogGalleryModel();
                CatalogGalleryModel.Mapfrom(item, ref  ml);
                if (ml.Name.Length > 10)
                {
                    ml.Name = ml.Name.Substring(0, 10) + "...";
                }
                ml.CountImg = m.Count;
                model.Add(ml);
            }
            return View(model);
        }
        public ActionResult _ListCatalogGalery()
        {
            List<CatalogGalleryModel> model = new List<CatalogGalleryModel>();
            var a = _catalog_gallery.List();
            foreach (var item in a)
            {
                CatalogGalleryModel uu = new CatalogGalleryModel();
                CatalogGalleryModel.Mapfrom(item, ref uu);
                model.Add(uu);
            }
            return PartialView(model);
        }
        public ActionResult GalleryDetails(int id)
        {
            List<GalleryModel> model = new List<GalleryModel>();
            var m = _gallery_srv.GetTableListById(id).ToList();
            var cata = _catalog_gallery.GetById(id);
            foreach (var item in m)
            {
                GalleryModel uu = new GalleryModel();
                GalleryModel.Mapfrom(item, ref uu);
                uu.ImageCatalog = cata.Image;
                uu.NameCatalog = cata.Name;
                ViewBag.ImageCatalog_gallery = cata.Image;
                ViewBag.NameCatalog_gallery = cata.Name;
                ViewBag.CountCatalog_gallery = m.Count;
                ViewBag.ImagePerRow = cata.ImagesPerRow;
                ViewBag.RowPerPage = cata.RowPerPage;
                ViewBag.Title = uu.NameCatalog;
                model.Add(uu);
            }

            ViewBag.CatalogTitle = cata.Name;
            ViewBag.CatalogMetaTitle = cata.MetaTitle;
            ViewBag.CatalogMetaDesc = cata.MetaDescription;
            ViewBag.CatalogMetaKeywords = cata.MetaKeywords;
            return View(model);
        }
        public ActionResult GetBannerGallery(int id)
        {
            GalleryModel model = new GalleryModel();
            ViewBag.Image = _catalog_gallery.GetById(id).Image;
            return PartialView();
        }
    }
}
