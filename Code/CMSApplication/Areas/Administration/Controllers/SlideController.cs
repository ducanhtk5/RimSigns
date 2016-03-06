using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Common.Mvc;
using CMS.Data;
using CMSApplication.Areas.Administration.Models;
using CMS.Service.Repository;
using CMS.Common.Helpers;
using System.IO;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class SlideController : UTController
    {
        //
        // GET: /Administration/Slide/
        SlideRepository _slider_srv = new SlideRepository();
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            return View();
        }
          [Authorize(Roles = "Administrator,Manager")]
        public ActionResult ListSlide()
        {
            List<SlideModel> model = new List<SlideModel>();
            var a = _slider_srv.List().OrderBy(m => m.OrderImage);
            foreach (var item in a)
            {
                SlideModel ml = new SlideModel();
                SlideModel.Mapfrom(item, ref  ml);
                model.Add(ml);
            }
            return PartialView(model);
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Add()
        {
            return View();
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
                Slide entity = _slider_srv.GetById(gallery_id);

                IEnumerable<Slide> a = new List<Slide>();
                 Slide temp = new Slide ();
                // get the nearest
                if (direction == 0) // up
                {
                    a = _slider_srv.List().OrderBy(m => m.OrderImage).Where(m => m.OrderImage < entity.OrderImage);
                    if (a.Count() > 0)
                        temp = a.Last();
                }
                else
                {
                    a = _slider_srv.List().OrderBy(m => m.OrderImage).Where(m => m.OrderImage > entity.OrderImage);
                    if (a.Count() > 0)
                        temp = a.First();
                }

                if (temp.Id > 0)
                {
                    int? t = temp.OrderImage;
                    temp.OrderImage = entity.OrderImage;
                    entity.OrderImage = t;
                    _slider_srv.CreateOrUpdate(entity);
                    _slider_srv.CreateOrUpdate(temp);
                }
            }
            catch (Exception)
            {
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int id)
        {
            SlideModel model = new SlideModel();
            var entity = _slider_srv.GetById(id);
            SlideModel.Mapfrom(entity, ref model);
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddOrUpdate(SlideModel model, HttpPostedFileBase FileUp)
        {
            Slide entity = new Slide();
            SlideModel.Mapfrom(model, ref entity);

            // update order
            if (model.Id == 0)
            {
                IEnumerable<Slide> a = new List<Slide>();
                a = _slider_srv.List().OrderByDescending(m => m.OrderImage);
                if (a.Count() == 0)
                {
                    entity.OrderImage = 1;
                }
                else
                {
                    entity.OrderImage = a.Max(c => c.OrderImage) + 1;
                }
                entity.Images = UploadFile(model.Title, FileUp);
            }
            else
            {
                entity.OrderImage = model.OrderImage;
                entity.Images = UploadFile(model.Title, FileUp);
            }

            _slider_srv.CreateOrUpdate(entity);
            return RedirectToAction("Index");
        }
          [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int id)
        {
           var slide= _slider_srv.GetById(id);
           string basePath = "~/Content/File/ImagesUpload";
           FileHelper file = new FileHelper(basePath);
           file.DeleteFile(slide.Images);
           _slider_srv.Delete(id);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowSlide()
        {
            try
            {
                List<SlideModel> model = new List<SlideModel>();
                var a = _slider_srv.List();
                foreach (var item in a)
                {
                    SlideModel ml = new SlideModel();
                    SlideModel.Mapfrom(item, ref  ml);
                    model.Add(ml);
                }
                return PartialView(model.OrderBy(m => m.OrderImage));
            }
            catch (Exception)
            {
                               
            }
            return PartialView();
        }

        private string UploadFile(string galleryName, HttpPostedFileBase FileUp)
        {
            var db_path = "";

            if (string.IsNullOrEmpty(galleryName))
            {
                galleryName = "Default";
            }
            string folder = galleryName.Replace("&", "").Replace("/", "").Replace("?", "");
            string filenamefinal = string.Empty;
            folder = EscapeName.Renamefile(folder);
            if (FileUp != null)
            {
                if (FileUp.ContentLength > 0)
                {
                    var dir = Server.MapPath("~/Content/File/ImagesUpload/");
                    var path = "";

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    var ext = Path.GetExtension(FileUp.FileName).ToLower();
                    if (ext == ".gif" || ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {
                        dir = Server.MapPath("~/Content/File/ImagesUpload");
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        var filename = Path.GetFileNameWithoutExtension(FileUp.FileName);
                        filename = EscapeName.Renamefile(filename);
                        string tmp = Path.GetRandomFileName().Substring(0, 3);
                        filenamefinal = folder + "-" + filename + "-" + tmp + ext;
                        path = Path.Combine(Server.MapPath("~/Content/File/ImagesUpload"), filenamefinal);
                        db_path = "Content/File/ImagesUpload/" + filenamefinal;
                        FileUp.SaveAs(path);
                    }
                }
            }
            return filenamefinal;
        }

    }
}
