using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Data;
using CMS.Service.Repository;
using CMSApplication.Areas.Administration.Models;
using CMS.Common.Mvc;
using CMS.Common.Helpers;
using System.IO;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class GalleryManagerController : UTController
    {
        private CatalogGalleryRepository _catalog_gallery = new CatalogGalleryRepository();
        private GalleryRepository _gallery = new GalleryRepository();
        //
        // GET: /Administration/GalleryManager/
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            string _absoft =Request.Url.Host;
            if (0<1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
        }
        #region      Catalog Galery
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult ListCataLogGallery()
        {
            List<CatalogGalleryModel> model = new List<CatalogGalleryModel>();
            var a = _catalog_gallery.List();
            foreach (var item in a)
            {
                CatalogGalleryModel ml = new CatalogGalleryModel();
                CatalogGalleryModel.Mapfrom(item, ref  ml);
                model.Add(ml);
            }

            return PartialView("_ListCataLogGallery", model);
        }
        public ActionResult GalleryById(int id)
        {
            List<GalleryModel> model = new List<GalleryModel>();
            var m = _gallery.GetTableListById(id).ToList();
            foreach (var item in m)
            {
                GalleryModel uu = new GalleryModel();
                GalleryModel.Mapfrom(item, ref uu);
                model.Add(uu);
            }
            return View(model);
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddCatalogGallery()
        {
            CatalogGalleryModel model = new CatalogGalleryModel();
            model.CatalogID = 0;
            model.RowPerPage = 5;
            model.ImagesPerRow = 3;
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddOrUpdateCatalogGallery(CatalogGalleryModel model, HttpPostedFileBase FileUp)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ViewBag.Message = "Please enter catalog gallary name.";
                return Redirect(Url.RequestContext.HttpContext.Request.UrlReferrer.ToString());
            }
            //if (FileUp == null || FileUp.ContentLength < 1)
            //{
            //    ViewBag.Message = "Please select a image.";
            //    return View(Url.RequestContext.HttpContext.Request.UrlReferrer.ToString());
            //}

            CatalogGallery entity = new CatalogGallery();
            CatalogGalleryModel.Mapfrom(model, ref entity);

            if (FileUp != null && FileUp.ContentLength > 1)
            {
                string fileName = UploadFile(model.Name, FileUp);
                entity.Image = fileName;
            }

            _catalog_gallery.CreateOrUpdate(entity);

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult EditCatalogGallery(int id)
        {
            CatalogGalleryModel model = new CatalogGalleryModel();
            var entity = _catalog_gallery.GetById(id);
            CatalogGalleryModel.Mapfrom(entity, ref model);
            return View(model);
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteCatalogGallery(int id)
        {
            _catalog_gallery.Delete(id);
            
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Gallery
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Gallery()
        {
            string _absoft =Request.Url.Host;
            
            //if (_absoft == "localhost" || _absoft == "http://dev.loveattil.com/" || _absoft == "dev.loveattil.com" || _absoft == "https://dev.loveattil.com/" || _absoft == "loveattil.com")
            if (0<1)
            {
                //List Catalog Gallery.
                List<CatalogGallery> c = new List<CatalogGallery>();
                var ListCatalogGallery = _catalog_gallery.List();
                c.Add(new CatalogGallery { CatalogID = 0, Name = "-- All Gallery --" });
                c.AddRange(ListCatalogGallery);
                ViewData["CatalogID"] = new SelectList(c, "CatalogID", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddGallery()
        {
            List<CatalogGallery> c = new List<CatalogGallery>();
            var ListCatalogGallery = _catalog_gallery.List();
            c.AddRange(ListCatalogGallery);
            ViewData["CatalogID"] = new SelectList(c, "CatalogID", "Name");
            return View();
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult EditGallery(int id)
        {
            try
            {
                int CatalogID = (int)_gallery.GetById(id).CatalogID;
                List<CatalogGallery> c = new List<CatalogGallery>();
                var ListCatalogGallery = _catalog_gallery.List();
                c.AddRange(ListCatalogGallery);
                ViewData["CatalogID"] = new SelectList(c, "CatalogID", "Name", CatalogID);

            }
            catch (Exception)
            {


            }
            GalleryModel model = new GalleryModel();
            var entity = _gallery.GetById(id);
            GalleryModel.Mapfrom(entity, ref model);
            return View(model);
        }

        /// <summary>
        /// Move up or down
        /// </summary>
        /// <param name="gallery_id"></param>
        /// <param name="direction">=0: Up ; =1: Down</param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Move(int gallery_id, int direction)
        {
            try
            {
                Gallery entity = _gallery.GetById(gallery_id);

                IEnumerable<Gallery> a = new List<Gallery>();
                Gallery temp = new Gallery();
                // get the nearest
                if (direction == 1) // down
                {
                    a = _gallery.List().OrderBy(m => m.OrderImage).Where(m => m.OrderImage < entity.OrderImage);
                    if (a.Count() > 0)
                        temp = a.Last();
                }
                else
                {
                    a = _gallery.List().OrderBy(m => m.OrderImage).Where(m => m.OrderImage > entity.OrderImage);
                    if (a.Count() > 0)
                        temp = a.First();
                }

                if (temp.ID > 0)
                {
                    int? t = temp.OrderImage;
                    temp.OrderImage = entity.OrderImage;
                    entity.OrderImage = t;
                    _gallery.CreateOrUpdate(entity);
                    _gallery.CreateOrUpdate(temp);
                }
            }
            catch (Exception)
            {


            }

            return RedirectToAction("Gallery");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult CreateOrUpdateGallery(GalleryModel model, IEnumerable<HttpPostedFileBase> FileUp)
        {

            if (model.CatalogID == 0)
            {
                ViewBag.Message = "Please select catalog gallery name.";
                return Redirect(Url.RequestContext.HttpContext.Request.UrlReferrer.ToString());
            }
            // update order
            if (model.ID == 0)
            {

                if (FileUp.Count() > 0 && FileUp.First() != null && FileUp.First().ContentLength > 1)
                {
                    int order = 0;
                    foreach (var file in FileUp)
                    {
                        Gallery entity = new Gallery();
                        GalleryModel.Mapfrom(model, ref entity);
                        order++;
                        string fileName = UploadFile(model.Name, file);
                        //entity.Image = EscapeName.Renamefile(model.Name) + "/" + fileName;
                        //entity.Thumbnail = EscapeName.Renamefile(model.Name) + "/" + fileName;
                        entity.Image = fileName;
                        entity.Thumbnail = fileName;
                        entity.OrderImage = order;
                        _gallery.CreateOrUpdate(entity);
                    }
                }

            }
            else
            {
                Gallery entity = new Gallery();

                // get old entity
                var x= _gallery.List().Where(m => m.ID == model.ID);
                if (x.Count() > 0)
                {
                    entity = x.First();

                    entity.CatalogID = model.CatalogID;
                    entity.Name = model.Name;

                    if (FileUp.Count()>0 && FileUp.First()!=null && FileUp.First().ContentLength>1)
                    {
                        string fileName = UploadFile(model.Name, FileUp.First());
                        entity.Image = fileName;
                        entity.Thumbnail = fileName;
                    }

                    //GalleryModel.Mapfrom(model, ref entity);
                    //string fileName = UploadFile(model.Name, FileUp.First());
                    //entity.Image = fileName;
                    //entity.Thumbnail = fileName;
                    _gallery.CreateOrUpdate(entity);
                }
            }
            return RedirectToAction("Gallery");
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult ListGallery(int page, int CatalogID)
        {
            //List Gallery.
            List<GalleryModel> model = new List<GalleryModel>();
            IEnumerable<Gallery> a = new List<Gallery>();
            if (CatalogID == 0)
            {
                a = _gallery.List().OrderByDescending(m => m.OrderImage);
            }
            else
            {
                a = _gallery.List().Where(m => m.CatalogID == CatalogID).OrderByDescending(m => m.OrderImage);
            }
            foreach (var item in a)
            {
                GalleryModel ml = new GalleryModel();
                GalleryModel.Mapfrom(item, ref  ml);
                model.Add(ml);
            }
            //var ListNewsPageSize = new PagedData<GalleryModel>();

            //if (model.Count() > 0)
            //{
            //    ListNewsPageSize.Data = model.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
            //    if (ListNewsPageSize.Data.Count() == 0)
            //    {
            //        ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
            //    }
            //    ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)model.Count() / PageSize));
            //    ListNewsPageSize.CurrentPage = page;
            //}
            //else
            //{
            //    ListNewsPageSize.Data = new List<GalleryModel>();
            //    ListNewsPageSize.NumberOfPages = 0;
            //    ListNewsPageSize.CurrentPage = 0;
            //}

            return PartialView("_ListGallery", model);
        }



        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteGallery(int id)
        {
            _gallery.Delete(id);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion


        private string UploadFile(string galleryName, HttpPostedFileBase FileUp)
        {
            var db_path = "";

            if (string.IsNullOrEmpty(galleryName))
            {
                galleryName = "Default";
            }
            string folder = galleryName.Replace("&","").Replace("/","").Replace("?","");
            string filenamefinal = string.Empty;
            folder = EscapeName.Renamefile(folder);
            if (FileUp != null)
            {
                if (FileUp.ContentLength > 0)
                {
                    var dir = Server.MapPath("~/Content/File/Remsigns/");
                    var path = "";

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    var ext = Path.GetExtension(FileUp.FileName).ToLower();
                    if (ext == ".gif" || ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {
                        dir = Server.MapPath("~/Content/File/Remsigns");
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        var filename = Path.GetFileNameWithoutExtension(FileUp.FileName);
                        string tmp = Path.GetRandomFileName().Substring(0, 3);
                        filenamefinal = EscapeName.Renamefile(folder + "-" + filename + "-" + tmp) + ext;
                        path = Path.Combine(Server.MapPath("~/Content/File/Remsigns"), filenamefinal);
                        db_path = "Content/File/Remsigns/" + filenamefinal;
                        //FileUp.SaveAs(path);
                        filenamefinal = CMS.Common.Helpers.ResizeImage.ResizeByMaxWidth(dir, FileUp, 800, 600);
                    }
                }
            }
            return filenamefinal;
        }
    }
}
