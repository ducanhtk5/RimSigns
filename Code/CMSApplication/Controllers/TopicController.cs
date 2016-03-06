using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSApplication.Models;
using CMS.Service.Repository;

namespace CMSApplication.Controllers
{
    public class TopicController : Controller
    {
        //
        // GET: /Topic/
        private TopicRepository Topicsrv = new TopicRepository();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TopicDetails(int ids)
        {
            string _absoft = Request.Url.Host;
            if (_absoft == "localhost" || _absoft == "http://dev.loveattil.com/" || _absoft == "dev.loveattil.com" || _absoft == "https://dev.loveattil.com/" || _absoft == "loveattil.com")
            {
                TopicModel model = new TopicModel();
                try
                {
                    var entity = Topicsrv.GetById(ids);
                    TopicModel.MapFrom(entity, ref model);
                    return View(model);
                }
                catch (Exception)
                {
                    return null;
                }

            }
            else
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }
        public ActionResult TopicByName(string systemname)
        {
            TopicModel model = new TopicModel();
            try
            {
                var entity = Topicsrv.getTopiccByName(systemname);
                TopicModel.MapFrom(entity, ref model);
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public ActionResult Detail(string systemname)
        {
            TopicModel model = new TopicModel();
            try
            {
                var entity = Topicsrv.getTopiccByName(systemname);
                TopicModel.MapFrom(entity, ref model);
                return View("TopicBlock", model);
            }
            catch (Exception)
            {
                return null;
            }

        }


        public ActionResult TopicBySystemWithTitle(string systemname, bool show_title=false)
        {
            string _absoft = Request.Url.Host;
            if (0<1)
            {
                TopicModel model = new TopicModel();
                try
                {
                    var entity = Topicsrv.getTopiccByName(systemname);
                    TopicModel.MapFrom(entity, ref model);

                    // show title
                    if (!show_title)
                        model.Title = "";

                    return PartialView(model);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }

        public ActionResult TopicByUser(string systemname)
        {
            string _absoft = Request.Url.Host;
            if (_absoft == "localhost" || _absoft == "http://dev.loveattil.com/" || _absoft == "dev.loveattil.com" || _absoft == "https://dev.loveattil.com/" || _absoft == "loveattil.com")
            {
                TopicModel model = new TopicModel();
                try
                {
                    var entity = Topicsrv.getTopiccByName(systemname);
                    TopicModel.MapFrom(entity, ref model);
                    return PartialView(model);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }
        public ActionResult TopicBlock(int id)
        {
            string _absoft = Request.Url.Host;
            //if (_absoft == "localhost" || _absoft == "http://dev.loveattil.com/" || _absoft == "dev.loveattil.com" || _absoft == "https://dev.loveattil.com/" || _absoft == "loveattil.com")
            if (0 < 1)
            {
                TopicModel model = new TopicModel();
                try
                {
                    var entity = Topicsrv.GetById(id);
                    TopicModel.MapFrom(entity, ref model);
                    ViewBag.Title = model.Title;
                    return View(model);
                }
                catch (Exception)
                {
                    return null;
                }

            }
            else
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }
    }
}
